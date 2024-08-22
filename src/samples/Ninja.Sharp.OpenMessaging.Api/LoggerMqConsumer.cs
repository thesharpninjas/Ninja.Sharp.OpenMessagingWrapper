using Ninja.Sharp.OpenMessaging.Interfaces;
using Ninja.Sharp.OpenMessaging.Model;
using Ninja.Sharp.OpenMessaging.Model.Enums;

namespace Ninja.Sharp.OpenMessaging.Api
{
    public class LoggerMqConsumer(ILogger<LoggerMqConsumer> logger) : IMessageConsumer
    {
        public Task<MessageAction> ConsumeAsync(IncomingMessage message)
        {
            logger.LogWarning("Message consumed: {0}", message.Body);
            return Task.FromResult(MessageAction.Complete);
        }
    }
}
