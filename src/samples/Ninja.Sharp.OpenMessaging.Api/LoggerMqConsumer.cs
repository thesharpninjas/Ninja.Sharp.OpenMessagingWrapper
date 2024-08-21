using Ninja.Sharp.OpenMessaging.Interfaces;
using Ninja.Sharp.OpenMessaging.Model;

namespace Ninja.Sharp.OpenMessaging.Api
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
