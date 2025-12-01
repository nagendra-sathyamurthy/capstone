using Messaging;
using Messaging.Events;

namespace Cart.API.BackgroundServices;

public class CartMessageConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CartMessageConsumerService> _logger;
    private readonly IConfiguration _configuration;
    private IMessageConsumer? _consumer;

    public CartMessageConsumerService(
        IServiceProvider serviceProvider,
        ILogger<CartMessageConsumerService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cart Message Consumer Service starting...");

        try
        {
            var rabbitMQHost = _configuration["RABBITMQ_HOST"] ?? "localhost";
            var rabbitMQPort = int.Parse(_configuration["RABBITMQ_PORT"] ?? "5672");
            var rabbitMQUser = _configuration["RABBITMQ_USER"] ?? "guest";
            var rabbitMQPassword = _configuration["RABBITMQ_PASSWORD"] ?? "guest";
            var exchangeName = _configuration["RABBITMQ_EXCHANGE"] ?? "food-delivery-exchange";

            _consumer = new RabbitMQConsumer(rabbitMQHost, exchangeName, rabbitMQPort, rabbitMQUser, rabbitMQPassword);

            // Listen for PaymentCompleted events to clear cart
            _consumer.StartConsuming<PaymentCompletedEvent>(
                RabbitMQConfiguration.Queues.CartService,
                HandlePaymentCompletedEvent
            );

            _logger.LogInformation("Cart Message Consumer Service started successfully");

            // Keep the service running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting Cart Message Consumer Service");
        }
    }

    private async Task HandlePaymentCompletedEvent(PaymentCompletedEvent @event)
    {
        _logger.LogInformation($"Received PaymentCompletedEvent for Order: {@event.OrderId}, Customer: {@event.CustomerId}");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var cartService = scope.ServiceProvider.GetRequiredService<CartService>();

            // Clear the customer's cart after successful payment
            await cartService.ClearCartByUserIdAsync(@event.CustomerId);

            _logger.LogInformation($"Successfully cleared cart for customer: {@event.CustomerId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error clearing cart for customer {@event.CustomerId}: {ex.Message}");
            throw; // Re-throw to trigger message requeue
        }
    }

    public override void Dispose()
    {
        if (_consumer is IDisposable disposableConsumer)
        {
            disposableConsumer.Dispose();
        }
        base.Dispose();
    }
}
