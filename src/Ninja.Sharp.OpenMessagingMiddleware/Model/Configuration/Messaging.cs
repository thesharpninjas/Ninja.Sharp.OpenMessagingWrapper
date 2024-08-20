namespace Ninja.Sharp.OpenMessagingMiddleware.Model.Configuration
{
    internal class Messaging
    {
        public ArtemisConfig Artemis { get; set; } = new();
        public KafkaConfig Kafka { get; set; } = new();
    }
}
