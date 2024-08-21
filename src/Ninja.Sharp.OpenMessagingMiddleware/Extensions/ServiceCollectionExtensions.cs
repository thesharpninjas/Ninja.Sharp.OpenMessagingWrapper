using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ninja.Sharp.OpenMessagingMiddleware.Factory;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Ninja.Sharp.OpenMessagingMiddleware.Model.Configuration;
using Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ;
using Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka;

namespace Ninja.Sharp.OpenMessagingMiddleware.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            services.AddScoped<IMessageProducerFactory, MessageProducerFactory>();
            return services;
        }

        public static IMessagingBuilder AddArtemisServices(this IServiceCollection services, ArtemisConfig config)
        {
            services.AddCommonServices();
            return new ArtemisMqBuilder(services, config);
        }

        public static IMessagingBuilder AddArtemisServices(this IServiceCollection services, IConfiguration config)
        {
            var settings = config.GetSection("Messaging:Artemis").Get<ArtemisConfig>();
            return settings == null ? throw new ArgumentException("Artemis configuration not found") : services.AddArtemisServices(settings);
        }

        public static IMessagingBuilder AddKafkaServices(this IServiceCollection services, KafkaConfig config)
        {
            services.AddCommonServices();
            return new KafkaBuilder(services, config);
        }

        public static IMessagingBuilder AddKafkaServices(this IServiceCollection services, IConfiguration config)
        {
            var settings = config.GetSection("Messaging:Kafka").Get<KafkaConfig>();
            return settings == null ? throw new ArgumentException("Kafka configuration not found") : services.AddKafkaServices(settings);
        }
    }
}