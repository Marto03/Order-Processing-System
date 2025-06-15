using Infrastructure.Messaging.Interfaces;
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
        private readonly string _userServiceBaseUrl;
        private readonly IMessageBusPublisher _publisher;

        public OrdersService(HttpClient httpClient, IOrderRepository repository, IMessageBusPublisher publisher, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _repository = repository;
            _publisher = publisher;
            _userServiceBaseUrl = configuration["Services:UserService"];
        }


        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _repository.GetAllOrdersAsync();

            var orderDtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                string customerName = "Unknown";

                var response = await _httpClient.GetAsync($"{_userServiceBaseUrl}/api/users/{order.UserId}");
                if (response.IsSuccessStatusCode)
                {
                    var userDto = await response.Content.ReadFromJsonAsync<UserDTO>();
                    if (userDto != null)
                    {
                        customerName = userDto.UserName;
                    }
                }

                orderDtos.Add(new OrderDto
                {
                    Id = order.Id,
                    CustomerName = customerName,
                    TotalAmount = order.TotalAmount,
                    CreatedAt = order.CreatedAt
                });
            }

            return orderDtos;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _repository.GetOrderByIdAsync(id);
        }
        
        public async Task<Order?> UpdateAsync(int id, decimal totalAmount)
        {
            var order = await _repository.GetOrderByIdAsync(id);
            if (order == null) return null;
            order.TotalAmount = totalAmount;
            order.CreatedAt = DateTime.UtcNow;
            await _repository.UpdateOrderAsync(order);
            return order;
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
            await _publisher.PublishAsync(orderDto, "order.created");

            return createdOrder;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteOrderAsync(id);
        }
    }
}
