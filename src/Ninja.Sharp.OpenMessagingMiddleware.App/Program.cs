// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ninja.Sharp.OpenMessagingMiddleware.Extensions;
using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using Ninja.Sharp.OpenMessagingMiddleware.Model.Configuration;
using Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ;

namespace Ninja.Sharp.OpenMessagingMiddleware.App
{
    public static class Program
    {
        const string topic = "MS00536.AS.AckINPSPRE";

        class Tester
        {
            public string Pippo { get; set; } = string.Empty;
        }

        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Mettere qui la logica per scegliere cosa runnare
            await host.StartAsync();

            var myMessageProducerFactory = host.Services.GetRequiredService<IMessageProducerFactory>();
            
            while (true)
            {
                var id = await myMessageProducerFactory.Producer(topic).SendAsync(new Tester()
                {
                    Pippo = Guid.NewGuid().ToString()
                });
                Console.WriteLine("Sent message with ID " + id);
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
                    .AddArtemisServices(configuration)
                    .AddProducer(topic) // Volendo si può tipizzare
                    .AddConsumer<MqConsumer>(topic)
                    .Build();

                 services.BuildServiceProvider();
             });
    }
}