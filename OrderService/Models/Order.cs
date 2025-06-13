using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    // Представлява една поръчка в системата
    public class Order
    {
        [Key] // Уникален идентификатор за поръчката (първичен ключ)
        public int Id { get; set; }

        [Required] // Потребителят, който е направил поръчката
        public int UserId { get; set; }

        [Required] // Дата на създаване на поръчката
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Статус на поръчката (напр. Pending, Completed, Cancelled)
        [Required]
        public string Status { get; set; } = "Pending";

        // Списък с продукти към поръчката (навигиращо свойство – ще го направим по-късно)
        public List<OrderItem> Items { get; set; } = new();
    }
}
