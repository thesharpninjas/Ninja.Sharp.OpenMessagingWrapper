using Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ.Configuration;
using Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka.Configuration;

namespace Ninja.Sharp.OpenMessagingMiddleware.Model
{
    internal class Messaging
    {
        public ArtemisConfig Artemis { get; set; } = new();
        public KafkaConfig Kafka { get; set; } = new();
    }
}
