using Confluent.Kafka;
using Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Messaging.Kafka
{
    public class KafkaProducerService : IMessageBusPublisher, IDisposable
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly string _topic;

        public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
        {
            _logger = logger;
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                MessageTimeoutMs = 10000, // 10 секунди
                Acks = Acks.All           // изисква потвърждение от всички брокери
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
            _topic = configuration["Kafka:Topic"];
        }

        public async Task PublishAsync<T>(T message, string topicOrRoutingKey)
        {
            try
            {
                var result = await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = JsonSerializer.Serialize(message) });

                _logger.LogInformation("Message sent to Kafka topic {Topic}, partition {Partition}, offset {Offset}", result.Topic, result.Partition, result.Offset);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to Kafka");
                throw;
            }
        }

        public void Dispose()
        {
            _producer?.Flush(TimeSpan.FromSeconds(5));
            _producer?.Dispose();
        }

    }
}
