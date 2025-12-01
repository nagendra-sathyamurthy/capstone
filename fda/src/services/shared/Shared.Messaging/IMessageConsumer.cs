namespace Shared.Messaging;

public interface IMessageConsumer
{
    /// <summary>
    /// Start consuming messages from a queue
    /// </summary>
    void StartConsuming<TEvent>(string queueName, Func<TEvent, Task> handler) where TEvent : class;
    
    /// <summary>
    /// Stop consuming messages
    /// </summary>
    void StopConsuming();
}
