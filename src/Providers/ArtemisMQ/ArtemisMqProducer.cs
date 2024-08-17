using Amqp;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ
{
    public class ArtemisMqProducer : IMessageProducer
    {
        private readonly IConnection _connection;
        private readonly ISession _session;
        private readonly IMessageProducer _producer;

        public ArtemisMqProducer(string brokerUri)
        {
            // TODO
        }

        public async Task SendAsync(string topic, string message)
        {
            // TODO
            await Task.CompletedTask;
        }
    }
}
