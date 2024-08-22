using ActiveMQ.Artemis.Client;
using Moq;
using Ninja.Sharp.OpenMessaging.Factory;
using Ninja.Sharp.OpenMessaging.Interfaces;
using Ninja.Sharp.OpenMessaging.Providers.ArtemisMQ;
using Ninja.Sharp.OpenMessaging.Providers.ArtemisMQ.Configuration;

namespace Ninja.Sharp.OpenMessaging.Tests
{
    public class MessageProducerFactoryTests
    {
        [Fact]
        public void MessageProducerFactory_whenRequestingProducer_thenReturnsProducer()
        {
            var mockAnonymousProducer = new Mock<IMessageProducer>(MockBehavior.Strict);
            mockAnonymousProducer
                .Setup(x => x.Topic)
                .Returns("topic");

            var producerFactory = new MessageProducerFactory(new List<IMessageProducer>()
            {
                mockAnonymousProducer.Object
            });

            var producer = producerFactory.Producer("topic");

            Assert.NotNull(producer);
        }

        [Fact]
        public void MessageProducerFactory_whenNoProducers_thenThrow()
        {
            var producerFactory = new MessageProducerFactory(new List<IMessageProducer>()
            {
                
            });

            Assert.Throws<ArgumentException>(() => producerFactory.Producer("topic"));
        }

        [Fact]
        public void MessageProducerFactory_whenNoProducerForTopic_thenThrows()
        {
            var mockAnonymousProducer = new Mock<IMessageProducer>(MockBehavior.Strict);
            mockAnonymousProducer
                .Setup(x => x.Topic)
                .Returns(Guid.NewGuid().ToString());

            var producerFactory = new MessageProducerFactory(new List<IMessageProducer>()
            {
                mockAnonymousProducer.Object,
                mockAnonymousProducer.Object,
                mockAnonymousProducer.Object
            });

            Assert.Throws<ArgumentException>(() => producerFactory.Producer("topic"));
        }
    }
}