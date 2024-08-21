namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka.Enums
{
    public enum KafkaAutoOffsetReset
    {
        /// <summary>
        /// Reset on latest message
        /// </summary>
        Latest,
        /// <summary>
        /// Reset on earliest message
        /// </summary>
        Earliest,
        /// <summary>
        /// Generate error if no previous offset is found
        /// </summary>
        Error
    }
}
