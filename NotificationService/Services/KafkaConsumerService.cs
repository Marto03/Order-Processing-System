using Confluent.Kafka;
using Infrastructure.Models;
using NotificationService.Data;
using NotificationService.Repositories;
using static System.Formats.Asn1.AsnWriter;

namespace NotificationService.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IConfiguration _configuration;

        public KafkaConsumerService(IServiceProvider serviceProvider, ILogger<KafkaConsumerService> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }
         
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = "notification-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("order.created");

            _logger.LogInformation("Kafka consumer started. Listening to topic: order-created");

            try
            {
                var repo = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<INotificationRepository>();

                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = consumer.Consume(stoppingToken);
                    var message = result.Message.Value;

                    // Запис в базата
                    var kafkaMessage = new KafkaMessage
                    {
                        ReceivedAt = DateTime.UtcNow,
                        MessageContent = message 
                    };
                    await repo.SaveKafkaMessageAsync(message, stoppingToken);

                    _logger.LogInformation("Kafka message saved in DB: {Message}", message);
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
        }
    }

}
