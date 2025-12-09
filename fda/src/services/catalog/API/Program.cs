using MongoDB.Driver;
using catalog.API;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Helper function to read secrets from Docker secrets or environment variables
static string GetSecret(IConfiguration configuration, string secretName, string? defaultValue = null)
{
    // Try to read from Docker secret file first
    var secretPath = $"/run/secrets/{secretName}";
    if (File.Exists(secretPath))
    {
        return File.ReadAllText(secretPath).Trim();
    }
    
    // Try direct environment variable (MONGO_CONNECTION_STRING)
    var envVarName = secretName.ToUpper();
    var envValue = configuration[envVarName];
    if (!string.IsNullOrEmpty(envValue))
    {
        return envValue;
    }
    
    // Try with double underscores for .NET configuration format (ConnectionStrings__DefaultConnection)
    var configKey = secretName.Replace("_", "__");
    envValue = configuration[configKey];
    if (!string.IsNullOrEmpty(envValue))
    {
        return envValue;
    }
    
    // Try as-is (original secretName)
    envValue = configuration[secretName];
    if (!string.IsNullOrEmpty(envValue))
    {
        return envValue;
    }
    
    // Use default if provided
    return defaultValue ?? throw new InvalidOperationException($"Secret '{secretName}' not found");
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog API", Version = "v1" });
});

// Register MongoDB client - Read connection string from secret
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = GetSecret(configuration, "mongo_connection_string", "mongodb://localhost:27017");
    return new MongoClient(connectionString);
});

// Register services
builder.Services.AddScoped<ItemService>(); // Keep for backward compatibility
builder.Services.AddScoped<MenuService>(); // New food delivery menu service
builder.Services.AddScoped<RestaurantService>(); // Restaurant management service
builder.Services.AddScoped<OrderService>(); // Order management service for operators

// Add controllers
builder.Services.AddControllers();

// Add JWT authentication
var jwtKey = "GJ0VFqmRVBR0iE2ojyzh28HlayZgRcUI";

// Clear default claim mappings to preserve original claim types
Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = "role", // Map the "role" claim to the role claim type
            NameClaimType = "email" // Map the "email" claim to the name claim type
        };
    });

// Add role-based authorization
builder.Services.AddAuthorization(options =>
{
    // Restaurant Owner policy (role: Biller in the system, represents Restaurant Owner)
    options.AddPolicy("RestaurantOwnerOnly", policy =>
        policy.RequireRole("Biller", "RestaurantOwner"));
    
    // Restaurant Staff (Biller, Operator, Worker)
    options.AddPolicy("RestaurantStaff", policy =>
        policy.RequireRole("Biller", "Operator", "Worker"));
    
    // Restaurant Operator policy
    options.AddPolicy("RestaurantOperatorOnly", policy =>
        policy.RequireRole("RestaurantOperator", "Operator", "KitchenWorker", "Worker"));
    
    // Delivery Agent policy
    options.AddPolicy("DeliveryAgentOnly", policy =>
        policy.RequireRole("DeliveryAgent"));
    
    // IT Admin policy
    options.AddPolicy("ITAdminOnly", policy =>
        policy.RequireRole("ITAdmin", "Admin"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
//app.UseAuthentication(); // Temporarily disabled for testing
//app.UseAuthorization(); // Temporarily disabled for testing

app.MapControllers();

app.Run();
