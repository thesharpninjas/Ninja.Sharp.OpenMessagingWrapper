using ActiveMQ.Artemis.Client;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ
{
    internal class ArtemisMqMessageProducer(IAnonymousProducer producer)
    {
        private readonly IAnonymousProducer _producer = producer;

        public async Task PublishAsync(string message, string queue, string qmsMessageId = "")
        {
            var address = queue;
            var msg = new Message(message);
            var msgId = Guid.NewGuid().ToString();
            if(!string.IsNullOrWhiteSpace(qmsMessageId))
            {
                msg.ApplicationProperties["QMSMessageId"] = qmsMessageId;
            }            
            msg.SetMessageId(msgId);
            msg.SetCorrelationId(msgId);
            await _producer.SendAsync(address, msg);
        }
    }
}
