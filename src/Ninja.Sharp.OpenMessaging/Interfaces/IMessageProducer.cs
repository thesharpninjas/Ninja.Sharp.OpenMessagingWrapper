namespace Ninja.Sharp.OpenMessaging.Interfaces
{
    /// <summary>
    /// Wrapper for message delivery
    /// </summary>
    public interface IMessageProducer
    {
        /// <summary>
        /// Sends a message to the specified topic.
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <returns>The message identifier</returns>
        Task<string> SendAsync(string message);

        /// <summary>
        /// The topic used by this producer
        /// </summary>
        string Topic { get; }
    }
}
