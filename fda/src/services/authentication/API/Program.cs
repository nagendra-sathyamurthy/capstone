
using Authentication.DataAccess;
using Authentication.Models;
using Authentication.API.BusinessServices;
using Authentication.API.Middleware;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
    
    // Fallback to environment variable (for local development)
    var envValue = configuration[secretName.Replace("_", "__").ToUpper()];
    if (!string.IsNullOrEmpty(envValue))
    {
        return envValue;
    }
    
    // Use default if provided
    return defaultValue ?? throw new InvalidOperationException($"Secret '{secretName}' not found");
}

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "FDA Authentication Service", Version = "v1" });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// JWT Authentication - Read secret from file or environment
var jwtSecret = GetSecret(builder.Configuration, "jwt_secret_key", "GJ0VFqmRVBR0iE2ojyzh28HlayZgRcUI");
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// Add role-based authorization policies
builder.Services.AddRoleBasedPolicies();

// CORS configuration for development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// MongoDB setup - Read connection string from secret or environment
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = GetSecret(configuration, "mongo_connection_string", "mongodb://localhost:27017");
    var mongoClient = new MongoClient(connectionString);
    
    // Extract database name from connection string, default to "authenticationdb"
    var mongoUrl = new MongoUrl(connectionString);
    var databaseName = mongoUrl.DatabaseName ?? "authenticationdb";
    
    return mongoClient.GetDatabase(databaseName);
});

builder.Services.AddScoped<IRepository<UserAccount>>(sp =>
    new MongoRepository<UserAccount>(sp.GetRequiredService<IMongoDatabase>(), "UserAccounts"));

builder.Services.AddScoped<IRepository<OtpCode>>(sp =>
    new MongoRepository<OtpCode>(sp.GetRequiredService<IMongoDatabase>(), "OtpCodes"));

builder.Services.AddScoped<AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FDA Authentication Service V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

// Configure pipeline
app.UseCors();

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
