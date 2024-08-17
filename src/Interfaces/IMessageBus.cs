namespace Ninja.Sharp.OpenMessagingMiddleware.Interfaces
{
    public interface IMessageBus
    {
        IMessageProducer Producer { get; }
        IMessageConsumer Consumer { get; }
    }
}
