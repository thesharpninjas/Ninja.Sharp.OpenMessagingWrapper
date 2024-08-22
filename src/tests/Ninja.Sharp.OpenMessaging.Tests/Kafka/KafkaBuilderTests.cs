using AutoFixture.Xunit2;
using Microsoft.Extensions.DependencyInjection;
using Ninja.Sharp.OpenMessaging.Interfaces;
using Ninja.Sharp.OpenMessaging.Model;
using Ninja.Sharp.OpenMessaging.Model.Enums;
using Ninja.Sharp.OpenMessaging.Providers.Kafka;
using Ninja.Sharp.OpenMessaging.Providers.Kafka.Configuration;
using Ninja.Sharp.OpenMessaging.Providers.Kafka.Enums;

namespace Ninja.Sharp.OpenMessaging.Tests.Kafka
{
    public class KafkaBuilderTests
    {
        [Theory]
        [InlineAutoData(null)]
        [InlineAutoData(KafkaSecurityProtocol.SaslSsl, null, null)]
        [InlineAutoData(KafkaSecurityProtocol.SaslPlaintext)]
        [InlineAutoData]
        public void Construct_Ok(KafkaSecurityProtocol? securityProtocol, KafkaSaslMechanism? saslMechanism, KafkaAutoOffsetReset? autoOffsetReset, KafkaConfig config)
        {
            ServiceCollection services = new();
            config.SecurityProtocol = securityProtocol;
            config.SaslMechanism = saslMechanism;
            config.AutoOffsetReset = autoOffsetReset;

            KafkaBuilder builder = new(services, config);

            Assert.NotNull(builder);
        }

        [Theory]
        [InlineAutoData]
        public void Construct_NoBootstrapServers_ArgumentException(KafkaConfig config)
        {
            ServiceCollection services = new();
            config.BootstrapServers.Clear();

            Assert.Throws<ArgumentException>(() => new KafkaBuilder(services, config));
        }

        [Theory]
        [InlineAutoData]
        public void Construct_AlreadyRegistered_ArgumentException(KafkaConfig config)
        {
            ServiceCollection services = new();
            services.AddSingleton(config);

            Assert.Throws<ArgumentException>(() => new KafkaBuilder(services, config));
        }

        [Theory]
        [InlineAutoData]
        public void AddProducer_Ok(KafkaConfig config, string topic, Channel type)
        {
            ServiceCollection services = new();
            config.ReceiveMessageMaxBytes += 1000;
            KafkaBuilder builder = new(services, config);

            IMessagingBuilder messagingBuilder = builder.AddProducer(topic, type);

            Assert.NotNull(messagingBuilder);
            Assert.NotEmpty(services.Where(s => s.ServiceType == typeof(IMessageProducer)));
        }

        private class EmptyMqConsumer : IMessageConsumer
        {
            public Task<MessageAction> ConsumeAsync(IncomingMessage message)
            {
                return Task.FromResult(MessageAction.Complete);
            }
        }

        [Theory]
        [InlineAutoData]
        public void AddConsumer_Ok(KafkaConfig config, string topic, string subscriber, Channel type, bool acceptIfError)
        {
            ServiceCollection services = new();
            config.ReceiveMessageMaxBytes = int.MaxValue;
            KafkaBuilder builder = new(services, config);

            IMessagingBuilder messagingBuilder = builder.AddConsumer<EmptyMqConsumer>(topic, subscriber, type, acceptIfError);

            Assert.NotNull(messagingBuilder);
            Assert.NotEmpty(services.Where(s => s.ServiceType == typeof(IMessageConsumer)));
        }

        [Theory]
        [InlineAutoData]
        public void Build_Ok(KafkaConfig config, string topic, Channel type)
        {
            ServiceCollection services = new();
            config.ReceiveMessageMaxBytes = int.MaxValue;
            config.HealthChecks = true;
            KafkaBuilder builder = new(services, config);
            builder.AddProducer(topic, type);

            IServiceCollection updatedServices = builder.Build();

            Assert.NotNull(updatedServices);
            Assert.NotEmpty(updatedServices);
        }

        [Theory]
        [InlineAutoData]
        public void Build_NoHealthCheck_Ok(KafkaConfig config)
        {
            ServiceCollection services = new();
            config.HealthChecks = false;
            KafkaBuilder builder = new(services, config);

            IServiceCollection updatedServices = builder.Build();

            Assert.NotNull(updatedServices);
            Assert.NotEmpty(updatedServices);
        }

    }
}
