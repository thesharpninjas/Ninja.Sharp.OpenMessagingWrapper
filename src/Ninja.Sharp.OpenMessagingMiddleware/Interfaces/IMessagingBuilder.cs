using Microsoft.Extensions.DependencyInjection;
using Ninja.Sharp.OpenMessagingMiddleware.Model.Enums;

namespace Ninja.Sharp.OpenMessagingMiddleware.Interfaces
{
    public interface IMessagingBuilder
    {
        IMessagingBuilder AddConsumer<TConsumer>(string topic, string subscriber = "", MessagingType type = MessagingType.Queue, bool acceptIfInError = true) where TConsumer : class, IMessageConsumer;
        IMessagingBuilder AddProducer(string topic, MessagingType type = MessagingType.Queue);
        IServiceCollection Build();
    }
}
