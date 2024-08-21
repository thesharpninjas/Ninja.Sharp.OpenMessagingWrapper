using Ninja.Sharp.OpenMessaging.Model;

namespace Ninja.Sharp.OpenMessaging.Interfaces
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
        Task ConsumeAsync(IncomingMessage message);
    }
}
