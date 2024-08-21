using Microsoft.Extensions.Configuration;
using Ninja.Sharp.OpenMessaging.Factory;
using Ninja.Sharp.OpenMessaging.Interfaces;
using Ninja.Sharp.OpenMessaging.Providers.ArtemisMQ;
using Ninja.Sharp.OpenMessaging.Providers.ArtemisMQ.Configuration;
using Ninja.Sharp.OpenMessaging.Providers.Kafka;
using Ninja.Sharp.OpenMessaging.Providers.Kafka.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddCommonServices(this IServiceCollection services)
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