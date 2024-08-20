using Ninja.Sharp.OpenMessagingMiddleware.Extensions;

namespace Ninja.Sharp.OpenMessagingMiddleware.Interfaces
{
    public interface IMessageProducer
    {
        Task SendAsync(string topic, string message);
        string Name { get; }
    }
}
