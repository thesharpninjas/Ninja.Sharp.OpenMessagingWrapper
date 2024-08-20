namespace Ninja.Sharp.OpenMessagingMiddleware.Interfaces
{
    public interface IMessageProducerFactory
    {
        IMessageProducer Producer(string topic = "");
    }
}
