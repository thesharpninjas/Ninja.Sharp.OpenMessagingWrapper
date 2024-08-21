namespace Ninja.Sharp.OpenMessagingMiddleware.Interfaces
{
    /// <summary>
    /// Factory for creating message producers
    /// </summary>
    public interface IMessageProducerFactory
    {
        /// <summary>
        /// Find and return the producer for the specified topic.
        /// If no provider is found, an error will be thrown.
        /// </summary>
        /// <param name="topic">The topic you want to send data to. 
        /// If no topic is specified, will be returned the first declared produced.</param>
        /// <returns>The message producer instance, or throw exception otherwise</returns>
        IMessageProducer Producer(string topic = "");
    }
}
