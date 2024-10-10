using ActiveMQ.Artemis.Client;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Ninja.Sharp.OpenMessagingWrapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ
{
    internal class ArtemisBackgroundConsumer(
        IConsumer artemisConsumer,
        IMessageConsumer consumer,
        bool acceptIfError) : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
