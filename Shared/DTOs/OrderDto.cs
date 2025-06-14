namespace Shared.DTOs
{
    // Това е DTO-то, което ще връщаме към клиента
    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
