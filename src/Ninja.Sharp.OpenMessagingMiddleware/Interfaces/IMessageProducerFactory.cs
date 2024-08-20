using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingMiddleware.Interfaces
{
    public interface IMessageProducerFactory
    {
        IMessageProducer Producer(string name = "");
    }
}
