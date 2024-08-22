using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Ninja.Sharp.OpenMessaging.Interfaces;
using Ninja.Sharp.OpenMessaging.Model;
using Ninja.Sharp.OpenMessaging.Model.Enums;

namespace Ninja.Sharp.OpenMessaging.Providers.Kafka
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
                    MessageAction resultAction = MessageAction.Complete;
                    ConsumeResult<string, string> result = kafkaConsumer.Consume(stoppingToken);
                    if (result.Message != null)
                    {
                        try
                        {
                            resultAction = await consumer.ConsumeAsync(new IncomingMessage()
                            {
                                Body = result.Message.Value,
                                Id = result.Message.Key
                            });
                        }
                        catch (Exception)
                        {
                            resultAction = MessageAction.Error;
                        }
                        finally
                        {
                            switch (resultAction)
                            {
                                case MessageAction.Complete:
                                    kafkaConsumer.Commit(result);
                                    break;
                                case MessageAction.Reject:
                                    kafkaConsumer.Commit(result);
                                    break;
                                case MessageAction.Error:
                                    if (acceptIfError)
                                    {
                                        kafkaConsumer.Commit(result);
                                    }
                                    break;
                                default:       
                                    break;
                            }
                        }
                    }
                }
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
