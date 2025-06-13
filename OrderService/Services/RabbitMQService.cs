using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderService.Services
{
    // Сървис за изпращане на съобщения в RabbitMQ
    public class RabbitMQService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName;
        private readonly string _routingKey;

        public RabbitMQService(IConfiguration configuration)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };

            _exchangeName = configuration["RabbitMQ:ExchangeName"];
            _routingKey = configuration["RabbitMQ:RoutingKey"];

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Деклариране на exchange тип "direct"
            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct, durable: true);
        }

        public void PublishOrderCreated(object orderDto)
        {
            var message = JsonSerializer.Serialize(orderDto);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true; // Запазваме съобщението дори ако RabbitMQ се рестартира

            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: _routingKey,
                basicProperties: properties,
                body: body);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
