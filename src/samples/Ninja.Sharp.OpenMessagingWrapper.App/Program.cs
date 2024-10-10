// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Ninja.Sharp.OpenMessagingWrapper.Extensions;
using Ninja.Sharp.OpenMessagingWrapper.Interfaces;

namespace Ninja.Sharp.OpenMessagingWrapper.App
{
    public static class Program
    {
        const string artemisTopic = "artemisTopic";
        const string kafkaTopic = "kafkaTopic";

        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Mettere qui la logica per scegliere cosa runnare
            await host.StartAsync();

            var myMessageProducerFactory = host.Services.GetRequiredService<IMessageProducerFactory>();
            var myDoctor = host.Services.GetRequiredService<HealthCheckService>();

            while (true)
            {
                var idArtemis = await myMessageProducerFactory.Producer(artemisTopic).SendAsync(new Tester()
                {
                    Property1 = Guid.NewGuid().ToString(),
                    Property2 = Guid.NewGuid().ToString()
                });
                Console.WriteLine("Sent message with ID " + idArtemis);

                var idKafka = await myMessageProducerFactory.Producer(kafkaTopic).SendAsync(new Tester()
                {
                    Property1 = Guid.NewGuid().ToString(),
                    Property2 = Guid.NewGuid().ToString()
                });
                Console.WriteLine("Sent message with ID " + idKafka);

                HealthReport report = await myDoctor.CheckHealthAsync();
                Console.WriteLine("Status: " + report.Status);

                await Task.Delay(5000);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
         Host.CreateDefaultBuilder(args)
             .ConfigureServices((hostContext, services) =>
             {
                 var builder = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json", optional: false)
                   .AddUserSecrets(typeof(Program).Assembly)
                   .AddEnvironmentVariables();

                 var configuration = builder.Build();

                 services
                     .AddArtemisServices(configuration)
                     .AddProducer(artemisTopic)
                     .AddConsumer<LoggerMqConsumer>(artemisTopic)
                     .Build();

                 services
                     .AddKafkaServices(configuration)
                     .AddProducer(kafkaTopic)
                     .AddConsumer<LoggerMqConsumer>(kafkaTopic)
                     .Build();

                 services.BuildServiceProvider();
             });
    }
}