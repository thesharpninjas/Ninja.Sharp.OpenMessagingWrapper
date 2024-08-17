using Confluent.Kafka;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka
{
    public class KafkaProducer : IMessageProducer
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(string brokerUri)
        {
            var config = new ProducerConfig { BootstrapServers = brokerUri };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task SendAsync(string topic, string message)
        {
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
        }

        ~KafkaProducer()
        {
            _producer.Dispose();
        }
    }
}
