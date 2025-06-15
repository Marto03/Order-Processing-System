namespace NotificationService.Repositories
{
    public interface INotificationRepository
    {
        Task SaveRabbitMqMessageAsync(string content, CancellationToken cancellationToken = default);
        Task SaveKafkaMessageAsync(string content, CancellationToken cancellationToken = default);
    }
}
