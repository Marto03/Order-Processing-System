using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using NotificationService.Data;
using System.Text;
using Infrastructure.Models;
using NotificationService.Repositories;
using System.Threading.Channels;

namespace NotificationService.Services
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMqConsumerService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IConfiguration _configuration;

        public RabbitMqConsumerService(IServiceProvider serviceProvider, ILogger<RabbitMqConsumerService> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = configuration["RabbitMQ:HostName"],
                    UserName = configuration["RabbitMQ:UserName"],
                    Password = configuration["RabbitMQ:Password"]
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                var _queueName = configuration["RabbitMQ:QueueName"];
                var exchangeName = configuration["RabbitMQ:Exchange"];
                var routingKey = configuration["RabbitMQ:RoutingKey"];

                _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true);
                _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                _channel.QueueBind(queue: _queueName, exchange: exchangeName, routingKey: routingKey);

                _logger.LogInformation($"✅ RabbitMQ connected. Exchange: {exchangeName}, Queue: {_queueName}, RoutingKey: {routingKey}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to initialize RabbitMqConsumerService");
                throw;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var repo = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<INotificationRepository>();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var rabbitMessage = new RabbitMqMessage
                {
                    ReceivedAt = DateTime.UtcNow,
                    MessageContent = message
                };
                await repo.SaveRabbitMqMessageAsync(message, stoppingToken);

                _logger.LogInformation("RabbitMQ message saved in DB: {Message}", message);

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: _configuration["RabbitMQ:QueueName"], autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
