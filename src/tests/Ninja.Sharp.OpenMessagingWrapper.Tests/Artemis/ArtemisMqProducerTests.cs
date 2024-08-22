using ActiveMQ.Artemis.Client;
using Moq;
using Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ;
using Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ.Configuration;

namespace Ninja.Sharp.OpenMessagingWrapper.Tests.Artemis
{
    public class ArtemisMqProducerTests
    {
        [Fact]
        public async Task ArtemisMqProducer_whenSending_thenReturnsMessageId()
        {
            var mockAnonymousProducer = new Mock<IAnonymousProducer>(MockBehavior.Strict);
            mockAnonymousProducer
                .Setup(x => x.SendAsync(It.IsAny<string>(), null, It.IsAny<Message>(), null, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var producer = new ArtemisMqProducer(new ArtemisMqMessageProducer(mockAnonymousProducer.Object), "topic", new ArtemisConfig());

            var msgId = await producer.SendAsync("message");

            Assert.NotEmpty(msgId);
            Assert.Equal("topic", producer.Topic);
        }

        [Fact]
        public async Task ArtemisMqProducer_whenChanneldClosed_thenThrows()
        {
            var mockAnonymousProducer = new Mock<IAnonymousProducer>(MockBehavior.Strict);
            mockAnonymousProducer
                .Setup(x => x.SendAsync(It.IsAny<string>(), null, It.IsAny<Message>(), null, It.IsAny<CancellationToken>()))
                .Throws<ArgumentException>();

            var producer = new ArtemisMqProducer(new ArtemisMqMessageProducer(mockAnonymousProducer.Object), "topic", new ArtemisConfig());

            await Assert.ThrowsAsync<TaskCanceledException>(async () => await producer.SendAsync("message"));
        }
    }
}