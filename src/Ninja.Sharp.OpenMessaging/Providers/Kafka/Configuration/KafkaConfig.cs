using Ninja.Sharp.OpenMessaging.Providers.Kafka.Enums;

namespace Ninja.Sharp.OpenMessaging.Providers.Kafka.Configuration
{
    /// <summary>
    /// Configuration for Kafka
    /// </summary>
    public class KafkaConfig
    {
        /// <summary>
        /// Bootstrap server(s) (required)
        /// </summary>
        public ICollection<KafkaServer> BootstrapServers { get; set; } = [];
        /// <summary>
        /// Maximum bytes to receive (optional)
        /// </summary>
        public int? ReceiveMessageMaxBytes { get; set; }
        /// <summary>
        /// Security protocol for the connection (default: plaintext)
        /// </summary>
        public KafkaSecurityProtocol? SecurityProtocol { get; set; }
        /// <summary>
        /// Group ID for the consumer (optional)
        /// </summary>
        public string GroupId { get; set; } = string.Empty;
        /// <summary>
        /// Auto offset reset behavior (default: latest)
        /// </summary>
        public KafkaAutoOffsetReset? AutoOffsetReset { get; set; }
        /// <summary>
        /// Enable auto offset store (default: true)
        /// </summary>
        public bool? EnableAutoOffsetStore { get; set; }
        /// <summary>
        /// Enable auto commit (default: false)
        /// </summary>
        public bool? EnableAutoCommit { get; set; }
        /// <summary>
        /// Username for the connection (optional)
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// Password for the connection (optional)
        /// </summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// SASL mechanism (default: plain)
        /// </summary>
        public KafkaSaslMechanism? SaslMechanism { get; set; }
        /// <summary>
        /// Identifier (will be used as part of the Client ID for the producer and as part of the ID of any sent messages) (optional)
        /// </summary>
        public string Identifier { get; set; } = string.Empty;
        /// <summary>
        /// Enable health checks (default: true)
        /// </summary>
        public bool HealthChecks { get; set; } = true;

        /// <summary>
        /// Kafka bootstrap server
        /// </summary>
        public class KafkaServer
        {
            /// <summary>
            /// Port of the bootstrap server
            /// </summary>
            public int Port { get; set; } = 0;
            /// <summary>
            /// Hostname of the bootstrap server
            /// </summary>
            public string Host { get; set; } = string.Empty;
        }
    }
}
