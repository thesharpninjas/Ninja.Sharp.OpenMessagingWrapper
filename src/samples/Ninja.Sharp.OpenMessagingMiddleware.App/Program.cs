// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;

namespace Ninja.Sharp.OpenMessagingMiddleware.App
{
    public static class Program
    {
        const string topicArtemis = "MS00536.AS.AckINPSPRE";
        const string topicKafka = "hello-world";

        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Mettere qui la logica per scegliere cosa runnare
            await host.StartAsync();

            var myMessageProducerFactory = host.Services.GetRequiredService<IMessageProducerFactory>();
            var myDoctor = host.Services.GetRequiredService<HealthCheckService>();

            while (true)
            {
                var idArtemis = await myMessageProducerFactory.Producer(topicArtemis).SendAsync(new Tester()
                {
                    Property1 = Guid.NewGuid().ToString(),
                    Property2 = Guid.NewGuid().ToString()
                });
                Console.WriteLine("Sent message with ID " + idArtemis);

                var idKafka = await myMessageProducerFactory.Producer(topicKafka).SendAsync(new Tester()
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

        //https://refactoring.guru/design-patterns/builder/csharp/example#example-0
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
                     .AddProducer(topicArtemis)
                     .AddConsumer<LoggerMqConsumer>(topicArtemis)
                     .Build();

                 services
                     .AddKafkaServices(configuration)
                     .AddProducer(topicKafka)
                     .AddConsumer<LoggerMqConsumer>(topicKafka)
                     .Build();

                 services.BuildServiceProvider();
             });
    }
}