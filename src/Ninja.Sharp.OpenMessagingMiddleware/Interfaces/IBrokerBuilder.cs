using Microsoft.Extensions.DependencyInjection;
using Ninja.Sharp.OpenMessagingMiddleware.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingMiddleware.Interfaces
{
    public interface IBrokerBuilder
    {
        IBrokerBuilder AddConsumer<TConsumer>(string topic, MessagingType type = MessagingType.Queue, bool acceptIfInError = true) where TConsumer : IMessageConsumer;
        IBrokerBuilder AddProducer(string topic);
    }
}
