using AutoFixture;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;
using System.Diagnostics;

namespace Ninja.Sharp.OpenMessaging.Tests
{
    public class AppFixture : IDisposable
    {
        internal Fixture Fixture { get; private set; }

        public AppFixture()
        {
            Fixture = new();
            Fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior(recursionDepth: 1));
            Fixture.RepeatCount = 100;
        }

        public static ILogger<T> GetLogger<T>()
        {
            var logger = new Mock<ILogger<T>>();

            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()))
                .Callback(new InvocationAction(invocation =>
                {
                    LogLevel logLevel = (LogLevel)invocation.Arguments[0];
                    object state = invocation.Arguments[2];
                    Exception exception = (Exception)invocation.Arguments[3];
                    object formatter = invocation.Arguments[4];

                    MethodInfo? invokeMethod = formatter.GetType().GetMethod("Invoke");
                    object? logMessage = invokeMethod!.Invoke(formatter, [state, exception]);

                    Trace.WriteLine($"{logLevel} - {logMessage}");
                }));

            return logger.Object;
        }

        public static IConfigurationRoot Configuration
        {
            get
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                   .AddInMemoryCollection(new Dictionary<string, string?>()
                   {
                        { "Name:SubName", "value" }
                   }).Build();
                return config;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
        }

    }
}
