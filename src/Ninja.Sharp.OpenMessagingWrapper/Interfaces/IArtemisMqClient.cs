using ActiveMQ.Artemis.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingWrapper.Interfaces
{
    internal interface IArtemisMqClient
    {
        bool IsConnected { get; }

        Task<IAnonymousProducer> CreateAnonymousProducer();
    }
}
