
OpenMessaging - a .Net wrapper for message brokers
========================================
Release Notes
-------------
First release has been created!

Packages
--------

| Package | NuGet Stable | 
| ------- | ------------ | 
| [OpenSODA](https://github.com/thesharpninjas/Ninja.Sharp.OpenMessaging/) | [![OpenMessaging](https://img.shields.io/badge/nuget-v0.0.1-blue)](https://www.nuget.org/packages/Ninja.Sharp.OpenMessaging/)

Features
--------
OpenMessaging is a [NuGet library](https://www.nuget.org/packages/Ninja.Sharp.OpenMessaging) that aims in simplifying usage for the most common messaging framework.
The library encapsulates some behavior and common options for most frameworks, providing a simple management to rule them all via configuration.

Right now, it allows integration with these framework:
 - [**Apache ArtemisMQ**](https://activemq.apache.org/components/artemis/): a next-generation message broker, based on the HornetQ code-base. The implementation is based on [ArtemisNetClient](https://havret.github.io/dotnet-activemq-artemis-client/docs/getting-started/) 
 - [**Apache Kafka**]: Apache Kafka is an open-source distributed event streaming platform used by thousands of companies for high-performance data pipelines, streaming analytics, data integration, and mission-critical applications. The implementation is based on [Confluent .NET client](https://docs.confluent.io/kafka-clients/dotnet/current/overview.html)

Next steps:
 we'll try to provide abstraction for most popular message brokers, such as
  - [**Azure ServiceBus**]
  - [**Azure StorageQueue**]
  - [**Azure EventGrid**]
  - [**Amazon MQ**]
  - [**Amazon SQS**]
  - [**Amazon SNS**]
  - [**GCloud Pub/Sub**]
  - [**RabbitMQ**]

## Limitations
OpenMessaging allows to use in the same application several message brokers at a time. However
 - you cannot use multiple instances of the same message broker; you can use ArtemisMQ AND Kafka, but you cannot connect multiple instances of ArtemisMQ.
 - you cannot use the same topic name twice, even if the topic resides in different message brokers.

## Configuration
OpenMessaging can be configured manually or via appsettings.json. Configuration differs for each message broker, while the subsequent usage will be hidden by OpenMessaging framework. 

If you're using appsetting.json, then you just need to add configurations under the 'Messaging' tag:

``` json
{
  "Messaging": {
    "Kafka": {
        "Identifier": "myIdentifier",
        "BootstrapServers": [
          {
            "Host": "myHost", 
            "Port": 9092
          }
        ],
        "SecurityProtocol": "SaslSsl", 
        "UserName": "myUsername",
        "Password": "myPassword",
        "GroupId": "myGroupId",
        "SaslMechanism": "ScramSha256"
    },
    "Artemis": {
        "Identifier": "myIdentifier",
        "Retries": 2,
        "RetryWaitTime": 500,
        "Endpoints": [
          {
            "Host": "myHost",
            "Port": 61616,
            "Username": "myUsername",
            "Password": "myUsername",
          }
        ]
    },
  }
}
```
otherwise, if you're using manual configuration, you just need to add configurations when you add the requested services 
``` csharp
builder.Services.AddArtemisServices(new ArtemisConfig()
    {
        //...
    });
```

## Choose and add a provider
OpenMessaging manages allows you to add several message brokers, and simplify the message management.
You just need to provide, for each message broker you are configuring
 - the topics/queues where you need a Producer (the object that *sends* messages)
 - the topics/queues where you need a Consumer (the object that *receive* messages), and the the class that will manage those messages. These class **must** implement IMessageConsumer.

``` csharp
builder.Services
    .AddArtemisServices(builder.Configuration)
    .AddProducer("myArtemisProducerTopic")
    .AddConsumer<LoggerMqConsumer>("myArtemisConsumerTopic")
    .Build();
    
builder.Services
    .AddKafkaServices(builder.Configuration)
    .AddProducer("myKafkaProducerTopic1")
    .AddProducer("myKafkaProducerTopic2")
    .AddProducer("myKafkaProducerTopic3")
    .AddConsumer<LoggerMqConsumer>("myKafkaConsumerTopic1")
    .AddConsumer<LoggerMqConsumer>("myKafkaConsumerTopic2")
    .Build();
```

## Sending messages
Once configured, sending a message is quite easy. You do not need to know how the broker work, or which broker you need - you just need the destination topic/queue and the message you have to send.
Once configured, you can inject the IMessageProducerFactory instance that you'll use to send a single message

``` csharp
    [ApiController]
    [Route("[controller]")]
    public class MyApiController(IMessageProducerFactory producerFactory) : ControllerBase
    {
        private readonly IMessageProducerFactory producerFactory = producerFactory;

        [HttpPost("Send/{topic}")]
        public async Task<string> Get([FromRoute]string topic, [FromBody]Tester payload)
        {
            var producer = producerFactory.Producer(topic);
            var msgId = await producer.SendAsync(payload);
            return msgId;
        }
    }
```
## Receiving messages
Receive a message could be quite a pain, depending on the specific broker implementation. OpenMessaging simplifies message management, you just need to provide, while adding a Consumer, a class, implementing IMessageConsumer. Whenever a message is available for the specified topic, the method ConsumeAsync will be triggered, providing you basic info about the message

``` csharp
  public class LoggerMqConsumer(ILogger<LoggerMqConsumer> logger) : IMessageConsumer
    {
        public Task ConsumeAsync(MqMessage message)
        {
            logger.LogWarning("Message consumed: {0}", message.Body);
            return Task.CompletedTask;
        }
    }
```

## Bonus: infrastructure healthcheck
Sometimes, can happen that you message broker can fail, disconnected, crash, or whatever. When this happens, your services need to be restarted. 
If you're using a k8s cluster, you'll need to implement healthcheck for readiness and liveness, to tell your cluster when pods needs to be started again.
OpenMessaging exploits the amazing features provided by Microsoft [HealthChecks](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.diagnostics.healthchecks?view=net-8.0) and adds some checks for the provided brokers and topics. You just need to build a simple liveness/readiness probe to use them

``` csharp
    [ApiController]
    [Route("[controller]")]
    public class MyApiController(HealthCheckService healthCheckService) : ControllerBase
    {
        private readonly HealthCheckService healthCheckService = healthCheckService;
    
        [HttpGet]
        [Route("liveness")]
        [Route("readiness")]
        public async Task<IActionResult> Get()
        {
            HealthReport report = await healthCheckService.CheckHealthAsync();
            return report.Status == HealthStatus.Healthy ? Ok(report) : StatusCode((int)HttpStatusCode.ServiceUnavailable, report);
        }
    }
```




 ## Contributing
Thank you for considering to help out with the source code!
If you'd like to contribute, please fork, fix, commit and send a pull request for the maintainers to review and merge into the main code base.
 
**Getting started with Git and GitHub**
 
* [Setting up Git for Windows and connecting to GitHub](http://help.github.com/win-set-up-git/)
* [Forking a GitHub repository](http://help.github.com/fork-a-repo/)
* [The simple guide to GIT guide](http://rogerdudler.github.com/git-guide/)
* [Open an issue](https://github.com/thesharpninjas/Ninja.Sharp.OpenSODA/issues) if you encounter a bug or have a suggestion for improvements/features
****