using ActiveMQ.Artemis.Client;
using ActiveMQ.Artemis.Client.AutoRecovering.RecoveryPolicy;
using ActiveMQ.Artemis.Client.Extensions.DependencyInjection;
using ActiveMQ.Artemis.Client.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Ninja.Sharp.OpenMessagingMiddleware.Model;
using Ninja.Sharp.OpenMessagingMiddleware.Model.Configuration;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ
{
    internal class ArtemisMqBuilder : IMessagingBuilder
    {
        private readonly ArtemisConfig config;
        private readonly IServiceCollection services;
        private readonly IActiveMqBuilder activeMqBuilder;
        private readonly IHealthChecksBuilder healthBuilder;
        private readonly ICollection<string> topics = [];

        public ArtemisMqBuilder(IServiceCollection services, ArtemisConfig config)
        {
            if (config.Endpoints.Count == 0)
            {
                throw new ArgumentException("Endpoints not provided in Artemis configuration.");
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
            activeMqBuilder = activeMqBuilder.AddAnonymousProducer<ArtemisMqMessageProducer>();
            this.services = services;
            this.config = config;

            healthBuilder = services.AddHealthChecks();
        }

        public IMessagingBuilder AddConsumer<TConsumer>(string topic, string subscriber = "", MessagingType type = MessagingType.Queue, bool acceptIfInError = true) where TConsumer : class, IMessageConsumer
        {
            services.AddScoped<IMessageConsumer, TConsumer>();
            services.AddScoped<TConsumer>();
            topics.Add(topic);
            activeMqBuilder.AddConsumer(topic,
                   type == MessagingType.Queue ? RoutingType.Anycast : RoutingType.Multicast,
                   async (message, consumer, serviceProvider, _) => await ConsumerHandlerAsync(message, consumer, serviceProvider, topic, typeof(TConsumer), acceptIfInError));
            return this;
        }

        public IMessagingBuilder AddProducer(string topic, MessagingType type = MessagingType.Queue)
        {
            topics.Add(topic);
            services.AddScoped<IMessageProducer>(x => new ArtemisMqProducer(x.GetRequiredService<ArtemisMqMessageProducer>(), topic));
            return this;
        }

        public IServiceCollection Build()
        {
            services.AddActiveMqHostedService();
            foreach(var topic in topics.Distinct())
            {
                healthBuilder.AddCheck("Artemis connection for " + topic, new ArtemisMqHealthCheck(config, topic), tags: ["Artemis"]);
            }
            return services;
        }

        #region static
        internal static async Task ConsumerHandlerAsync(Message message, IConsumer consumer, IServiceProvider serviceProvider, string queue, Type type, bool acceptIfInError)
        {
            bool inError = false;
            try
            {
                string messageString = message.GetBody<string>();

                var selectedConsumer = serviceProvider.GetRequiredService(type) as IMessageConsumer;

                await selectedConsumer!.ConsumeAsync(new MqMessage()
                {
                    Body = messageString,
                    Id = message.MessageId,
                    GroupId = message.GroupId,
                });
            }
            catch (Exception)
            {
                inError = true;
            }
            finally
            {
                if (!inError)
                {
                    await consumer.AcceptAsync(message);
                }
                else if (acceptIfInError)
                {
                    await consumer.AcceptAsync(message);
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
