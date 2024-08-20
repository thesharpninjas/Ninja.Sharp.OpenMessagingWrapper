// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

                 // Questa parte aggiunge il producer, sempre di tipo anonimo (il topic per ora andrà passato col sendasync)
                 var messagingBuilder = services
                    .AddMessagingServices(configuration); // Prende da appsettings o lo fa vuoto se non trova nulla

                 messagingBuilder
                    .AddKafka(KafkaConfig config) // Prende da oggetto
                    .AddArtemis(ArtemisConfig config);  // Prende da oggetto

                 /// Consumer col delegate
                 /// Il consumer va messo PER OGNI sistema di messaggistica che ho configurato
                 /// Aggiungere un booleano throw if unauthorized per capire se lanciare un'eccezione se non ho i permessi
                 messagingBuilder.AddConsumer<T>("topic"); // where T : IMessageConsumer

                 services.BuildServiceProvider();
             });
    }
}