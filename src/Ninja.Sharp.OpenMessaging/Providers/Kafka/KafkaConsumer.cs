using Confluent.Kafka;
using Ninja.Sharp.OpenMessaging.Interfaces;
using Ninja.Sharp.OpenMessaging.Model;

namespace Ninja.Sharp.OpenMessaging.Providers.Kafka
{
    public class KafkaConsumer : IMessageConsumer
    {
        private readonly IConsumer<Ignore, string> _consumer;

        public KafkaConsumer(string brokerUri)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = brokerUri,
                GroupId = "message-bus-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        public async Task StartAsync(string topic, Func<string, Task> onMessageReceived)
        {
            _consumer.Subscribe(topic);

            while (true)
            {
                var cr = _consumer.Consume();
                await onMessageReceived(cr.Message.Value);
            }
        }

        public Task ConsumeAsync(MqMessage message)
        {
            throw new NotImplementedException();
        }

        ~KafkaConsumer()
        {
            _consumer.Close();
            _consumer.Dispose();
        }
    }
}
