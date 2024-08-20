using ActiveMQ.Artemis.Client;
using Amqp;
using Ninja.Sharp.OpenMessagingMiddleware.Exceptions;
using Ninja.Sharp.OpenMessagingMiddleware.Extensions;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Message = ActiveMQ.Artemis.Client.Message;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ
{
    public class ArtemisMqProducer(IAnonymousProducer producer) : IMessageProducer
    {
        private readonly IAnonymousProducer producer = producer;

        public async Task SendAsync(string topic, string message)
        {
            try
            {
                string caa = string.Empty; // TODO
                double delay = 0.0D;    // TODO

                Message msg = new(message);
                string msgId = Guid.NewGuid().ToString();
                msg.ApplicationProperties["QMSMessageId"] = $"{caa}.{msgId}";
                msg.SetMessageId(msgId);
                msg.SetCorrelationId(msgId);
                msg.ScheduledDeliveryDelay = TimeSpan.FromSeconds(delay);
                await producer.SendAsync(topic, msg);
            }
            catch (Exception ex)
            {
                throw new TransientBrokerException($"Non è stato possibile inviare il messaggio alla coda: {ex.Message}", ex);
            }
        }

        public async Task SendAsync<T>(string topic, T message)
        {
            string serializedData = message.Serialize();

            await SendAsync(topic, serializedData);
        }
    }
}
