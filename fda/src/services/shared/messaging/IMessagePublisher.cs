namespace Messaging;

public interface IMessagePublisher
{
    /// <summary>
    /// Publish an event to the message broker
    /// </summary>
    Task PublishAsync<TEvent>(TEvent @event, string? routingKey = null) where TEvent : class;
}
