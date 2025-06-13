using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    // Един ред от поръчка (един продукт и неговото количество)
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; } // Идентификатор на продукта

        [Required]
        public int Quantity { get; set; } // Количество

        [Required]
        public decimal Price { get; set; } // Цена за единица

        // Външен ключ към Order (на коя поръчка принадлежи този item)
        public int OrderId { get; set; }

        public Order? Order { get; set; } // Навигиращо свойство
    }
}
