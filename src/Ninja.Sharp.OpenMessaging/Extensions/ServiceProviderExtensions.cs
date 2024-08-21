using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    internal static class ServiceProviderExtensions
    {
        internal static object TryGetRequiredService(this IServiceProvider serviceProvider, Type type)
        {
            try
            {
                var service = serviceProvider.GetRequiredService(type);
                return service;
            }
            catch (InvalidOperationException)
            {
                // Nel caso di servizi in background, i messaggi possono essere ricevuti quando l'host non è ancora attivo ==> senza scope.
                // In questo caso, dovremmo creare noi uno scope
                var service = serviceProvider.CreateScope().ServiceProvider.GetRequiredService(type);
                return service;
            }
        }

        internal static T TryGetRequiredService<T>(this IServiceProvider serviceProvider) where T : class
        {
            try
            {
                var service = serviceProvider.GetRequiredService<T>();
                return service;
            }
            catch (InvalidOperationException)
            {
                // Nel caso di servizi in background, i messaggi possono essere ricevuti quando l'host non è ancora attivo ==> senza scope.
                // In questo caso, dovremmo creare noi uno scope
                var service = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<T>();
                return service;
            }
        }
    }
}
