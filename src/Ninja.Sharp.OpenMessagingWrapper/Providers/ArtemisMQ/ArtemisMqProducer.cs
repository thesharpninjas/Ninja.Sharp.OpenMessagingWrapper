using Ninja.Sharp.OpenMessagingWrapper.Interfaces;
using Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ.Configuration;

namespace Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ
{
    internal class ArtemisMqProducer(ArtemisMqMessageProducer producer, string topic, ArtemisConfig artemisConfig) : IMessageProducer
    {
        private readonly ArtemisMqMessageProducer producer = producer;
        private readonly string topic = topic;
        private readonly ArtemisConfig artemisConfig = artemisConfig;

        public async Task<string> SendAsync(string message)
        {
            try
            {
                string identifier = artemisConfig.Identifier;
                return await producer.PublishAsync(message, topic, identifier);
            }
            catch (Exception ex)
            {
                throw new TaskCanceledException($"Error while sending message: {ex.Message}", ex);
            }
        }

        public string Topic => topic;
    }
}
