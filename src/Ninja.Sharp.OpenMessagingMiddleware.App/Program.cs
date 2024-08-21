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
        //const string topic = "MS00536.AS.AckINPSPRE";
        const string topic = "hello-world";

        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Mettere qui la logica per scegliere cosa runnare
            await host.StartAsync();

            //var myMessageProducerFactory = host.Services.GetRequiredService<IMessageProducerFactory>();
            var myMessageProducer = host.Services.GetRequiredService<IMessageProducer>();
            //var myDoctor = host.Services.GetRequiredService<HealthCheckService>();

            while (true)
            {
                //var id = await myMessageProducerFactory.Producer(topic).SendAsync(new Tester()
                var id = await myMessageProducer.SendAsync(new Tester()
                {
                    Property1 = Guid.NewGuid().ToString(),
                    Property2 = Guid.NewGuid().ToString()
                });
                Console.WriteLine("Sent message with ID " + id);

                //HealthReport report = await myDoctor.CheckHealthAsync();
                //Console.WriteLine("Status: " + report.Status);

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

                 services = services
                    //.AddArtemisServices(configuration)
                    .AddKafkaServices(configuration)
                    .AddProducer(topic) // Volendo si può tipizzare
                    .AddConsumer<LoggerMqConsumer>(topic)
                    .Build();

                 services.BuildServiceProvider();
             });
    }
}