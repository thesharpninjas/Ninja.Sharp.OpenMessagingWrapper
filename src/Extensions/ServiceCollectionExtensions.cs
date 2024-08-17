using Microsoft.Extensions.DependencyInjection;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ;
using Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka;
using Ninja.Sharp.OpenMessagingMiddleware.Providers;

namespace Ninja.Sharp.OpenMessagingMiddleware.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageBus(
            this IServiceCollection services,
            string brokerUriOrList,
            MessageBusProvider implementation,
            Action<MessageBusOptions> configureOptions)
        {
            var options = new MessageBusOptions();
            configureOptions(options);

            switch (implementation)
            {
                case MessageBusProvider.Kafka:
                    services.AddSingleton<IMessageBus>(sp => new KafkaMessageBus(brokerUriOrList, options));
                    break;

                case MessageBusProvider.ArtemisMQ:
                    services.AddSingleton<IMessageBus>(sp => new ArtemisMqMessageBus(brokerUriOrList, options));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(implementation), implementation, null);
            }

            return services;
        }
    }
}
