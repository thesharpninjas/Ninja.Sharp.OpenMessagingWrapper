using Microsoft.Extensions.Logging;
using Ninja.Sharp.OpenMessagingWrapper.Interfaces;
using Ninja.Sharp.OpenMessagingWrapper.Model;
using Ninja.Sharp.OpenMessagingWrapper.Model.Enums;

namespace Ninja.Sharp.OpenMessagingWrapper.App
{
    public class LoggerMqConsumer(ILogger<LoggerMqConsumer> logger) : IMessageConsumer
    {
        public Task<MessageAction> ConsumeAsync(IncomingMessage message)
        {
            logger.LogInformation("Message consumed. Id: {MessageId}, Body: {MessageBody}.", message.Id, message.Body);
            return Task.FromResult(MessageAction.Complete);
        }
    }
}
