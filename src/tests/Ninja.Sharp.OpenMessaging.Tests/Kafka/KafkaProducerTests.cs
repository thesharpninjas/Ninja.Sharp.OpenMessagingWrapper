using AutoFixture.Xunit2;
using Confluent.Kafka;
using Moq;
using Ninja.Sharp.OpenMessaging.Providers.Kafka;
using Ninja.Sharp.OpenMessaging.Providers.Kafka.Configuration;

namespace Ninja.Sharp.OpenMessaging.Tests.Kafka
{
    public class KafkaProducerTests
    {
        [Theory]
        [InlineAutoData]
        public async Task Send_Ok(KafkaConfig config, string topic, string message, DeliveryResult<string, string> deliveryResult)
        {
            Mock<IProducer<string, string>> mockProducer = new(MockBehavior.Strict);
            mockProducer.Setup(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(deliveryResult);
            KafkaProducer producer = new(mockProducer.Object, topic, config);

            string result = await producer.SendAsync(message);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.StartsWith(config.Identifier, result);
        }
    }
}
