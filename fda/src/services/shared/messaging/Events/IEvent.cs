namespace Messaging.Events;

/// <summary>
/// Base interface for all events in the system
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    string EventId { get; }
    
    /// <summary>
    /// Timestamp when the event was created
    /// </summary>
    DateTime Timestamp { get; }
    
    /// <summary>
    /// Type of the event
    /// </summary>
    string EventType { get; }
}
