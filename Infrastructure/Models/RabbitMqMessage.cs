namespace Infrastructure.Models
{
    public class RabbitMqMessage
    {
        public int Id { get; set; }
        public DateTime ReceivedAt { get; set; }
        public string MessageContent { get; set; } = null!;
    }
}
