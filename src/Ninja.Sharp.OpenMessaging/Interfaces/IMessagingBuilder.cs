using Microsoft.Extensions.DependencyInjection;
using Ninja.Sharp.OpenMessaging.Model.Enums;

namespace Ninja.Sharp.OpenMessaging.Interfaces
{
    /// <summary>
    /// Builder for creating messaging brokers.
    /// Whenever you add a new broker, you must implement this interface
    /// </summary>
    public interface IMessagingBuilder
    {
        /// <summary>
        /// Adds a consumer to the specified topic.
        /// </summary>
        /// <typeparam name="TConsumer">The class receiving the message. It must implement IMessageConsumer</typeparam>
        /// <param name="topic">The topic where you have to listen for messages</param>
        /// <param name="subscriber">(Optional) the subscriber for this queue</param>
        /// <param name="type">The messaging type, queue or broadcast (event)</param>
        /// <param name="acceptIfInError">If true, even when an exception is thrown by the TConsumer, the message will be flagged as received</param>
        /// <returns>The builder instance</returns>
        IMessagingBuilder AddConsumer<TConsumer>(string topic, string subscriber = "", Channel type = Channel.Queue, bool acceptIfInError = true) where TConsumer : class, IMessageConsumer;

        /// <summary>
        /// Adds a producer to the specified topic.
        /// </summary>
        /// <param name="topic">The topic where you have to send messages</param>
        /// <param name="type">The messaging type, queue or broadcast (event)</param>
        /// <returns>The builder instance</returns>
        IMessagingBuilder AddProducer(string topic, Channel type = Channel.Queue);

        /// <summary>
        /// Builder method. Creates listeners and providers, and adds the desired services to the collection
        /// </summary>
        /// <returns></returns>
        IServiceCollection Build();
    }
}
