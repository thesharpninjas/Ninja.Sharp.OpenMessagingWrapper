namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ.Configuration
{
    /// <summary>
    /// Configuration for Artemis MQ
    /// </summary>
    public class ArtemisConfig
    {
        /// <summary>
        /// Identifier (will be used as part of the Client ID for the producer and as part of the ID of any sent messages) (optional)
        /// </summary>
        public string Identifier { get; set; } = string.Empty;
        /// <summary>
        /// Number of retries
        /// </summary>
        public int Retries { get; set; } = 2; // Da mettere tramite options
        /// <summary>
        /// Wait time between retries (in milliseconds)
        /// </summary>
        public int RetryWaitTime { get; set; } = 500;

        /// <summary>
        /// Endpoints
        /// </summary>
        public ICollection<ArtemisEndpoint> Endpoints { get; set; } = [];
        /// <summary>
        /// Enable health checks
        /// </summary>
        public bool HealthChecks { get; set; } = true;
    }

    public class ArtemisEndpoint
    {
        /// <summary>
        /// Username for the endpoint
        /// </summary>
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// Password for the endpoint
        /// </summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// Port of the endpoint
        /// </summary>
        public int Port { get; set; } = 0;
        /// <summary>
        /// Hostname of the endpoint
        /// </summary>
        public string Host { get; set; } = string.Empty;
    }
}
