using Confluent.Kafka;
using Infrastructure.Messaging.Kafka;
using Shared.Messaging;

namespace OrderService.Services
{
    public class KafkaService : IMessageBusPublisher, IDisposable
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaService> _logger;
        private readonly string _topic;

        public KafkaService(IConfiguration configuration, ILogger<KafkaService> logger)
        {
            _logger = logger;
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
            _topic = configuration["Kafka:Topic"];
        }

        public async Task PublishAsync<T>(T message, string topicOrRoutingKey)
        {
            try
            {
                var result = await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = message.ToString() });
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

        public void Publish<T>(T message, string routingKey)
        {
            throw new NotImplementedException();
        }
    }
}
