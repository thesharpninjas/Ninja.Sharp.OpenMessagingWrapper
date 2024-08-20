using Amqp;
using Microsoft.Extensions.Logging;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ
{
    public class MqConsumer(ILogger<MqConsumer> logger) : IMessageConsumer
    {
        public Task ConsumeAsync(Model.Message message)
        {
            logger.LogWarning("Message consumed: {0}", message.Body);
            return Task.CompletedTask;
        }
    }

    public class AnotherMqConsumer : IMessageConsumer
    {
        public Task ConsumeAsync(Model.Message message)
        {
            throw new NotImplementedException();
        }
    }
}
