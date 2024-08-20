using Microsoft.Extensions.DependencyInjection;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Ninja.Sharp.OpenMessagingMiddleware.Model.Configuration;
using Confluent.Kafka;
using Ninja.Sharp.OpenMessagingMiddleware.Exceptions;
using Ninja.Sharp.OpenMessagingMiddleware.Model;
using Microsoft.Extensions.Hosting;

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
                throw new TransientBrokerException("Nessun bootstrap server configurato");
            }

            // capire come valorizzare una cosa che è solo per INPS
            string caa = "kafka";
            string id = caa + "_" + Guid.NewGuid().ToString();
            string bootstrapServers = string.Join(',', config.BootstrapServers.Select(s => $"{s.Host}:{s.Port}"));
            ProducerConfig producerConfig = new()
            {
                BootstrapServers = bootstrapServers,
                ClientId = id,
                ReceiveMessageMaxBytes = config.ReceiveMessageMaxBytes ?? int.MaxValue,
                SecurityProtocol = (SecurityProtocol?)config.SecurityProtocol ?? SecurityProtocol.Plaintext,
                //Debug = "all"
            };
            producerBuilder = new(producerConfig);
            producerBuilder.SetErrorHandler((a, b) =>
            {
                Console.WriteLine("There was an error sending a message: " + b.Reason);
            });

            ConsumerConfig consumerConfig = new()
            {
                BootstrapServers = bootstrapServers,
                GroupId = config.GroupId,
                AutoOffsetReset = (AutoOffsetReset?)config.AutoOffsetReset ?? AutoOffsetReset.Latest,
                EnableAutoOffsetStore = config.EnableAutoOffsetStore ?? true,
                EnableAutoCommit = config.EnableAutoCommit ?? false,
                ReceiveMessageMaxBytes = config.ReceiveMessageMaxBytes ?? int.MaxValue,
                SecurityProtocol = (SecurityProtocol?)config.SecurityProtocol ?? SecurityProtocol.Plaintext,
                //Debug = "all",
            };
            consumerBuilder = new(consumerConfig);
            consumerBuilder.SetErrorHandler((a, b) =>
            {
                Console.WriteLine("SetErrorHandler: " + b.Reason);
            });
            consumerBuilder.SetOffsetsCommittedHandler((a, b) =>
            {
                Console.WriteLine("SetOffsetsCommittedHandler: " + b.Error);
            });
            consumerBuilder.SetPartitionsRevokedHandler((a, b) =>
            {
                Console.WriteLine("SetPartitionsRevokedHandler");
            });

            this.services = services;
        }

        public IMessagingBuilder AddProducer(string topic, MessagingType type = MessagingType.Queue)
        {
            // TODO dare errore se si chiama più volte o se si mette Queue

            IProducer<string, string> producer = producerBuilder.Build();
            services.AddScoped<IMessageProducer>(x => new KafkaProducer(producer, topic));
            return this;
        }

        public IMessagingBuilder AddConsumer<TConsumer>(string topic, string subscriber = "", MessagingType type = MessagingType.Queue, bool acceptIfInError = true) where TConsumer : class, IMessageConsumer
        {
            // TODO dare errore se si mette Queue
            services.AddScoped<IMessageConsumer, TConsumer>();
            services.AddScoped<TConsumer>();

            IConsumer<string, string> consumer = consumerBuilder.Build();
            consumer.Subscribe(topic);
            services.AddHostedService((a) => new KafkaBackgroundConsumer(consumer, a.GetRequiredService<TConsumer>(), acceptIfInError));

            return this;
        }

        public IServiceCollection Build()
        {
            return services;
        }
    }
}
