using OrderService.Models;
using Shared.DTOs;

namespace OrderService.Services
{
    // Интерфейс за бизнес логика – може да включва валидации, съобщения и др.
    public interface IOrdersService
    {
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> UpdateAsync(int id, decimal totalAmount);
        Task<Order> CreateAsync(Order order);
        Task<bool> DeleteAsync(int id);
    }
}
