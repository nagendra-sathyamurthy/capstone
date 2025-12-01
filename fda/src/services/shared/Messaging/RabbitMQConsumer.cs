using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Messaging;

public class RabbitMQConsumer : IMessageConsumer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchangeName;
    private EventingBasicConsumer? _consumer;

    public RabbitMQConsumer(string hostName, string exchangeName, int port = 5672, string userName = "guest", string password = "guest")
    {
        _exchangeName = exchangeName;
        
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        // Declare exchange
        _channel.ExchangeDeclare(
            exchange: _exchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false
        );
    }

    public void StartConsuming<TEvent>(string queueName, Func<TEvent, Task> handler) where TEvent : class
    {
        try
        {
            // Declare queue
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );
            
            // Bind queue to exchange with routing key pattern
            var routingKey = typeof(TEvent).Name.ToLower();
            _channel.QueueBind(
                queue: queueName,
                exchange: _exchangeName,
                routingKey: routingKey
            );
            
            // Set QoS (prefetch count)
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var @event = JsonSerializer.Deserialize<TEvent>(message);
                    
                    if (@event != null)
                    {
                        await handler(@event);
                        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to deserialize message to {typeof(TEvent).Name}");
                        _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    // Negative acknowledgment - message will be requeued or sent to dead letter
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };
            
            _channel.BasicConsume(
                queue: queueName,
                autoAck: false,
                consumer: _consumer
            );
            
            Console.WriteLine($"Started consuming from queue: {queueName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting consumer: {ex.Message}");
            throw;
        }
    }

    public void StopConsuming()
    {
        _consumer = null;
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
