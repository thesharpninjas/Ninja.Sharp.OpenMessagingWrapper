using ActiveMQ.Artemis.Client;
using Ninja.Sharp.OpenMessagingWrapper.Interfaces;
using Ninja.Sharp.OpenMessagingWrapper.Model.Enums;

namespace Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ
{
    internal class ArtemisMqProducer(IArtemisMqClient client, string address, Channel type, string identifier)
        : ArtemisMqPublisher, IMessageProducer
    {
        private IAnonymousProducer? producer;

        public async Task<string> SendAsync(string message)
        {
            try
            {
                if (producer is null || !client.IsConnected)
                    producer = await client.CreateAnonymousProducer();

                return await PublishAsync(producer, message, address, type, identifier);
            }
            catch (Exception ex)
            {
                throw new TaskCanceledException($"Error while sending message: {ex.Message}", ex);
            }
        }

        public string Topic => address;
    }
}
