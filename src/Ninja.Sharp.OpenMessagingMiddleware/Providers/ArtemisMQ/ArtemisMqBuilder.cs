using ActiveMQ.Artemis.Client;
using ActiveMQ.Artemis.Client.AutoRecovering.RecoveryPolicy;
using ActiveMQ.Artemis.Client.Extensions.DependencyInjection;
using Amqp.Framing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Ninja.Sharp.OpenMessagingMiddleware.Model;
using Ninja.Sharp.OpenMessagingMiddleware.Model.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ
{


    internal class ArtemisMQBuilder : IBrokerBuilder
    {
        private readonly static int _retryCount = 2;
        private readonly static int _retryWaitTimeMs = 500;
        private readonly IServiceCollection services;
        private readonly IActiveMqBuilder activeMqBuilder;

        public ArtemisMQBuilder(IServiceCollection services, ArtemisConfig config)
        {
            if (!config.Endpoints.Any())
            {
                throw new Exception();
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

            /// Capire come valorizzare una cosa che è solo per inps
            var caa = "artemis";
            var id = caa + "_" + Guid.NewGuid().ToString();

            activeMqBuilder = services.AddActiveMq(id, endpoints);
            activeMqBuilder = activeMqBuilder.ConfigureConnectionFactory((a, b) => ConfigureFactory(a, b, id));
            activeMqBuilder = activeMqBuilder.AddAnonymousProducer<ArtemisMqMessageProducer>();
            this.services = services;

            // Anonymous Producer. Può essere creato solo uno per i services, il che impedisce l'uso di Artemis multipli
        }



        public IBrokerBuilder AddConsumer<TConsumer>(string topic, MessagingType type = MessagingType.Queue, bool acceptIfInError = true) where TConsumer : IMessageConsumer
        {
            services.AddScoped<TConsumer, IMessageConsumer>();
            activeMqBuilder.AddConsumer(topic,
                   type == MessagingType.Queue ? RoutingType.Anycast : RoutingType.Multicast,
                   async (message, consumer, serviceProvider, _) => await ConsumerHandlerAsync(message, consumer, serviceProvider, topic, typeof(TConsumer), acceptIfInError));
            return this;
        }

        internal static async Task ConsumerHandlerAsync(ActiveMQ.Artemis.Client.Message message, IConsumer consumer, IServiceProvider serviceProvider, string queue, Type type, bool acceptIfInError)
        {
            bool inError = false;
            try
            {
                string messageString = message.GetBody<string>();

                var selectedConsumer = serviceProvider.GetRequiredService(type) as IMessageConsumer;

                await selectedConsumer!.ConsumeAsync(new Model.Message()
                {
                    Body = messageString,
                    Id = message.MessageId,
                    GroupId = message.GroupId,
                });
            }
            catch (Exception ex)
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

        public IBrokerBuilder AddProducer(string topic)
        {
            services.AddScoped(x => new ArtemisMqProducer(x.GetRequiredService<ArtemisMqMessageProducer>(), topic));
            return this;
        }

        #region static
        internal static void ConfigureFactory(IServiceProvider serviceProvider, ConnectionFactory factory, string? id)
        {
            factory.AutomaticRecoveryEnabled = true;
            factory.ClientIdFactory = () => id;
            factory.RecoveryPolicy = RecoveryPolicyFactory.LinearBackoff(
                    initialDelay: TimeSpan.FromMilliseconds(_retryWaitTimeMs),
                    retryCount: _retryCount,
                    fastFirst: true);
            factory.LoggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        }
        #endregion
    }
}
