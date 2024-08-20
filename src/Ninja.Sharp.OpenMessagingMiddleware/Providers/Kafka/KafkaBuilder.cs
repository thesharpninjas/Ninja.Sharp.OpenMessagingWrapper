using Microsoft.Extensions.DependencyInjection;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Ninja.Sharp.OpenMessagingMiddleware.Model.Configuration;
using Confluent.Kafka;
using Ninja.Sharp.OpenMessagingMiddleware.Exceptions;
using Ninja.Sharp.OpenMessagingMiddleware.Model;
using ActiveMQ.Artemis.Client.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows.Input;

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

        public IMessagingBuilder AddProducer(string topic)
        {
            // TODO dare errore se si chiama più volte

            IProducer<string, string> producer = producerBuilder.Build();
            services.AddScoped<IMessageProducer>(x => new KafkaProducer(producer, topic));
            return this;
        }

        public IMessagingBuilder AddConsumer<TConsumer>(string topic, MessagingType type = MessagingType.Queue, bool acceptIfInError = true) where TConsumer : class, IMessageConsumer
        {
            services.AddScoped<IMessageConsumer, TConsumer>();

            IConsumer<string, string> consumer = consumerBuilder.Build();
            consumer.Subscribe(topic);

            // TODO nella Build (a parte), crea backgroundconsumer e fallo partire
            return this;
        }

        private class KafkaBackgroundConsumer(IConsumer<string, string> kafkaConsumer, IMessageConsumer consumer, bool acceptIfError) : BackgroundService
        {
            protected override Task ExecuteAsync(CancellationToken stoppingToken)
            {
                return StartConsumerLoop(stoppingToken);
            }

            private async Task StartConsumerLoop(CancellationToken stoppingToken)
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        bool inError = false;
                        ConsumeResult<string, string> result = kafkaConsumer.Consume(stoppingToken);
                        if (result.Message != null)
                        {
                            try
                            {
                                await consumer.ConsumeAsync(new Message()
                                {
                                    Body = result.Message.Value,
                                    Id = result.Message.Key
                                });
                            }
                            catch (Exception ex)
                            {
                                inError = true;
                            }
                            finally
                            {
                                if (!inError || acceptIfError)
                                {
                                    kafkaConsumer.Commit(result);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO
                    throw;
                }
                finally
                {
                    kafkaConsumer.Close();
                }
            }
        }
    }
}
