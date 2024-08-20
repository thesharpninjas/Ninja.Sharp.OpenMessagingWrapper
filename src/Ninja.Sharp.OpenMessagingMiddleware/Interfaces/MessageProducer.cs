using Ninja.Sharp.OpenMessagingMiddleware.Extensions;

namespace Ninja.Sharp.OpenMessagingMiddleware.Interfaces
{
    public interface IMessageProducerFactory
    {
        MessageProducer Producer(string name = "");
    }

    internal class MessageProducerFactory : IMessageProducerFactory
    {
        private readonly ICollection<MessageProducer> producers;

        public MessageProducerFactory(ICollection<MessageProducer> producers)
        {
            this.producers = producers;
        }

        public MessageProducer Producer(string name = "")
        {
            if (producers.Count == 0)
            {
                throw new Exception();
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return producers.First();
            }

            var producer = producers.FirstOrDefault(p => p.Name == name);
            if(producer == null)
            {
                throw new Exception();
            }
            return producer;
        }
    }

    public abstract class MessageProducer
    {
        public abstract Task SendAsync(string topic, string message);
        public abstract string Name { get; }

        public async Task SendAsync<T>(string topic, T message)
        {
            string serializedData = message.Serialize();

            await SendAsync(topic, serializedData);
        }
    }
}
