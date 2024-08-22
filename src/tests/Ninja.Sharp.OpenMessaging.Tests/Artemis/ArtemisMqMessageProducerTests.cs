using ActiveMQ.Artemis.Client;
using Moq;
using Ninja.Sharp.OpenMessaging.Providers.ArtemisMQ;

namespace Ninja.Sharp.OpenMessaging.Tests.Artemis
{
    public class ArtemisMqMessageProducerTests
    {
        [Fact]
        public async Task ArtemisMqMessageProducer_whenSending_thenReturnsMessageId()
        {
            var mockAnonymousProducer = new Mock<IAnonymousProducer>(MockBehavior.Strict);
            mockAnonymousProducer.Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<RoutingType>(), It.IsAny<Message>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);


            var producer = new ArtemisMqMessageProducer(mockAnonymousProducer.Object);

            var msgId = await producer.PublishAsync("message", "queue", "identifier");

            Assert.NotEmpty(msgId);
        }
    }
}