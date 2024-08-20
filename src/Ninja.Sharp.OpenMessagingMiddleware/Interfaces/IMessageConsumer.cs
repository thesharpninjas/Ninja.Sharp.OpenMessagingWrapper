using Ninja.Sharp.OpenMessagingMiddleware.Model;

namespace Ninja.Sharp.OpenMessagingMiddleware.Interfaces
{
    
    public interface IMessageConsumer
    {
        Task ConsumeAsync(Message message);
    }
}
