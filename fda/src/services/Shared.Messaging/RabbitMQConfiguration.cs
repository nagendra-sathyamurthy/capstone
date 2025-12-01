namespace Shared.Messaging;

/// <summary>
/// Configuration for RabbitMQ connection
/// </summary>
public class RabbitMQConfiguration
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "food-delivery-exchange";
    
    /// <summary>
    /// Queue names for different services
    /// </summary>
    public static class Queues
    {
        public const string OrderService = "order-service-queue";
        public const string PaymentService = "payment-service-queue";
        public const string CartService = "cart-service-queue";
        public const string CRMService = "crm-service-queue";
        public const string CatalogService = "catalog-service-queue";
    }
}
