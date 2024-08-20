using Ninja.Sharp.OpenMessagingMiddleware.Model.Enums;

namespace Ninja.Sharp.OpenMessagingMiddleware.Model.Configuration
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

        public class KafkaServer
        {
            public int Port { get; set; } = 0;
            public string Host { get; set; } = string.Empty;
        }
    }
}
