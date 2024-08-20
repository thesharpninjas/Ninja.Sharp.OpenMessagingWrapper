using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingMiddleware.Factory
{
    internal class MessageProducerFactory(IEnumerable<IMessageProducer> producers) : IMessageProducerFactory
    {
        private readonly List<IMessageProducer> myProducers = producers.ToList();

        public IMessageProducer Producer(string topic = "")
        {
            if (myProducers.Count == 0)
            {
                throw new Exception();
            }

            if (string.IsNullOrWhiteSpace(topic))
            {
                return myProducers[0];
            }

            var producer = myProducers.Find(p => p.Topic == topic);
            if (producer == null)
            {
                throw new Exception();
            }
            return producer;
        }
    }
}
