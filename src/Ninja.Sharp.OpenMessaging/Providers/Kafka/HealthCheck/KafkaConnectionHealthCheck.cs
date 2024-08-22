using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ninja.Sharp.OpenMessaging.Providers.Kafka.HealthCheck
{
    internal class KafkaConnectionHealthCheck() : IHealthCheck
    {
        private static bool IsRunning = true;
        private static string HealthDescription = DESCRIPTION_OK;
        private const string DESCRIPTION_OK = "Working!";
        private const string DESCRIPTION_CONNECTIONCLOSED = "Connection has been closed";

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HealthCheckResult result = new(IsRunning ? HealthStatus.Healthy : HealthStatus.Unhealthy, description: HealthDescription);

            return Task.FromResult(result);
        }

        public static void KafkaConsumerErrorHandler(IConsumer<string, string> consumer, Error error)
        {
            if (error.IsError)
            {
                IsRunning = false;
                HealthDescription = DESCRIPTION_CONNECTIONCLOSED;
            }
            else
            {
                IsRunning = true;
                HealthDescription = DESCRIPTION_OK;
            }
        }

        public static void KafkaProducerErrorHandler(IProducer<string, string> producer, Error error)
        {
            if (error.IsError)
            {
                IsRunning = false;
                HealthDescription = DESCRIPTION_CONNECTIONCLOSED;
            }
            else
            {
                IsRunning = true;
                HealthDescription = DESCRIPTION_OK;
            }
        }

    }
}
