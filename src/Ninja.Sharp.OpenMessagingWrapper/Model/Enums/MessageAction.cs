using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingWrapper.Model.Enums
{
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
