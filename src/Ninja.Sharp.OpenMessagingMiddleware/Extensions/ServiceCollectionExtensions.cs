using Microsoft.Extensions.DependencyInjection;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ;
using Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka;
using Microsoft.Extensions.Configuration;
using Ninja.Sharp.OpenMessagingMiddleware.Model.Configuration;

namespace Ninja.Sharp.OpenMessagingMiddleware.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IMessagingBuilder AddArtemisServices(this IServiceCollection services, ArtemisConfig config)
        {
            return new ArtemisMqBuilder(services, config);
        }

        public static IMessagingBuilder AddArtemisServices(this IServiceCollection services, IConfiguration config)
        {
            var settings = config.GetSection("Messaging").Get<ArtemisConfig>();
            if (settings == null)
            {
                throw new ArgumentException("Artemis configuration not found");
            }

            return services.AddArtemisServices(settings);
        }
        public static IMessagingBuilder AddKafkaServices(this IServiceCollection services, KafkaConfig config)
        {
            return new KafkaBuilder(services, config);
        }

        public static IMessagingBuilder AddKafkaServices(this IServiceCollection services, IConfiguration config)
        {
            KafkaConfig? settings = config.GetSection("Messaging").Get<KafkaConfig>();
            return settings == null ? throw new ArgumentException("Kafka configuration not found") : services.AddKafkaServices(settings);
        }
    }
}
