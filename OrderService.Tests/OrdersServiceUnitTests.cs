using RichardSzalay.MockHttp;
using Xunit;
using OrderService.Services;
using OrderService.Repositories;
using Shared.DTOs;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Infrastructure.Messaging.Interfaces;

public class OrdersServiceUnitTests
{
    [Fact]
    public async Task CreateAsync_Should_CreateOrder_And_PublishMessage()
    {
        // Arrange
        var order = new OrderService.Models.Order
        {
            UserId = 1,
            TotalAmount = 100
        };

        var mockHttp = new MockHttpMessageHandler();

        mockHttp
            .When("http://userservice/api/users/1")
            .Respond("application/json", "{\"userName\":\"TestUser\"}");

        var httpClient = new HttpClient(mockHttp)
        {
            BaseAddress = new Uri("http://userservice")
        };

        var orderRepoMock = new Mock<IOrderRepository>();
        orderRepoMock.Setup(x => x.CreateOrderAsync(It.IsAny<OrderService.Models.Order>()))
                     .ReturnsAsync(order);

        var publisherMock = new Mock<IMessageBusPublisher>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Services:UserService", "http://userservice" }
            })
            .Build();

        var service = new OrdersService(httpClient, orderRepoMock.Object, publisherMock.Object, config, new LogService());

        // Act
        var result = await service.CreateAsync(order);

        // Assert
        Assert.NotNull(result);
        orderRepoMock.Verify(r => r.CreateOrderAsync(order), Times.Once);
        publisherMock.Verify(p => p.PublishAsync(It.IsAny<OrderDto>(), "order.created"), Times.Once);
    }
    [Fact]
    public async Task CreateAsync_Should_ThrowException_When_UserApiFails()
    {
        // Arrange
        var order = new OrderService.Models.Order { UserId = 1, TotalAmount = 50 };

        var mockHttp = new MockHttpMessageHandler();

        mockHttp
            .When("http://userservice/api/users/1")
            .Respond(HttpStatusCode.NotFound);

        var httpClient = new HttpClient(mockHttp)
        {
            BaseAddress = new Uri("http://userservice")
        };

        var repoMock = new Mock<IOrderRepository>();
        var publisherMock = new Mock<IMessageBusPublisher>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
            { "Services:UserService", "http://userservice" }
            })
            .Build();

        var service = new OrdersService(httpClient, repoMock.Object, publisherMock.Object, config, new LogService());

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.CreateAsync(order));
    }
    [Fact]
    public async Task CreateAsync_Should_StillCreateOrder_If_PublishingFails()
    {
        // Arrange
        var order = new OrderService.Models.Order { UserId = 1, TotalAmount = 50 };

        var mockHttp = new MockHttpMessageHandler();

        mockHttp
            .When("http://userservice/api/users/1")
            .Respond("application/json", "{\"userName\":\"TestUser\"}");

        var httpClient = new HttpClient(mockHttp)
        {
            BaseAddress = new Uri("http://userservice")
        };

        var repoMock = new Mock<IOrderRepository>();
        repoMock.Setup(x => x.CreateOrderAsync(It.IsAny<OrderService.Models.Order>()))
                .ReturnsAsync(order);

        var publisherMock = new Mock<IMessageBusPublisher>();
        publisherMock.Setup(p => p.PublishAsync(It.IsAny<OrderDto>(), It.IsAny<string>()))
                     .ThrowsAsync(new Exception("Kafka down"));

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
            { "Services:UserService", "http://userservice" }
            })
            .Build();

        var service = new OrdersService(httpClient, repoMock.Object, publisherMock.Object, config, new LogService());

        // Act
        var result = await service.CreateAsync(order);

        // Assert
        Assert.Equal(order.UserId, result.UserId);
        repoMock.Verify(x => x.CreateOrderAsync(order), Times.Once);
        publisherMock.Verify(p => p.PublishAsync(It.IsAny<OrderDto>(), "order.created"), Times.Once);
    }
}
