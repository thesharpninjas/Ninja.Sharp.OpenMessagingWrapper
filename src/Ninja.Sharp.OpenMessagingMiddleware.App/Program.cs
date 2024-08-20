// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ninja.Sharp.OpenMessagingMiddleware.Extensions;
using Ninja.Sharp.OpenMessagingMiddleware.Model.Configuration;
using Ninja.Sharp.OpenMessagingMiddleware.Providers.ArtemisMQ;

namespace Ninja.Sharp.OpenMessagingMiddleware.App
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();


            // Mettere qui la logica per scegliere cosa runnare
            await host.StartAsync();

            //var myService = host.Services.GetRequiredService<IExecutorService>();
            //await myService.ExecuteAsync(args);
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
                    .AddSingleton<IConfiguration>(x => configuration);

                 var artemisBuilder = services.AddArtemisServices(configuration);

                 artemisBuilder
                    .AddProducer("topic1") // Volendo si può tipizzare
                    .AddProducer("topic2")
                    .AddConsumer<MqConsumer>("topic3")
                    .AddConsumer<AnotherMqConsumer>("topic4");

                 services.BuildServiceProvider();
             });
    }
}