using ActiveMQ.Artemis.Client;
using Amqp.Framing;
using Apache.NMS.ActiveMQ.Transport.Mock;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Ninja.Sharp.OpenMessaging.Providers.ArtemisMQ;
using Ninja.Sharp.OpenMessaging.Providers.ArtemisMQ.Configuration;

namespace Ninja.Sharp.OpenMessaging.Tests.Artemis
{
    public class ArtemisMqBuilderTests
    {
        //private ArtemisMqBuilder ServiceUnderTest
        //{
        //    get
        //    {
        //        var serviceCollection = new ServiceCollection();
        //        var config = new ArtemisConfig
        //        {
        //            Endpoints =
        //            [
        //                new ArtemisEndpoint()
        //                {
        //                    Host = "localhost",
        //                    Password = "admin",
        //                    Port = 5672,
        //                    Username = "admin"
        //                }
        //            ],
        //        };
        //        return new ArtemisMqBuilder(serviceCollection, config);
        //    }
        //}

        [Fact]
        public void ArtemisMqBuilder_whenNoEndpoints_thenThrows()
        {
            var serviceCollection = new ServiceCollection();
            var config = new ArtemisConfig
            {
                Endpoints =
                [
                ],
            };

            Assert.Throws<ArgumentException>(() =>
            {
                var _ = new ArtemisMqBuilder(serviceCollection, config);
            });
        }
    }
}