using Microsoft.Extensions.Configuration;
using Ninja.Sharp.OpenMessaging.Factory;
using Ninja.Sharp.OpenMessaging.Interfaces;
using Ninja.Sharp.OpenMessaging.Providers.ArtemisMQ;
using Ninja.Sharp.OpenMessaging.Providers.ArtemisMQ.Configuration;
using Ninja.Sharp.OpenMessaging.Providers.Kafka;
using Ninja.Sharp.OpenMessaging.Providers.Kafka.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaServiceCollectionExtensions
    {
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
    }
}