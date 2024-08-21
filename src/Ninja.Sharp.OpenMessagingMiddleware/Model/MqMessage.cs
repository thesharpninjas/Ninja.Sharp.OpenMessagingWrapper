namespace Ninja.Sharp.OpenMessagingMiddleware.Model
{
    /// <summary>
    /// Defines structural data for the delivered message
    /// </summary>
    public class MqMessage
    {
        /// <summary>
        /// The message ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        // The message group ID. Often used as an identifier for a group of subscribers
        public string GroupId { get; set; } = string.Empty;

        // The message body. This field contains the message payload
        public string Body { get; set; } = string.Empty;
    }

}
