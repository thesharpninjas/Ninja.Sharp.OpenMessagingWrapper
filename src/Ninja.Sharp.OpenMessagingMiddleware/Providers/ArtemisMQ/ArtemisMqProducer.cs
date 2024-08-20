using ActiveMQ.Artemis.Client;
using Amqp;
using Ninja.Sharp.OpenMessagingMiddleware.Exceptions;
using Ninja.Sharp.OpenMessagingMiddleware.Extensions;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ
{
    internal class ArtemisMqProducer(ArtemisMqMessageProducer producer, string name) : MessageProducer
    {
        private readonly ArtemisMqMessageProducer producer = producer;

        public override string Name { get; } = name;

        public override async Task SendAsync(string topic, string message)
        {
            try
            {
                string caa = string.Empty; // TODO
                string msgId = Guid.NewGuid().ToString();
                await producer.PublishAsync(message, topic, $"{caa}.{msgId}");
            }
            catch (Exception ex)
            {
                throw new TransientBrokerException($"Non è stato possibile inviare il messaggio alla coda: {ex.Message}", ex);
            }
        }
    }
}
