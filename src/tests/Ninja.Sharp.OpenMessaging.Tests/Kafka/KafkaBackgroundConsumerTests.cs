using AutoFixture.Xunit2;
using Confluent.Kafka;
using Moq;
using Ninja.Sharp.OpenMessaging.Interfaces;
using Ninja.Sharp.OpenMessaging.Model;
using Ninja.Sharp.OpenMessaging.Providers.Kafka;
using Ninja.Sharp.OpenMessaging.Model.Enums;

namespace Ninja.Sharp.OpenMessaging.Tests.Kafka
{
    public class KafkaBackgroundConsumerTests
    {
        [Theory]
        [InlineAutoData]
        [InlineAutoData(MessageAction.Complete)]
        [InlineAutoData(MessageAction.Reject)]
        [InlineAutoData(MessageAction.Error, true)]
        [InlineAutoData(MessageAction.Error, false)]
        [InlineAutoData(MessageAction.Requeue)]
        [InlineAutoData(MessageAction.Complete, true, "Error")]
        public async Task Execute_Ok(MessageAction messageAction, bool acceptIfError, string messageId, ConsumeResult<string, string> consumeResult)
        {
            consumeResult.Message.Key = messageId;
            Mock<IConsumer<string, string>> mockConsumer = new(MockBehavior.Strict);
            mockConsumer.Setup(c => c.Consume(It.IsAny<CancellationToken>())).Returns(consumeResult);
            mockConsumer.Setup(c => c.Close());
            mockConsumer.Setup(c => c.Dispose());
            mockConsumer.Setup(c => c.Commit(It.IsAny<ConsumeResult<string, string>>()));
            Mock<IMessageConsumer> mockMessageConsumer = new(MockBehavior.Strict);
            mockMessageConsumer.Setup(c => c.ConsumeAsync(It.Is<IncomingMessage>(m => m.Id == "Error"))).ThrowsAsync(new Exception());
            mockMessageConsumer.Setup(c => c.ConsumeAsync(It.Is<IncomingMessage>(m => m.Id != "Error"))).ReturnsAsync(messageAction);
            KafkaBackgroundConsumer consumer = new(mockConsumer.Object, mockMessageConsumer.Object, acceptIfError);

            CancellationToken token = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
            Exception? ex = await Record.ExceptionAsync(() => consumer.StartAsync(token));
            await Task.Delay(TimeSpan.FromSeconds(5));

            Assert.Null(ex);
        }

        [Fact]
        public void Dispose_Ok()
        {
            Mock<IConsumer<string, string>> mockConsumer = new(MockBehavior.Strict);
            Mock<IMessageConsumer> mockMessageConsumer = new(MockBehavior.Strict);
            mockConsumer.Setup(c => c.Close());
            mockConsumer.Setup(c => c.Dispose());
            KafkaBackgroundConsumer consumer = new(mockConsumer.Object, mockMessageConsumer.Object, true);

            Exception? ex = Record.Exception(consumer.Dispose);

            Assert.Null(ex);
        }
    }
}
