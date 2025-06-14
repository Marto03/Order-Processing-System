using OrderService.Models;
using OrderService.Repositories;
using Shared.DTOs;

namespace OrderService.Services
{
    // Имплементация на бизнес логиката чрез репозиторито
    public class OrdersService : IOrdersService
    {
        private readonly HttpClient _httpClient;
        private readonly IOrderRepository _repository;
        private readonly RabbitMQService _rabbitMqService;
        private readonly string _userServiceBaseUrl;

        public OrdersService(HttpClient httpClient, IOrderRepository repository, RabbitMQService rabbitMqService, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _repository = repository;
            _rabbitMqService = rabbitMqService;
            _userServiceBaseUrl = configuration["Services:UserService"];
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
            var userResponse = await _httpClient.GetAsync($"{_userServiceBaseUrl}/api/users/{order.UserId}");
            if (!userResponse.IsSuccessStatusCode)
            {
                throw new Exception("User not found");
            }

            var userDto = await userResponse.Content.ReadFromJsonAsync<UserDTO>();
            if (userDto == null)
            {
                throw new Exception("Could not deserialize UserDto");
            }

            var createdOrder = await _repository.CreateOrderAsync(order);
            var orderDto = new OrderDto
            {
                Id = order.Id,
                CustomerName = userDto.UserName,
                TotalAmount = order.TotalAmount,
                CreatedAt = DateTime.UtcNow,
                
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
