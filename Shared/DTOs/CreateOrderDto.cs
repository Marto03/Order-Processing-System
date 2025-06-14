namespace Shared.DTOs
{
    // Това е DTO-то, което клиентът изпраща при POST заявка
    public class CreateOrderDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
