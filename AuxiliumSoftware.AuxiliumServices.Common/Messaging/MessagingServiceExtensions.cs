
using AuxiliumSoftware.AuxiliumServices.Common.Configuration.Sections.Databases;
using AuxiliumSoftware.AuxiliumServices.Common.Configuration.Sections.Databases.RabbitMQ;
using AuxiliumSoftware.AuxiliumServices.Common.Messaging.Interfaces;
using AuxiliumSoftware.AuxiliumServices.Common.Messaging.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuxiliumSoftware.AuxiliumServices.Common.Messaging
{
    public static class MessagingServiceExtensions
    {
        /// <summary>
        /// Registers shared RabbitMQ infrastructure (connection manager).
        /// Call this from both API and background task runner.
        /// Expects RabbitMQConfigurationSection to already be registered/validated.
        /// </summary>
        public static IServiceCollection AddRabbitMqCore(
            this IServiceCollection services,
            RabbitMQConfigurationSection configuration)
        {
            services.AddSingleton(configuration);
            services.AddSingleton<IRabbitMqConnectionManager, RabbitMqConnectionManager>();

            return services;
        }

        /// <summary>
        /// Registers the message queue producer. Call this from the API project.
        /// </summary>
        public static IServiceCollection AddRabbitMqProducer(this IServiceCollection services)
        {
            services.AddSingleton<IMessageQueueProducer, RabbitMqProducer>();
            return services;
        }

        /// <summary>
        /// Registers a consumer hosted service for a specific message type.
        /// Call this from the background task runner.
        /// Queue name is resolved from the Queues dictionary in config using the provided key.
        /// </summary>
        public static IServiceCollection AddRabbitMqConsumer<TMessage, THandler>(
            this IServiceCollection services,
            Func<RabbitMQQueuesConfigurationSection, string> queueSelector,
            string routingKey
        )
            where TMessage : QueueMessage
            where THandler : class, IMessageHandler<TMessage>
        {
            services.AddScoped<IMessageHandler<TMessage>, THandler>();

            services.AddHostedService(sp =>
            {
                var connectionManager = sp.GetRequiredService<IRabbitMqConnectionManager>();
                var queueName = queueSelector(connectionManager.Configuration.Queues);

                if (string.IsNullOrWhiteSpace(queueName))
                    throw new InvalidOperationException(
                        "Resolved queue name is empty - check the RabbitMQ Queues configuration.");

                return new RabbitMqConsumerService<TMessage>(
                    connectionManager,
                    sp.GetRequiredService<IServiceScopeFactory>(),
                    sp.GetRequiredService<ILogger<RabbitMqConsumerService<TMessage>>>(),
                    queueName,
                    routingKey
                );
            });

            return services;
        }
    }
}
