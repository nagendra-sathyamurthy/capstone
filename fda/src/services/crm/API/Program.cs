using Crm.API;
using MongoDB.Driver;


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


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

// Register MongoDB client - CRM uses a different secret with database specified
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = GetSecret(configuration, "mongo_connection_string_crm", "mongodb://localhost:27017/crmdb");
    return new MongoClient(connectionString);
});

// Register IMongoDatabase for DI
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    // Use the database name from connection string or hardcode as needed
    return client.GetDatabase("crmdb");
});

// Register CustomerService
builder.Services.AddScoped<CustomerService>();

// Register UserProfileService
builder.Services.AddScoped<UserProfileService>();

// JWT Authentication configuration
var jwtKey = "GJ0VFqmRVBR0iE2ojyzh28HlayZgRcUI";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Enable Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM Service V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


app.MapControllers();

app.Run();
