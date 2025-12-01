using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Shared.Messaging;

public class RabbitMQPublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchangeName;

    public RabbitMQPublisher(string hostName, string exchangeName, int port = 5672, string userName = "guest", string password = "guest")
    {
        _exchangeName = exchangeName;
        
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        // Declare exchange (topic exchange for routing flexibility)
        _channel.ExchangeDeclare(
            exchange: _exchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false
        );
    }

    public Task PublishAsync<TEvent>(TEvent @event, string? routingKey = null) where TEvent : class
    {
        try
        {
            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);
            
            // Use event type as routing key if not provided
            var key = routingKey ?? typeof(TEvent).Name.ToLower();
            
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.Type = typeof(TEvent).Name;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            
            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: key,
                basicProperties: properties,
                body: body
            );
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing message: {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
