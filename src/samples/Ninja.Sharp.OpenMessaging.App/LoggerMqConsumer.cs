using Microsoft.Extensions.Logging;
using Ninja.Sharp.OpenMessaging.Interfaces;
using Ninja.Sharp.OpenMessaging.Model;

namespace Ninja.Sharp.OpenMessaging.App
{
    public class LoggerMqConsumer(ILogger<LoggerMqConsumer> logger) : IMessageConsumer
    {
        public Task ConsumeAsync(IncomingMessage message)
        {
            logger.LogInformation("Message consumed. Id: {MessageId}, Body: {MessageBody}.", message.Id, message.Body);
            return Task.CompletedTask;
        }
    }
}
