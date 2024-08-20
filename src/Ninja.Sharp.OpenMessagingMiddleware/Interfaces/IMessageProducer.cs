namespace Ninja.Sharp.OpenMessagingMiddleware.Interfaces
{
    public interface IMessageProducer
    {
        Task SendAsync(string topic, string message);
        Task SendAsync<T>(string topic, T message);
    }
}
