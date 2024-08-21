using Microsoft.Extensions.Logging;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Ninja.Sharp.OpenMessagingMiddleware.Model;

namespace Ninja.Sharp.OpenMessagingMiddleware.Api
{
    public class LoggerMqConsumer(ILogger<LoggerMqConsumer> logger) : IMessageConsumer
    {
        public Task ConsumeAsync(MqMessage message)
        {
            logger.LogWarning("Message consumed: {0}", message.Body);
            return Task.CompletedTask;
        }
    }
}
