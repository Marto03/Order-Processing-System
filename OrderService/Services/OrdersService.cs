using Infrastructure.Messaging.Interfaces;
using Nest;
using OrderService.Models;
using OrderService.Repositories;
using Shared.DTOs;
using System.Text.Json;

namespace OrderService.Services
{
    // Имплементация на бизнес логиката чрез репозиторито
    public class OrdersService : IOrdersService
    {
        private readonly HttpClient _httpClient;
        private readonly IOrderRepository _repository;
        private readonly string _userServiceBaseUrl;
        private readonly IMessageBusPublisher _publisher;
        private readonly LogService _logService;
        private readonly IRedisService _redis;

        public OrdersService(HttpClient httpClient, IOrderRepository repository, IMessageBusPublisher publisher, IConfiguration configuration,
            LogService logService, IRedisService redis)
        {
            _httpClient = httpClient;
            _repository = repository;
            _publisher = publisher;
            _userServiceBaseUrl = configuration["Services:UserService"];
            _logService = logService;
            _redis = redis;
        }


        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = (await _repository.GetAllOrdersAsync());

            // Get all unique userIds
            var userIds = orders.Select(o => o.UserId).Distinct();

            // Try to get all user names from Redis in parallel
            var redisTasks = userIds.ToDictionary(
                id => id,
                id => _redis.GetValueAsync($"user:{id}")
            );
            await Task.WhenAll(redisTasks.Values);

            // Find which userIds are missing from cache
            var missingUserIds = redisTasks
                .Where(kvp => string.IsNullOrEmpty(kvp.Value.Result))
                .Select(kvp => kvp.Key)
                .ToList();

            // Batch fetch missing users from User Service (if API supports it)
            var userNames = new Dictionary<int, string>();
            if (missingUserIds.Any())
            {
                // Example: POST /api/users/batch with { ids: [1,2,3] }
                var response = await _httpClient.PostAsJsonAsync(
                    $"{_userServiceBaseUrl}/api/users/batch",
                    missingUserIds
                );
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserDTO>>();
                    foreach (var user in users)
                    {
                        var cached = await _redis.GetValueAsync($"user:{user.Id}");
                        if (cached == null)
                            Console.WriteLine($"Cache miss for user:{user.Id}");

                        userNames[user.Id] = user.UserName;
                        // Set in Redis for future
                        await _redis.SetValueAsync($"user:{user.Id}", user.UserName);
                    }
                }
            }

            // Merge Redis and HTTP results
            foreach (var kvp in redisTasks)
            {
                if (!string.IsNullOrEmpty(kvp.Value.Result))
                    userNames[kvp.Key] = kvp.Value.Result;
            }

            // Build DTOs
            var orderDtos = orders.Select(order => new OrderDto
            {
                Id = order.Id,
                CustomerName = userNames.TryGetValue(order.UserId, out var name) ? name : "Unknown",
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt
            }).ToList();

            // Log once for the whole operation
            await _logService.LogAsync(new LogModel
            {
                UserId = 0,
                Action = "Get All Orders",
                Message = $"Successfully retrieved {orders.Count()} orders",
                Level = "Info"
            });

            return orderDtos;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            var cached = await _redis.GetValueAsync($"order:{id}");
            if (!string.IsNullOrEmpty(cached))
                return JsonSerializer.Deserialize<Order>(cached);

            var order = await _repository.GetOrderByIdAsync(id);
            if (order != null)
                await _redis.SetValueAsync($"order:{id}", JsonSerializer.Serialize(order));

            await _logService.LogAsync(new LogModel
            {
                UserId = id,
                Action = "Get Order",
                Message = "Successfully get order",
                Level = "Info"
            });
            return await _repository.GetOrderByIdAsync(id);
        }
        
        public async Task<Order?> UpdateAsync(int id, decimal totalAmount)
        {
            var order = await _repository.GetOrderByIdAsync(id);
            if (order == null) return null;
            order.TotalAmount = totalAmount;
            order.CreatedAt = DateTime.UtcNow;
            await _repository.UpdateOrderAsync(order);

            await _logService.LogAsync(new LogModel
            {
                UserId = order.UserId,
                Action = $"Updated Order -> Total Amount changed to: {totalAmount}",
                Message = "Successfully updated order",
                Level = "Info"
            });
            await _publisher.PublishAsync(order, "order.created");
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
            await _logService.LogAsync(new LogModel
            {
                UserId = order.UserId,
                Action = $"Created Order -> Order Id: {order.Id}",
                Message = "Successfully created order",
                Level = "Info"
            });
            await _publisher.PublishAsync(orderDto, "order.created");

            return createdOrder;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _logService.LogAsync(new LogModel
            {
                UserId = id,
                Action = $"Deleted Order",
                Message = "Successfully deleted order",
                Level = "Info"
            });
            return await _repository.DeleteOrderAsync(id);
        }
    }
}
