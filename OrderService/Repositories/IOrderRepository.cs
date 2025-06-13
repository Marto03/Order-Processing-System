using OrderService.Models;

namespace OrderService.Repositories
{
    // Интерфейсът дефинира какви операции поддържа хранилището (repository)
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int id);
    }
}
