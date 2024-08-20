using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ
{
    public class ArtemisMqMessageBus : IMessageBus
    {
        public IMessageProducer Producer { get; private set; }
        public IMessageConsumer Consumer { get; private set; }

        public ArtemisMqMessageBus(string brokerUri, MessageBusOptions options)
        {
            if (options.IsProducerEnabled)
            {
                Producer = new ArtemisMqProducer(brokerUri);
            }

            if (options.IsConsumerEnabled)
            {
                Consumer = new ArtemisMqConsumer(brokerUri);
            }
        }
    }
}
