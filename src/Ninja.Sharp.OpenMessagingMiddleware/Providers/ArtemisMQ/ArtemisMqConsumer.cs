using Amqp;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ
{
    public class ArtemisMqConsumer : IMessageConsumer
    {
        public ArtemisMqConsumer(string brokerUri)
        {
            // TODO
        }

        public async Task StartAsync(string topic, Func<string, Task> onMessageReceived)
        {
            // TODO
            await Task.CompletedTask;
        }
    }
}
