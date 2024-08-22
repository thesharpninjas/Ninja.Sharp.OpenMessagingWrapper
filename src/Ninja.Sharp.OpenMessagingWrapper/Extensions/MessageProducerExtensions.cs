namespace Ninja.Sharp.OpenMessagingWrapper.Interfaces
{
    public static class MessageProducerExtensions
    {
        /// <summary>
        /// Sends a message to the specified topic.
        /// </summary>
        /// <param name="message">The object defining the message to be sent</param>
        /// <returns>The message identifier</returns>
        public static async Task<string> SendAsync<T>(this IMessageProducer producer, T message)
        {
            string serializedData = message.Serialize();
            return await producer.SendAsync(serializedData);
        }
    }
}
