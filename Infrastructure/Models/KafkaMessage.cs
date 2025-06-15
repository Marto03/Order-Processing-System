namespace Infrastructure.Models
{
    public class KafkaMessage
    {
        public int Id { get; set; }
        public DateTime ReceivedAt { get; set; }
        public string MessageContent { get; set; } = null!;
    }
}
