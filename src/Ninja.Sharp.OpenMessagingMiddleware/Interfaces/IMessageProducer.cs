using Ninja.Sharp.OpenMessagingMiddleware.Extensions;

namespace Ninja.Sharp.OpenMessagingMiddleware.Interfaces
{
    public interface IMessageProducer
    {
        Task<string> SendAsync(string message);
        string Topic { get; }
    }
}
