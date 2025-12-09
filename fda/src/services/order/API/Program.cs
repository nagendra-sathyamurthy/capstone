using MongoDB.Driver;
using Order.API;
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
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Order API", 
        Version = "v1",
        Description = "Order Management Microservice for Food Delivery Application"
    });
});

// Register MongoDB client - Read connection string from secret
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = GetSecret(configuration, "mongo_connection_string", "mongodb://localhost:27017");
    return new MongoClient(connectionString);
});

// Register services
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<DeliveryService>();

// Add controllers
builder.Services.AddControllers();

// Add JWT authentication
var jwtKey = "GJ0VFqmRVBR0iE2ojyzh28HlayZgRcUI";
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Add role-based authorization
builder.Services.AddAuthorization(options =>
{
    // Restaurant Staff (Biller/Owner, Operator, Worker)
    options.AddPolicy("RestaurantStaff", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("role", "Biller") ||
            context.User.HasClaim("role", "Operator") ||
            context.User.HasClaim("role", "Worker")));
    
    // Restaurant Operator Only
    options.AddPolicy("RestaurantOperatorOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("role", "RestaurantOperator") ||
            context.User.HasClaim("role", "Operator") ||
            context.User.HasClaim("role", "KitchenWorker") ||
            context.User.HasClaim("role", "Worker")));
    
    // Delivery Agent policy
    options.AddPolicy("DeliveryAgent", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("role", "DeliveryAgent")));
    
    // Delivery Agent Only
    options.AddPolicy("DeliveryAgentOnly", policy =>
        policy.RequireRole("DeliveryAgent"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
