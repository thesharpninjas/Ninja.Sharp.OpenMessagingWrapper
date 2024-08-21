using Microsoft.Extensions.Configuration;
using Ninja.Sharp.OpenMessagingMiddleware.Factory;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ;
using Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ.Configuration;
using Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka;
using Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Allows building the environment for ArtemisMQ, reading data from ArtemisConfig
        /// </summary>
        public static IMessagingBuilder AddArtemisServices(this IServiceCollection services, ArtemisConfig config)
        {
            services.AddCommonServices();
            return new ArtemisMqBuilder(services, config);
        }

        /// <summary>
        /// Allows building the environment for ArtemisMQ, reading data from IConfiguration
        /// </summary>
        public static IMessagingBuilder AddArtemisServices(this IServiceCollection services, IConfiguration config)
        {
            var settings = config.GetSection("Messaging:Artemis").Get<ArtemisConfig>();
            return settings == null ? throw new ArgumentException("Artemis configuration not found") : services.AddArtemisServices(settings);
        }

        /// <summary>
        /// Allows building the environment for Kafka, reading data from KafkaConfig
        /// </summary>
        public static IMessagingBuilder AddKafkaServices(this IServiceCollection services, KafkaConfig config)
        {
            services.AddCommonServices();
            return new KafkaBuilder(services, config);
        }

        /// <summary>
        /// Allows building the environment for Kafka, reading data from IConfiguration
        /// </summary>
        public static IMessagingBuilder AddKafkaServices(this IServiceCollection services, IConfiguration config)
        {
            var settings = config.GetSection("Messaging:Kafka").Get<KafkaConfig>();
            return settings == null ? throw new ArgumentException("Kafka configuration not found") : services.AddKafkaServices(settings);
        }

        private static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            services.AddScoped<IMessageProducerFactory, MessageProducerFactory>();
            return services;
        }

        internal static IServiceCollection AddProducer<IMessageProducer>(this IServiceCollection services, string topic, Func<IServiceProvider, IMessageProducer> implementationFactory) where IMessageProducer : class
        {
            if (services.Any(x => x.ServiceKey?.ToString() == topic))
            {
                throw new ArgumentException($"Producer for topic {topic} already exists");
            }

            services.AddScoped(implementationFactory);
            services.AddKeyedScoped(topic, (x, y) => implementationFactory.Invoke(x));
            return services;
        }
    }
}