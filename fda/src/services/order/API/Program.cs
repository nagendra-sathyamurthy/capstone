using MongoDB.Driver;
using Order.API;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
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

// Register MongoDB client
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration["MONGO_CONNECTION_STRING"] ?? "mongodb://localhost:27017";
    return new MongoClient(connectionString);
});

// Register services
builder.Services.AddScoped<OrderService>();

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
    
    // Delivery Agent policy
    options.AddPolicy("DeliveryAgent", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("role", "DeliveryAgent")));
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
