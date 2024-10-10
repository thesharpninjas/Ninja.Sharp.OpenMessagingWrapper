using ActiveMQ.Artemis.Client;
using ActiveMQ.Artemis.Client.AutoRecovering.RecoveryPolicy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ninja.Sharp.OpenMessagingWrapper.Interfaces;
using Ninja.Sharp.OpenMessagingWrapper.Model;
using Ninja.Sharp.OpenMessagingWrapper.Model.Enums;
using Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ.Configuration;
using Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ.HealthCheck;
using System.Net;

namespace Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ
{
    internal class ArtemisMqBuilder : IMessagingBuilder
    {
        private readonly ArtemisConfig config;
        private readonly IServiceCollection services;
        private readonly IHealthChecksBuilder? healthBuilder;
        private readonly ICollection<string> topics = [];

        public ArtemisMqBuilder(IServiceCollection services, ArtemisConfig config)
        {
            if (config.Endpoints.Count == 0)
            {
                throw new ArgumentException("Endpoints not provided in Artemis configuration.");
            }

            if (services.Any(x => x.ServiceType == typeof(ArtemisConfig)))
            {
                throw new ArgumentException("You cannot add more than one Artemis service.");
            }

            this.services = services;
            this.config = config;

            services.AddSingleton(config);

            if (config.HealthChecks)
            {
                healthBuilder = services.AddHealthChecks();
            }
        }

        public IMessagingBuilder AddConsumer<TConsumer>(string topic, string subscriber = "", Channel type = Channel.Queue, bool acceptIfInError = true) where TConsumer : class, IMessageConsumer
        {
            services.AddScoped<IMessageConsumer, TConsumer>();
            services.AddScoped<TConsumer>();
            topics.Add(topic);

            //services.AddHostedService(serviceProvider => new ArtemisBackgroundConsumer(
            //    serviceProvider.TryGetRequiredService<ActiveMqConsumer>(),
            //    serviceProvider.TryGetRequiredService<TConsumer>()
            //));

            //await CreateConnection();

            //activeMqBuilder.AddConsumer(topic,
            //        type == Channel.Queue ? RoutingType.Anycast : RoutingType.Multicast,
            //        async (message, consumer, serviceProvider, _) =>
            //        {
            //            await ConsumerHandlerAsync(message, consumer, serviceProvider, typeof(TConsumer), acceptIfInError);
            //        });

            return this;
        }

        public IMessagingBuilder AddProducer(string topic, Channel type = Channel.Queue)
        {
            services.AddProducer<IMessageProducer>(topic, serviceProvider =>
                new ArtemisMqProducer(
                    serviceProvider.GetRequiredService<IArtemisMqClient>(),
                    topic,
                    type,
                    config.Identifier));

            topics.Add(topic);

            return this;
        }

        public IServiceCollection Build()
        {
            if (config.HealthChecks && healthBuilder != null)
            {
                string[] tags = ["Artemis"];
                healthBuilder.AddCheck("ArtemisMq", new ArtemisMqConnectionHealthCheck(), tags: tags);
                foreach (var topic in topics.Distinct())
                {
                    healthBuilder.AddCheck("Artemis connection for topic " + topic, new ArtemisMqTopicHealthCheck(config, topic), tags: tags);
                }
            }
            return services;
        }

        internal static async Task ConsumerHandlerAsync(Message message, IConsumer consumer, IServiceProvider serviceProvider, Type type, bool acceptIfInError)
        {
            MessageAction result = MessageAction.Complete;
            try
            {
                string messageString = message.GetBody<string>();

                var selectedConsumer = serviceProvider.TryGetRequiredService(type) as IMessageConsumer;

                result = await selectedConsumer!.ConsumeAsync(new IncomingMessage()
                {
                    Body = messageString,
                    Id = message.MessageId,
                    GroupId = message.GroupId,
                });
            }
            catch (Exception)
            {
                result = MessageAction.Requeue;
            }
            finally
            {
                switch (result)
                {
                    case MessageAction.Requeue:
                        break;
                    case MessageAction.Reject:
                        consumer.Reject(message);
                        break;
                    case MessageAction.Error:
                        if (acceptIfInError)
                        {
                            await consumer.AcceptAsync(message);
                        }
                        break;
                    default:
                        await consumer.AcceptAsync(message);
                        break;
                }
            }
        }
    }
}
