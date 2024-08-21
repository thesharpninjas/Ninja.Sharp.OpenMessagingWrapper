﻿using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Ninja.Sharp.OpenMessagingMiddleware.Model;

namespace Ninja.Sharp.OpenMessagingMiddleware.Providers.Kafka
{
    internal class KafkaBackgroundConsumer(IConsumer<string, string> kafkaConsumer, IMessageConsumer consumer, bool acceptIfError) : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Thread thread = new(async () => await StartConsumerLoopAsync(stoppingToken));
            thread.Start();
            return Task.CompletedTask;
        }

        private async Task StartConsumerLoopAsync(CancellationToken stoppingToken)
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
                            await consumer.ConsumeAsync(new MqMessage()
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
                kafkaConsumer.Dispose();
            }
        }
        public override void Dispose()
        {
            kafkaConsumer.Close();
            kafkaConsumer.Dispose();

            base.Dispose();
        }
    }
}
