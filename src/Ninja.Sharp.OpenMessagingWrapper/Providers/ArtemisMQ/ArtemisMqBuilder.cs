﻿using ActiveMQ.Artemis.Client;
using ActiveMQ.Artemis.Client.AutoRecovering.RecoveryPolicy;
using ActiveMQ.Artemis.Client.Extensions.DependencyInjection;
using ActiveMQ.Artemis.Client.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ninja.Sharp.OpenMessagingWrapper.Interfaces;
using Ninja.Sharp.OpenMessagingWrapper.Model;
using Ninja.Sharp.OpenMessagingWrapper.Model.Enums;
using Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ.Configuration;
using Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ.HealthCheck;

namespace Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ
{
    internal class ArtemisMqBuilder : IMessagingBuilder
    {
        private readonly ArtemisConfig config;
        private readonly IServiceCollection services;
        private readonly IActiveMqBuilder activeMqBuilder;
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

            var endpoints = new List<Endpoint>();
            foreach (var myEndpoint in config.Endpoints)
            {
                var endpoint = Endpoint.Create(
                   myEndpoint.Host,
                   myEndpoint.Port,
                   myEndpoint.Username,
                   myEndpoint.Password,
                   scheme: Scheme.Amqp
                );
                endpoints.Add(endpoint);
            }

            var identifier = config.Identifier;
            var id = identifier + "_" + Guid.NewGuid().ToString();
            id = id.Trim('_');

            activeMqBuilder = services.AddActiveMq(id, endpoints);
            activeMqBuilder = activeMqBuilder.ConfigureConnectionFactory((a, b) => ConfigureFactory(a, b, id, config));
            activeMqBuilder = activeMqBuilder.ConfigureConnection(ConfigureConnection);
            activeMqBuilder = activeMqBuilder.AddAnonymousProducer<ArtemisMqMessageProducer>();
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
            activeMqBuilder.AddConsumer(topic,
                   type == Channel.Queue ? RoutingType.Anycast : RoutingType.Multicast,
                   async (message, consumer, serviceProvider, _) => await ConsumerHandlerAsync(message, consumer, serviceProvider, typeof(TConsumer), acceptIfInError));
            return this;
        }

        public IMessagingBuilder AddProducer(string topic, Channel type = Channel.Queue)
        {
            services.AddProducer<IMessageProducer>(topic, (a) => new ArtemisMqProducer(a.GetRequiredService<ArtemisMqMessageProducer>(), topic, a.GetRequiredService<ArtemisConfig>()));
            topics.Add(topic);
            return this;
        }

        public IServiceCollection Build()
        {
            services.AddActiveMqHostedService();

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

        #region static

        internal static void ConfigureConnection(IServiceProvider _, IConnection connection)
        {
            connection.ConnectionClosed += ArtemisMqConnectionHealthCheck.C_ConnectionClosed;
            connection.ConnectionRecovered += ArtemisMqConnectionHealthCheck.C_ConnectionRecovered;
            connection.ConnectionRecoveryError += ArtemisMqConnectionHealthCheck.C_ConnectionRecoveryError;
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

        internal static void ConfigureFactory(IServiceProvider serviceProvider, ConnectionFactory factory, string? id, ArtemisConfig config)
        {
            factory.AutomaticRecoveryEnabled = true;
            factory.ClientIdFactory = () => id;
            factory.RecoveryPolicy = RecoveryPolicyFactory.LinearBackoff(
                    initialDelay: TimeSpan.FromMilliseconds(config.RetryWaitTime),
                    retryCount: config.Retries,
                    fastFirst: true);
            factory.LoggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        }
        #endregion
    }
}
