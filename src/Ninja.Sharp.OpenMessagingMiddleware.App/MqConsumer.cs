using Amqp;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ
{
    public class MqConsumer : IMessageConsumer
    {
        public Task ConsumeAsync(Model.Message message)
        {
            throw new NotImplementedException();
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
