namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka.Enums
{
    public enum KafkaSecurityProtocol
    {
        /// <summary>
        /// Plaintext connection
        /// </summary>
        Plaintext,
        /// <summary>
        /// SSL
        /// </summary>
        Ssl,
        /// <summary>
        /// SASL plaintext
        /// </summary>
        SaslPlaintext,
        /// <summary>
        /// SASL through SSL
        /// </summary>
        SaslSsl
    }
}
