namespace Ninja.Sharp.OpenMessagingMiddleware.Interfaces
{
    public interface IMessageBus
    {
        MessageProducer Producer { get; }
        IMessageConsumer Consumer { get; }
    }
}
