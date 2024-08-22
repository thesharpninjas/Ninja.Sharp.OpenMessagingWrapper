using Confluent.Kafka;
using Ninja.Sharp.OpenMessaging.Interfaces;
using Ninja.Sharp.OpenMessaging.Providers.Kafka.Configuration;
using System.Text;

namespace Ninja.Sharp.OpenMessaging.Providers.Kafka
{
    public class KafkaProducer(IProducer<string, string> producer, string topic, KafkaConfig kafkaConfig) : IMessageProducer
    {
        private readonly IProducer<string, string> producer = producer;
        private readonly string topic = topic;

        public string Topic => topic;

        public async Task<string> SendAsync(string message)
        {
            CancellationToken cancellationToken = new CancellationTokenSource(5000).Token;
            string identifier = kafkaConfig.Identifier;
            string msgId = $"{identifier}.{Guid.NewGuid()}";
            msgId = msgId.Trim('.');
            Message<string, string> kafkaMessage = new()
            {
                Key = msgId,
                Value = message,
                Headers = []
            };
            if (!string.IsNullOrWhiteSpace(identifier))
            {
                kafkaMessage.Headers.Add("QMSMessageId", Encoding.UTF8.GetBytes(msgId));
            }
            await producer.ProduceAsync(topic, kafkaMessage, cancellationToken);
            return msgId;
        }
    }
}
