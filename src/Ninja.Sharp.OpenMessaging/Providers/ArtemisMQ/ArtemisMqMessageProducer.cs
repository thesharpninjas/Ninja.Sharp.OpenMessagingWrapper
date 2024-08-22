using ActiveMQ.Artemis.Client;

namespace Ninja.Sharp.OpenMessaging.Providers.ArtemisMQ
{
    internal class ArtemisMqMessageProducer(IAnonymousProducer producer)
    {
        private readonly IAnonymousProducer _producer = producer;

        public async Task<string> PublishAsync(string message, string queue, string identifier)
        {
            var address = queue;
            var msg = new Message(message);
            var msgId = identifier + "." + Guid.NewGuid().ToString();
            msgId = msgId.Trim('.');

            if (!string.IsNullOrWhiteSpace(identifier))
            {
                msg.ApplicationProperties["QMSMessageId"] = msgId;
            }
            msg.SetMessageId(msgId);
            msg.SetCorrelationId(msgId);
            await _producer.SendAsync(address, msg);
            return msgId;
        }
    }
}
