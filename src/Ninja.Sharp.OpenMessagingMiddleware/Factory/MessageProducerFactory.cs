using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingMiddleware.Factory
{
    internal class MessageProducerFactory(ICollection<IMessageProducer> producers) : IMessageProducerFactory
    {
        private readonly ICollection<IMessageProducer> producers = producers;

        public IMessageProducer Producer(string topic = "")
        {
            if (producers.Count == 0)
            {
                throw new Exception();
            }

            if (string.IsNullOrWhiteSpace(topic))
            {
                return producers.First();
            }

            var producer = producers.FirstOrDefault(p => p.Topic == topic);
            if (producer == null)
            {
                throw new Exception();
            }
            return producer;
        }
    }
}
