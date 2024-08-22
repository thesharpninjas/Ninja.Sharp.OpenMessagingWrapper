using Ninja.Sharp.OpenMessagingWrapper.Model;
using Ninja.Sharp.OpenMessagingWrapper.Model.Enums;

namespace Ninja.Sharp.OpenMessagingWrapper.Interfaces
{
    /// <summary>
    /// When you need to consume a message, you need to implement this interface
    /// </summary>
    public interface IMessageConsumer
    {
        /// <summary>
        /// Whenever a message is sent to the specified topic, this method will be triggered, providing basic information about the message delivered
        /// </summary>
        /// <param name="message">The delivered message</param>
        /// <returns></returns>
        Task<MessageAction> ConsumeAsync(IncomingMessage message);
    }

    
}
