namespace Messaging.Events;

/// <summary>
/// Event raised when a cart should be cleared (usually after successful order placement)
/// </summary>
public class CartClearEvent : IEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = nameof(CartClearEvent);
    
    public string CartId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string Reason { get; set; } = "OrderPlaced";
}
