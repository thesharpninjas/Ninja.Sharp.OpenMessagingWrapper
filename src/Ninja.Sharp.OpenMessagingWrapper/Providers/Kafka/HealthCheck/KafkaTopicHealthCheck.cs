using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Ninja.Sharp.OpenMessagingWrapper.Providers.Kafka.Configuration;
using System.Diagnostics;

namespace Ninja.Sharp.OpenMessagingWrapper.Providers.Kafka.HealthCheck
{
    internal class KafkaTopicHealthCheck(KafkaConfig configuration, string topic) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            Stopwatch stopwatch = new();
            double maxMilliseconds = 0.0D;
            string bootstrapServers = string.Join(',', configuration.BootstrapServers.Select(s => $"{s.Host}:{s.Port}"));

            try
            {
                stopwatch.Start();
                AdminClientConfig adminClientConfig = new()
                {
                    BootstrapServers = bootstrapServers,
                    ClientId = $"healthcheck_{topic}",
                    ReceiveMessageMaxBytes = configuration.ReceiveMessageMaxBytes ?? int.MaxValue,
                    SecurityProtocol = (SecurityProtocol?)configuration.SecurityProtocol ?? SecurityProtocol.Plaintext,
                };
                if (adminClientConfig.SecurityProtocol == SecurityProtocol.SaslSsl)
                {
                    adminClientConfig.SaslUsername = configuration.UserName;
                    adminClientConfig.SaslPassword = configuration.Password;
                    adminClientConfig.SaslMechanism = (SaslMechanism)configuration.SaslMechanism;
                }
                AdminClientBuilder adminClientBuilder = new(adminClientConfig);
                IAdminClient adminClient = adminClientBuilder.Build();

                DescribeTopicsResult describeTopicsResult = await adminClient.DescribeTopicsAsync(TopicCollection.OfTopicNames([topic]), new DescribeTopicsOptions { RequestTimeout = TimeSpan.FromSeconds(10) });
                List<TopicDescription> descriptions = describeTopicsResult.TopicDescriptions;
                if (descriptions.Count != 1)
                {
                    return new HealthCheckResult(HealthStatus.Unhealthy, $"La lettura dei dati del topic Kafka {topic} non ha restituito dati.");
                }
                if (descriptions.Single().Error.Code != ErrorCode.NoError)
                {
                    Error error = descriptions.Single().Error;
                    return new HealthCheckResult(HealthStatus.Unhealthy, $"La lettura dei dati del topic Kafka {topic} ha restituito un errore di tipo {error.Code} e con descrizione \"{error.Reason}\".");
                }
            }
            catch (Exception ex)
            {
                var message = new
                {
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    inner = ex.InnerException?.Message ?? string.Empty,
                    uri = bootstrapServers,
                    //user = configuration.UserName,
                    //pass = configuration.Password
                };
                return new HealthCheckResult(HealthStatus.Unhealthy, $"Si sono verificati errori durante la connessione ai server {bootstrapServers}. {message.Serialize()}");
            }
            if (maxMilliseconds > 5000)
            {
                return new HealthCheckResult(HealthStatus.Degraded, $"Connessione verso Kafka funzionante, ma con tempi di connessione elevati.");
            }
            return new HealthCheckResult(HealthStatus.Healthy, "Connessione verso Kafka funzionante.");

        }
    }
}
