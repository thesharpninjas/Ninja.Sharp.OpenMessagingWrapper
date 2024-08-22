namespace Ninja.Sharp.OpenMessagingWrapper.Providers.Kafka.Enums
{
    /// <summary>
    /// SASL mechanism
    /// </summary>
    public enum KafkaSaslMechanism
    {
        /// <summary>
        /// Generic Security Services API
        /// </summary>
        Gssapi,
        /// <summary>
        /// Plain
        /// </summary>
        Plain,
        /// <summary>
        /// Scram SHA 256
        /// </summary>
        ScramSha256,
        /// <summary>
        /// Scram SHA 512
        /// </summary>
        ScramSha512,
        /// <summary>
        /// OAuth Bearer
        /// </summary>
        OAuthBearer
    }
}
