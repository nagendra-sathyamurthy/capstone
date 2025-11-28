using MongoDB.Driver;
using catalog.API;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Helper function to read secrets from Docker secrets or environment variables
static string GetSecret(IConfiguration configuration, string secretName, string? defaultValue = null)
{
    var secretPath = $"/run/secrets/{secretName}";
    if (File.Exists(secretPath))
    {
        return File.ReadAllText(secretPath).Trim();
    }
    
    var envValue = configuration[secretName.Replace("_", "__").ToUpper()];
    if (!string.IsNullOrEmpty(envValue))
    {
        return envValue;
    }
    
    envValue = configuration[secretName];
    if (!string.IsNullOrEmpty(envValue))
    {
        return envValue;
    }
    
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
    // Restaurant Owner policy (role: Biller in the system, represents Restaurant Owner)
    options.AddPolicy("RestaurantOwnerOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("role", "Biller")));
    
    // Restaurant Staff (Biller, Operator, Worker)
    options.AddPolicy("RestaurantStaff", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("role", "Biller") ||
            context.User.HasClaim("role", "Operator") ||
            context.User.HasClaim("role", "Worker")));
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
