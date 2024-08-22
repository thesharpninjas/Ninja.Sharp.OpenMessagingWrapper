using ActiveMQ.Artemis.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ.HealthCheck
{
    internal class ArtemisMqConnectionHealthCheck() : IHealthCheck
    {
        private static bool IsRunning = true;
        private static string HealthDescription = DESCRIPTION_OK;
        private const string DESCRIPTION_OK = "Working!";
        private const string DESCRIPTION_CONNECTIONCLOSED = "Connection has been closed";
        private const string DESCRIPTION_CONNECTIONRECOVERYERROR = "Cannot recover from lost connection";

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HealthCheckResult result = new(IsRunning ? HealthStatus.Healthy : HealthStatus.Unhealthy, description: HealthDescription);

            return Task.FromResult(result);
        }

        public static void C_ConnectionRecovered(object? sender, ConnectionRecoveredEventArgs e)
        {
            IsRunning = true;
            HealthDescription = DESCRIPTION_OK;
        }

        public static void C_ConnectionRecoveryError(object? sender, ConnectionRecoveryErrorEventArgs e)
        {
            IsRunning = false;
            HealthDescription = DESCRIPTION_CONNECTIONRECOVERYERROR;
        }

        public static void C_ConnectionClosed(object? sender, ConnectionClosedEventArgs e)
        {
            IsRunning = false;
            HealthDescription = DESCRIPTION_CONNECTIONCLOSED;
        }
    }
}
