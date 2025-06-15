using Infrastructure.Models;
using NotificationService.Data;

namespace NotificationService.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task SaveRabbitMqMessageAsync(string content, CancellationToken cancellationToken = default)
        {
            _context.RabbitMqMessages.Add(new RabbitMqMessage
            {
                ReceivedAt = DateTime.UtcNow,
                MessageContent = content
            });

            await _context.SaveChangesAsync(cancellationToken);
        }
        
        public async Task SaveKafkaMessageAsync(string content, CancellationToken cancellationToken = default)
        {
            _context.KafkaMessages.Add(new KafkaMessage
            {
                ReceivedAt = DateTime.UtcNow,
                MessageContent = content
            });

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
