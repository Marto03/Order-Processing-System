using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Tests.Integration_Tests
{
    public class OrderRepositoryIntegrationTests
    {
        private OrderDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "TestOrdersDb")
                .Options;

            return new OrderDbContext(options);
        }
        private OrderDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new OrderDbContext(options);
        }
        [Fact]
        public async Task CreateOrderAsync_Should_AddOrder()
        {
            var context = GetInMemoryDbContext();
            var repo = new OrderRepository(context);

            var order = new Order
            {
                UserId = 1,
                TotalAmount = 200,
                CreatedAt = DateTime.UtcNow
            };

            var createdOrder = await repo.CreateOrderAsync(order);

            Assert.NotNull(createdOrder);
            Assert.Equal(1, await context.Orders.CountAsync());
        }
        [Fact]
        public async Task CreateOrderAsync_Should_SaveOrder()
        {
            // Arrange
            var context = GetDbContext();
            var repo = new OrderRepository(context);

            var order = new Order
            {
                UserId = 1,
                TotalAmount = 200,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = await repo.CreateOrderAsync(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.TotalAmount, result.TotalAmount);

            var savedOrder = await context.Orders.FindAsync(result.Id);
            Assert.NotNull(savedOrder);
        }
    }
}
