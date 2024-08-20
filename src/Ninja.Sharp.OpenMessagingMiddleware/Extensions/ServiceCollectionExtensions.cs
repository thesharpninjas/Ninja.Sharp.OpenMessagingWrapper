using Microsoft.Extensions.DependencyInjection;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ;
using Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka;
using Ninja.Sharp.OpenMessagingMiddleware.Providers;
using ActiveMQ.Artemis.Client;
using Microsoft.Extensions.Configuration;
using Confluent.Kafka;
using Ninja.Sharp.OpenMessagingMiddleware.Model.Configuration;

namespace Ninja.Sharp.OpenMessagingMiddleware.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IBrokerBuilder AddKafkaServices(this IServiceCollection services, KafkaConfig config)
        {
            return null;
        }

        public static IBrokerBuilder AddArtemisServices(this IServiceCollection services, ArtemisConfig config)
        {
            return new ArtemisMQBuilder(services, config);
        }
    }
}
