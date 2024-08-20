using ActiveMQ.Artemis.Client;
using Amqp;
using Ninja.Sharp.OpenMessagingMiddleware.Exceptions;
using Ninja.Sharp.OpenMessagingMiddleware.Extensions;
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
                string caa = string.Empty; // TODO
                string msgId = Guid.NewGuid().ToString();
                await producer.PublishAsync(message, topic, $"{caa}.{msgId}");
                return msgId;
            }
            catch (Exception ex)
            {
                throw new TransientBrokerException($"Non è stato possibile inviare il messaggio alla coda: {ex.Message}", ex);
            }
        }

        public string Topic => topic;   
    }
}
