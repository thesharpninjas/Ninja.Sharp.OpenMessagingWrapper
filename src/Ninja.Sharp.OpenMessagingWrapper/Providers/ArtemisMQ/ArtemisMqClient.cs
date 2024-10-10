using ActiveMQ.Artemis.Client;
using ActiveMQ.Artemis.Client.AutoRecovering.RecoveryPolicy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ninja.Sharp.OpenMessagingWrapper.Interfaces;
using Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ.Configuration;
using Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ.HealthCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ
{
    internal class ArtemisMqClient(IServiceProvider serviceProvider, ArtemisConfig config) : IArtemisMqClient
    {
        private IConnection? _connection;
        public bool IsConnected => _connection?.IsOpened ?? false; 

        public async Task<IAnonymousProducer> CreateAnonymousProducer()
        {
            if (!IsConnected)
                await OpenConnection();

            return await _connection!.CreateAnonymousProducerAsync();
        }

        private async Task OpenConnection()
        {
            if (_connection != null && _connection.IsOpened)
                return;

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

            var connectionFactory = new ConnectionFactory
            {
                AutomaticRecoveryEnabled = true,
                ClientIdFactory = () => id,
                RecoveryPolicy = RecoveryPolicyFactory.LinearBackoff(
                    initialDelay: TimeSpan.FromMilliseconds(config.RetryWaitTime),
                    retryCount: config.Retries,
                    fastFirst: true),
                LoggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>()
            };

            _connection = await connectionFactory.CreateAsync(endpoints);

            _connection.ConnectionClosed += ArtemisMqConnectionHealthCheck.C_ConnectionClosed;
            _connection.ConnectionRecovered += ArtemisMqConnectionHealthCheck.C_ConnectionRecovered;
            _connection.ConnectionRecoveryError += ArtemisMqConnectionHealthCheck.C_ConnectionRecoveryError;
        }

        //public async Task<IConsumer> GetConsumer(string address, Channel type = Channel.Queue, string queueName = "")
        //{
        //    try
        //    {
        //        if (Connection is null || !Connection.IsOpened)
        //            await OpenConnection();

        //        return await Connection!.CreateConsumerAsync(new ConsumerConfiguration()
        //        {
        //            Address = address,
        //            RoutingType = type == Channel.Queue ? RoutingType.Anycast : RoutingType.Multicast,
        //            Queue = queueName
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new TaskCanceledException($"Error while opening consumer: {ex.Message}", ex);
        //    }
        //}
    }
}
