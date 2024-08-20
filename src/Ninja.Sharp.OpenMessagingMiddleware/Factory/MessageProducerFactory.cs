using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;

namespace Ninja.Sharp.OpenMessagingMiddleware.Factory
{
    internal class MessageProducerFactory(IEnumerable<IMessageProducer> producers) : IMessageProducerFactory
    {
        private readonly List<IMessageProducer> myProducers = producers.ToList();

        public IMessageProducer Producer(string topic = "")
        {
            if (myProducers.Count == 0)
            {
                throw new ArgumentException("No producer were registered");
            }

            if (string.IsNullOrWhiteSpace(topic))
            {
                return myProducers[0];
            }

            var producer = myProducers.Find(p => p.Topic == topic);
            if (producer == null)
            {
                throw new ArgumentException("No producer were registered for topic " + topic);
            }
            return producer;
        }
    }
}
