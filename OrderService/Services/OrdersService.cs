using OrderService.DTOs;
using OrderService.Models;
using OrderService.Repositories;

namespace OrderService.Services
{
    // Имплементация на бизнес логиката чрез репозиторито
    public class OrdersService : IOrdersService
    {
        private readonly IOrderRepository _repository;
        private readonly RabbitMQService _rabbitMqService;
        // Инициализация на HttpClient (най-добре през DI)
        private readonly HttpClient _httpClient;

        public OrdersService(IOrderRepository repository, RabbitMQService rabbitMqService, HttpClient httpClient)
        {
            _repository = repository;
            _rabbitMqService = rabbitMqService;
            _httpClient = httpClient;
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
            // Викаме UserService API, за да вземем User данни
            var userResponse = await _httpClient.GetAsync($"http://userservice/api/users/{order.UserId}");
            if (!userResponse.IsSuccessStatusCode)
            {
                throw new Exception("User not found");
            }

            var userDto = await userResponse.Content.ReadFromJsonAsync<UserDto>();

            // Създаваме OrderDto за RabbitMQ с потребителско име от UserService
            var orderDto = new OrderDto
            {
                Id = order.Id,
                CustomerName = userDto.UserName,  // взето от UserService
                TotalAmount = order.TotalAmount,
                CreatedAt = DateTime.UtcNow
            };

            _rabbitMqService.PublishOrderCreated(orderDto);

            return createdOrder;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteOrderAsync(id);
        }
    }
}
