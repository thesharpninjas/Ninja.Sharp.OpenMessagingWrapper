using Confluent.Kafka;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka
{
    public class KafkaProducer(IProducer<string, string> producer, string topic) : IMessageProducer
    {
        private readonly IProducer<string, string> producer = producer;
        private readonly string topic = topic;

        public string Topic => topic;

        public async Task<string> SendAsync(string message)
        {
            //TODO aggiungere il caa al message id, come per artemis
            string msgId = Guid.NewGuid().ToString();
            await producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = msgId,
                Value = message
            });
            return msgId;
        }
    }
}
