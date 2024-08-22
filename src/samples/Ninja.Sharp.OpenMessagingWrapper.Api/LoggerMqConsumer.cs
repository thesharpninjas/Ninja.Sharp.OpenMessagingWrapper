using Ninja.Sharp.OpenMessagingWrapper.Interfaces;
using Ninja.Sharp.OpenMessagingWrapper.Model;
using Ninja.Sharp.OpenMessagingWrapper.Model.Enums;

namespace Ninja.Sharp.OpenMessagingWrapper.Api
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
