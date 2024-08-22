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
        Task<MessageAction> ConsumeAsync(IncomingMessage message);
    }

    /// <summary>
    /// Defines the action to be taken after processing the message
    /// </summary>
    public enum MessageAction
    {
        /// <summary>
        /// Complete the message processing. This message will be removed from the queue
        /// </summary>
        Complete,
        /// <summary>
        /// Reject the message. This message will be removed from the queue
        /// </summary>
        Reject,
        /// <summary>
        /// Requeue the message. This message will be requeued
        /// </summary>
        Requeue,
        /// <summary>
        /// Error processing the message. This message will be requeued
        /// </summary>
        Error
    }
}
