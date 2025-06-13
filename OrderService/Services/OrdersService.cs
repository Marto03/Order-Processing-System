using OrderService.Models;
using OrderService.Repositories;

namespace OrderService.Services
{
    // Имплементация на бизнес логиката чрез репозиторито
    public class OrdersService : IOrdersService
    {
        private readonly IOrderRepository _repository;

        public OrdersService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _repository.GetAllOrdersAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _repository.GetOrderByIdAsync(id);
        }

        public async Task<Order> CreateAsync(Order order)
        {
            // Тук можеш да добавиш валидации, логика, изпращане на съобщения
            return await _repository.CreateOrderAsync(order);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteOrderAsync(id);
        }
    }
}
