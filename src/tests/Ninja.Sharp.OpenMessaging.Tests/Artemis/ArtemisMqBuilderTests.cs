using Microsoft.Extensions.DependencyInjection;
using Ninja.Sharp.OpenMessaging.Providers.ArtemisMQ;
using Ninja.Sharp.OpenMessaging.Providers.ArtemisMQ.Configuration;

namespace Ninja.Sharp.OpenMessaging.Tests.Artemis
{
    public class ArtemisMqBuilderTests
    {
        private ArtemisMqBuilder ServiceUnderTest
        {
            get
            {
                var serviceCollection = new ServiceCollection();
                var config = new ArtemisConfig
                {
                    Endpoints = new List<ArtemisEndpoint>()
                    {
                        new ArtemisEndpoint()
                        {
                            Host = "localhost",
                            Password = "admin",
                            Port = 5672,
                            Username = "admin"
                        }
                    },
                };
                return new ArtemisMqBuilder(serviceCollection, config);
            }
        }

        [Fact]
        public void Test1()
        {
            
        }
    }
}