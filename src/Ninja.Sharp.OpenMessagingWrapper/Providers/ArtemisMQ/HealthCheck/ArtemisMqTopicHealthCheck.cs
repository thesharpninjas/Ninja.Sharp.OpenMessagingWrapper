using Apache.NMS;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ.Configuration;
using System.Diagnostics;

namespace Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ.HealthCheck
{
    internal class ArtemisMqTopicHealthCheck(ArtemisConfig configuration, string topic) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            Stopwatch stopwatch = new();
            double maxMilliseconds = 0.0D;
            foreach (ArtemisEndpoint endpoint in configuration.Endpoints)
            {
                Uri connecturi = new($"activemq:tcp://{endpoint.Host}:{endpoint.Port}");
                try
                {
                    stopwatch.Start();

                    NMSConnectionFactory factory = new(connecturi);
                    using Apache.NMS.ActiveMQ.Connection connection = (await factory.CreateConnectionAsync(endpoint.Username, endpoint.Password) as Apache.NMS.ActiveMQ.Connection)!;
                    connection.WatchTopicAdvisories = false;
                    using ISession session = await connection.CreateSessionAsync();
                    await connection.StartAsync();
                    using IQueue queue = await session.GetQueueAsync(topic);

                    var t1 = session.CreateBrowserAsync(queue);
                    var t2 = Task.Delay(5000, cancellationToken);
                    var result = Task.WhenAny(t1, t2);

                    if (result == t2)
                    {
                        return new HealthCheckResult(HealthStatus.Degraded, $"Connected, but with heavy delay.");
                    }

                    stopwatch.Stop();
                    maxMilliseconds = stopwatch.ElapsedMilliseconds > maxMilliseconds ? stopwatch.ElapsedMilliseconds : maxMilliseconds;
                }
                catch (Exception)
                {
                    return new HealthCheckResult(HealthStatus.Unhealthy, $"Error while connectiong to {endpoint.Host}.");
                }
            }
            if (maxMilliseconds > 5000)
            {
                return new HealthCheckResult(HealthStatus.Degraded, $"Connected, but with heavy delay.");
            }
            return new HealthCheckResult(HealthStatus.Healthy, "Working!");
        }
    }
}
