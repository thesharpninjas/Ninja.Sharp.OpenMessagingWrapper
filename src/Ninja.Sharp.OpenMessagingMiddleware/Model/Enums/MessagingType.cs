namespace Ninja.Sharp.OpenMessagingMiddleware.Model.Enums
{
    /// <summary>
    /// Defines the messaging channel
    /// </summary>
    public enum MessagingType
    {
        /// <summary>
        /// Queue
        /// </summary>
        Queue,
        /// <summary>
        /// Broadcast, or group of listeners
        /// </summary>
        Event
    }
}
