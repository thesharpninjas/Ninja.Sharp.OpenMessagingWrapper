using ActiveMQ.Artemis.Client;
using Moq;
using Ninja.Sharp.OpenMessagingWrapper.Interfaces;
using Ninja.Sharp.OpenMessagingWrapper.Model.Enums;
using Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ;
using Ninja.Sharp.OpenMessagingWrapper.Providers.ArtemisMQ.Configuration;

namespace Ninja.Sharp.OpenMessagingWrapper.Tests.Artemis
{
    public class ArtemisMqProducerTests
    {
        [Fact]
        public async Task ArtemisMqProducer_whenSending_thenReturnsMessageId()
        {
            var mockAnonymousProducer = new Mock<IAnonymousProducer>();
            mockAnonymousProducer
                .Setup(x => x.SendAsync(It.IsAny<string>(), null, It.IsAny<Message>(), null, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var mockArtemisMqClient = new Mock<IArtemisMqClient>();
            mockArtemisMqClient
                .Setup(x => x.CreateAnonymousProducer())
                .ReturnsAsync(mockAnonymousProducer.Object);

            var producer = new ArtemisMqProducer(mockArtemisMqClient.Object, "topic", Channel.Queue, "identifier");

            var msgId = await producer.SendAsync("message");

            Assert.NotEmpty(msgId);
            Assert.Equal("topic", producer.Topic);
        }

        [Fact]
        public async Task ArtemisMqProducer_whenChanneldClosed_thenThrows()
        {
            var mockAnonymousProducer = new Mock<IAnonymousProducer>();
            mockAnonymousProducer
                .Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<RoutingType>(), It.IsAny<Message>(), null, It.IsAny<CancellationToken>()))
                .Throws<ArgumentException>();

            var mockArtemisMqClient = new Mock<IArtemisMqClient>();
            mockArtemisMqClient
                .Setup(x => x.CreateAnonymousProducer())
                .ReturnsAsync(mockAnonymousProducer.Object);

            var producer = new ArtemisMqProducer(mockArtemisMqClient.Object, "topic", Channel.Queue, "identifier");

            await Assert.ThrowsAsync<TaskCanceledException>(async () => await producer.SendAsync("message"));
        }
    }
}