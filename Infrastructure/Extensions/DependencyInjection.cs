using Infrastructure.Messaging.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Messaging.Kafka;

namespace Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IMessageBusPublisher, KafkaProducerService>();
            //services.AddSingleton<IMessageBusPublisher, RabbitMQPublisher>();
            return services;
        }
    }
}
