using Microsoft.Extensions.Configuration;
using Ninja.Sharp.OpenMessagingWrapper.Factory;
using Ninja.Sharp.OpenMessagingWrapper.Interfaces;
using Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ;
using Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ.Configuration;
using Ninja.Sharp.OpenMessagingWrapper.Providers.Kafka;
using Ninja.Sharp.OpenMessagingWrapper.Providers.Kafka.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ArtemisServiceCollectionExtensions
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
    }
}