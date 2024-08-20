using Confluent.Kafka;
using Ninja.Sharp.OpenMessagingMiddleware.Extensions;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka
{
    public class KafkaProducer(IProducer<string, string> producer) : IMessageProducer
    {
        private readonly IProducer<string, string> producer = producer;

        public async Task SendAsync(string topic, string message)
        {
            string msgId = Guid.NewGuid().ToString();
            await producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = msgId,
                Value = message
            });
        }

        public async Task SendAsync<T>(string topic, T message)
        {
            string serializedData = message.Serialize();

            await SendAsync(topic, serializedData);
        }
    }
}
