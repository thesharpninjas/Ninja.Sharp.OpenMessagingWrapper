using Ninja.Sharp.OpenMessagingMiddleware.Exceptions;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ
{
    internal class ArtemisMqProducer(ArtemisMqMessageProducer producer, string topic) : IMessageProducer
    {
        private readonly ArtemisMqMessageProducer producer = producer;
        private readonly string topic = topic;

        public async Task<string> SendAsync(string message)
        {
            try
            {
                string identifier = string.Empty; // TODO
                return await producer.PublishAsync(message, topic, identifier);
            }
            catch (Exception ex)
            {
                throw new TransientBrokerException($"Non è stato possibile inviare il messaggio alla coda: {ex.Message}", ex);
            }
        }

        public string Topic => topic;
    }
}
