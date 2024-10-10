using ActiveMQ.Artemis.Client;
using Ninja.Sharp.OpenMessagingWrapper.Model.Enums;

namespace Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ
{
    internal class ArtemisMqPublisher()
    {
        public static async Task<string> PublishAsync(
            IAnonymousProducer producer,
            string message,
            string address,
            Channel type,
            string identifier)
        {
            var msg = new Message(message);

            var msgId = identifier + "." + Guid.NewGuid().ToString().Trim('.');

            if (!string.IsNullOrWhiteSpace(identifier))
                msg.ApplicationProperties["QMSMessageId"] = msgId;

            msg.SetMessageId(msgId);
            msg.SetCorrelationId(msgId);

            await producer.SendAsync(address, type == Channel.Queue ? RoutingType.Anycast : RoutingType.Multicast, msg);

            return msgId;
        }
    }
}
