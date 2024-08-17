namespace Ninja.Sharp.OpenMessagingMiddleware.Interfaces
{
    public interface IMessageConsumer
    {
        Task StartAsync(string topic, Func<string, Task> onMessageReceived);
    }
}
