
using Authentication.DataAccess;
using Authentication.Models;
using Authentication.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB setup
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING"));
    return mongoClient.GetDatabase("AuthenticationDb");
});
builder.Services.AddScoped<IRepository<UserAccount>>(sp =>
    new MongoRepository<UserAccount>(sp.GetRequiredService<IMongoDatabase>(), "UserAccounts"));
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
