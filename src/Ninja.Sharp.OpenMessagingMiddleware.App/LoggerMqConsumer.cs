using Microsoft.Extensions.Logging;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Ninja.Sharp.OpenMessagingMiddleware.Model;

namespace Ninja.Sharp.OpenMessagingMiddleware.App
{
    public class LoggerMqConsumer(ILogger<LoggerMqConsumer> logger) : IMessageConsumer
    {
        public Task ConsumeAsync(MqMessage message)
        {
            logger.LogInformation("Message consumed. Id: {MessageId}, Body: {MessageBody}.", message.Id, message.Body);
            return Task.CompletedTask;
        }
    }
}
