namespace Shared.Messaging.Events;

/// <summary>
/// Event raised when an order is accepted by the restaurant
/// </summary>
public class OrderAcceptedEvent : IEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = nameof(OrderAcceptedEvent);
    
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string RestaurantId { get; set; } = string.Empty;
    public string AcceptedBy { get; set; } = string.Empty; // Staff member who accepted
    public DateTime? EstimatedPreparationTime { get; set; }
}

/// <summary>
/// Event raised when an order is declined by the restaurant
/// </summary>
public class OrderDeclinedEvent : IEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = nameof(OrderDeclinedEvent);
    
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string RestaurantId { get; set; } = string.Empty;
    public string DeclinedBy { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

/// <summary>
/// Event raised when an order status changes
/// </summary>
public class OrderStatusChangedEvent : IEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = nameof(OrderStatusChangedEvent);
    
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string RestaurantId { get; set; } = string.Empty;
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// Event raised when an order is ready for pickup
/// </summary>
public class OrderReadyForPickupEvent : IEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = nameof(OrderReadyForPickupEvent);
    
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string RestaurantId { get; set; } = string.Empty;
    public string RestaurantName { get; set; } = string.Empty;
    public string? PackagedBy { get; set; }
}

/// <summary>
/// Event raised when an order is delivered
/// </summary>
public class OrderDeliveredEvent : IEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = nameof(OrderDeliveredEvent);
    
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string RestaurantId { get; set; } = string.Empty;
    public string? DeliveryAgentId { get; set; }
    public DateTime DeliveryTime { get; set; }
}
