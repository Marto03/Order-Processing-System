using OrderService.Models;

namespace OrderService.Services
{
    // Интерфейс за бизнес логика – може да включва валидации, съобщения и др.
    public interface IOrdersService
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task<Order> CreateAsync(Order order);
        Task<bool> DeleteAsync(int id);
    }
}
