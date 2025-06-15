namespace Infrastructure.Messaging.Interfaces
{
    public interface IMessageBusPublisher
    {
        Task PublishAsync<T>(T message, string topicOrRoutingKey);
    }
}
