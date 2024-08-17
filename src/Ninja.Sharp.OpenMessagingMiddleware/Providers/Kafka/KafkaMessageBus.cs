using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka
{
    public class KafkaMessageBus : IMessageBus
    {
        public IMessageProducer Producer { get; private set; }
        public IMessageConsumer Consumer { get; private set; }

        public KafkaMessageBus(string brokerUri, MessageBusOptions options)
        {
            if (options.IsProducerEnabled)
            {
                Producer = new KafkaProducer(brokerUri);
            }

            if (options.IsConsumerEnabled)
            {
                Consumer = new KafkaConsumer(brokerUri);
            }
        }
    }
}
