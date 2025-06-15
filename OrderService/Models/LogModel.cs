namespace OrderService.Models
{
    public class LogModel
    {
        public int UserId { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
        public string Level { get; set; } = "Info"; // Info, Warning, Error
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
