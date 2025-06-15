namespace Shared.DTOs
{
    // Това е DTO-то, което клиентът изпраща при POST заявка
    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
