using Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka.Enums;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka.Configuration
{
    public class KafkaConfig
    {
        public ICollection<KafkaServer> BootstrapServers { get; set; } = [];
        public int? ReceiveMessageMaxBytes { get; set; }
        public KafkaSecurityProtocol? SecurityProtocol { get; set; }
        public string GroupId { get; set; } = string.Empty;
        public KafkaAutoOffsetReset? AutoOffsetReset { get; set; }
        public bool? EnableAutoOffsetStore { get; set; }
        public bool? EnableAutoCommit { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public KafkaSaslMechanism SaslMechanism { get; set; }
        public string Identifier { get; set; } = string.Empty;

        public class KafkaServer
        {
            public int Port { get; set; } = 0;
            public string Host { get; set; } = string.Empty;
        }
    }
}
