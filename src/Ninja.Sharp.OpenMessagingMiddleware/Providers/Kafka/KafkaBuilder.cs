using Microsoft.Extensions.DependencyInjection;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Ninja.Sharp.OpenMessagingMiddleware.Model.Configuration;
using Confluent.Kafka;
using Ninja.Sharp.OpenMessagingMiddleware.Exceptions;
using Ninja.Sharp.OpenMessagingMiddleware.Model;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka
{
    internal class KafkaBuilder : IMessagingBuilder
    {
        private readonly IServiceCollection services;
        private readonly ProducerBuilder<string, string> producerBuilder;
        private readonly ConsumerBuilder<string, string> consumerBuilder;

        public KafkaBuilder(IServiceCollection services, KafkaConfig config)
        {
            if (config.BootstrapServers.Count == 0)
            {
                throw new ArgumentException("There are no bootstrap servers configured");
            }

            if (services.Any(x => x.ServiceType == typeof(KafkaConfig)))
            {
                throw new ArgumentException("You cannot add more than one Artemis service.");
            }

            string id = config.Identifier + "_" + Guid.NewGuid().ToString();
            id = id.TrimStart('_');

            string bootstrapServers = string.Join(',', config.BootstrapServers.Select(s => $"{s.Host}:{s.Port}"));
            ProducerConfig producerConfig = new()
            {
                BootstrapServers = bootstrapServers,
                ClientId = id,
                ReceiveMessageMaxBytes = config.ReceiveMessageMaxBytes ?? int.MaxValue,
                SecurityProtocol = (SecurityProtocol?)config.SecurityProtocol ?? SecurityProtocol.Plaintext,
                //Debug = "all"
            };

            if (producerConfig.SecurityProtocol == SecurityProtocol.SaslSsl)
            {
                producerConfig.SaslUsername = config.UserName;
                producerConfig.SaslPassword = config.Password;
                producerConfig.SaslMechanism = (SaslMechanism)config.SaslMechanism;
            }

            producerBuilder = new(producerConfig);

            ConsumerConfig consumerConfig = new()
            {
                BootstrapServers = bootstrapServers,
                GroupId = config.GroupId,
                AutoOffsetReset = (AutoOffsetReset?)config.AutoOffsetReset ?? AutoOffsetReset.Latest,
                EnableAutoOffsetStore = config.EnableAutoOffsetStore ?? true,
                EnableAutoCommit = config.EnableAutoCommit ?? false,
                ReceiveMessageMaxBytes = config.ReceiveMessageMaxBytes ?? int.MaxValue,
                SecurityProtocol = (SecurityProtocol?)config.SecurityProtocol ?? SecurityProtocol.Plaintext
            };

            if (consumerConfig.SecurityProtocol == SecurityProtocol.SaslSsl)
            {
                consumerConfig.SaslUsername = config.UserName;
                consumerConfig.SaslPassword = config.Password;
                consumerConfig.SaslMechanism = (SaslMechanism)config.SaslMechanism;
            }

            consumerBuilder = new(consumerConfig);

            this.services = services;

            services.AddSingleton(config);
        }

        public IMessagingBuilder AddProducer(string topic, MessagingType type = MessagingType.Queue)
        {
            IProducer<string, string> producer = producerBuilder.Build();

            services.AddProducer<IMessageProducer>(topic, (a) => new KafkaProducer(producer, topic));
            return this;
        }

       

        public IMessagingBuilder AddConsumer<TConsumer>(string topic, string subscriber = "", MessagingType type = MessagingType.Queue, bool acceptIfInError = true) where TConsumer : class, IMessageConsumer
        {
            // TODO dare errore se si mette Queue
            services.AddScoped<IMessageConsumer, TConsumer>();
            services.AddScoped<TConsumer>();

            IConsumer<string, string> consumer = consumerBuilder.Build();
            consumer.Subscribe(topic);
            services.AddHostedService((a) => new KafkaBackgroundConsumer(consumer, a.TryGetRequiredService<TConsumer>(), acceptIfInError));

            return this;
        }

        public IServiceCollection Build()
        {
            return services;
        }
    }
}
