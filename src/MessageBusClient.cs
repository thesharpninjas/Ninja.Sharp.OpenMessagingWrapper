using Microsoft.Extensions.DependencyInjection;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Ninja.Sharp.OpenMessagingMiddleware.Utilities;

namespace Ninja.Sharp.OpenMessagingMiddleware
{
    public class MessageBusClient
    {
        private readonly IMessageProducer _producer;
        private readonly IMessageConsumer _consumer;

        public MessageBusClient(
            IServiceProvider serviceProvider, 
            bool isProducerEnabled, 
            bool isConsumerEnabled)
        {
            var options = new MessageBusOptions
            {
                IsProducerEnabled = isProducerEnabled,
                IsConsumerEnabled = isConsumerEnabled
            };

            var messageBus = serviceProvider.GetRequiredService<IMessageBus>();

            _producer = isProducerEnabled ? messageBus.Producer : null;
            _consumer = isConsumerEnabled ? messageBus.Consumer : null;
        }

        public async Task SendMessageAsync(string topic, string message)
        {
            if (_producer == null)
            {
                throw new InvalidOperationException("Producer is not enabled.");
            }

            await _producer.SendAsync(topic, message);
        }

        public async Task StartConsumingAsync(string topic, Func<string, Task> onMessageReceived)
        {
            if (_consumer == null)
            {
                throw new InvalidOperationException("Consumer is not enabled.");
            }

            await _consumer.StartAsync(topic, onMessageReceived);
        }
    }
}
