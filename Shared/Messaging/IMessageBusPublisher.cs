namespace Shared.Messaging
{
    public interface IMessageBusPublisher
    {
        void Publish<T>(T message, string routingKey);
    }
}
