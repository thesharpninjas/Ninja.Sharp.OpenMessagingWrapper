using Microsoft.Extensions.DependencyInjection;
using Ninja.Sharp.OpenMessaging.Interfaces;
using Confluent.Kafka;
using Ninja.Sharp.OpenMessaging.Model.Enums;
using Ninja.Sharp.OpenMessaging.Providers.Kafka.Configuration;
using Ninja.Sharp.OpenMessaging.Providers.Kafka.HealthCheck;

namespace Ninja.Sharp.OpenMessaging.Providers.Kafka
{
    internal class KafkaBuilder : IMessagingBuilder
    {
        private readonly KafkaConfig configuration;
        private readonly IServiceCollection services;
        private readonly ProducerBuilder<string, string> producerBuilder;
        private readonly ConsumerBuilder<string, string> consumerBuilder;
        private readonly IHealthChecksBuilder? healthBuilder;
        private readonly ICollection<string> producerTopics = [];

        public KafkaBuilder(IServiceCollection services, KafkaConfig config)
        {
            if (config.BootstrapServers.Count == 0)
            {
                throw new ArgumentException("There are no bootstrap servers configured");
            }
            configuration = config;

            if (services.Any(x => x.ServiceType == typeof(KafkaConfig)))
            {
                throw new ArgumentException("You cannot add more than one Kafka service.");
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
            if (producerConfig.SecurityProtocol == SecurityProtocol.SaslSsl || producerConfig.SecurityProtocol == SecurityProtocol.SaslPlaintext)
            {
                producerConfig.SaslUsername = config.UserName;
                producerConfig.SaslPassword = config.Password;
                producerConfig.SaslMechanism = (SaslMechanism?)config.SaslMechanism ?? SaslMechanism.Plain;
            }
            producerBuilder = new(producerConfig);
            producerBuilder.SetErrorHandler(KafkaConnectionHealthCheck.KafkaProducerErrorHandler);

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
            if (consumerConfig.SecurityProtocol == SecurityProtocol.SaslSsl || consumerConfig.SecurityProtocol == SecurityProtocol.SaslPlaintext)
            {
                consumerConfig.SaslUsername = config.UserName;
                consumerConfig.SaslPassword = config.Password;
                consumerConfig.SaslMechanism = (SaslMechanism?)config.SaslMechanism ?? SaslMechanism.Plain;
            }
            consumerBuilder = new(consumerConfig);
            consumerBuilder.SetErrorHandler(KafkaConnectionHealthCheck.KafkaConsumerErrorHandler);

            this.services = services;

            services.AddSingleton(config);
            if (config.HealthChecks)
            {
                healthBuilder = services.AddHealthChecks();
            }
        }

        public IMessagingBuilder AddProducer(string topic, MessagingType type = MessagingType.Queue)
        {
            IProducer<string, string> producer = producerBuilder.Build();
            services.AddProducer<IMessageProducer>(topic, (a) => new KafkaProducer(producer, topic, configuration));

            producerTopics.Add(topic);

            return this;
        }

        public IMessagingBuilder AddConsumer<TConsumer>(string topic, string subscriber = "", MessagingType type = MessagingType.Queue, bool acceptIfInError = true) where TConsumer : class, IMessageConsumer
        {
            IConsumer<string, string> consumer = consumerBuilder.Build();
            consumer.Subscribe(topic);

            services.AddScoped<IMessageConsumer, TConsumer>();
            services.AddScoped<TConsumer>();
            services.AddHostedService((a) => new KafkaBackgroundConsumer(consumer, a.TryGetRequiredService<TConsumer>(), acceptIfInError));

            return this;
        }

        public IServiceCollection Build()
        {
            if (configuration.HealthChecks && healthBuilder != null)
            {
                string[] tags = ["Kafka"];
                healthBuilder.AddCheck("Kafka", new KafkaConnectionHealthCheck(), tags: tags);
                foreach (string topic in producerTopics.Distinct())
                {
                    healthBuilder.AddCheck($"Kafka connection for topic {topic}", new KafkaTopicHealthCheck(configuration, topic), tags: tags);
                }
            }
            return services;
        }
    }
}
