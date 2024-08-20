namespace Ninja.Sharp.OpenMessagingMiddleware.Model.Configuration
{
    public class ArtemisConfig
    {
        public string Identifier { get; set; } = string.Empty;
        public int Retries { get; set; } = 2; // Da mettere tramite options
        public int RetryWaitTime { get; set; } = 500;

        public ICollection<ArtemisEndpoint> Endpoints { get; set; } = [];

    }

    public class ArtemisEndpoint
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; } = 0;
        public string Host { get; set; } = string.Empty;
    }
}
