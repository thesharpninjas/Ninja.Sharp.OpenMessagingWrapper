# OpenMessagingWrapperWrapper - a .NET wrapper for message brokers

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![issues - Ninja.Sharp.OpenMessagingWrapperWrapper](https://img.shields.io/github/issues/thesharpninjas/Ninja.Sharp.OpenMessagingWrapperWrapper)](https://github.com/thesharpninjas/Ninja.Sharp.OpenMessagingWrapperWrapper/issues)
[![stars - Ninja.Sharp.OpenMessagingWrapperWrapper](https://img.shields.io/github/stars/thesharpninjas/Ninja.Sharp.OpenMessagingWrapperWrapper?style=social)](https://github.com/thesharpninjas/Ninja.Sharp.OpenMessagingWrapperWrapper)

Release Notes
-------------
First release has been created!

Packages
--------

| Package | NuGet Stable | 
| ------- | ------------ | 
| [OpenMessagingWrapperWrapper](https://github.com/thesharpninjas/Ninja.Sharp.OpenMessagingWrapperWrapper/) | [![OpenMessagingWrapperWrapper](https://img.shields.io/badge/nuget-v0.0.1-blue)](https://www.nuget.org/packages/Ninja.Sharp.OpenMessagingWrapperWrapper)

Features
--------
OpenMessagingWrapperWrapper is a [NuGet .NET library](https://www.nuget.org/packages/Ninja.Sharp.OpenMessagingWrapperWrapper) that aims in simplifying usage for the most common messaging framework.
The library encapsulates some behavior and common options for most frameworks, providing a simple management to rule them all via configuration.

Right now, it allows integration with these framework:
 - [**Apache ArtemisMQ**](https://activemq.apache.org/components/artemis/): a next-generation message broker, based on the HornetQ code-base. The implementation is based on [ArtemisNetClient](https://havret.github.io/dotnet-activemq-artemis-client/docs/getting-started/) 
 - [**Apache Kafka**](https://kafka.apache.org/): Apache Kafka is an open-source distributed event streaming platform used by thousands of companies for high-performance data pipelines, streaming analytics, data integration, and mission-critical applications. The implementation is based on [Confluent .NET client](https://docs.confluent.io/kafka-clients/dotnet/current/overview.html)

### Next steps:
We'll try to provide abstraction for most popular message brokers, such as
  - [**Azure ServiceBus**]
  - [**Azure StorageQueue**]
  - [**Azure EventGrid**]
  - [**Amazon MQ**]
  - [**Amazon SQS**]
  - [**Amazon SNS**]
  - [**GCloud Pub/Sub**]
  - [**RabbitMQ**]

## Limitations
OpenMessagingWrapperWrapper allows to use in the same application several message brokers at a time. However
 - you cannot use multiple instances of the same message broker; you can use ArtemisMQ **and** Kafka, but you cannot connect use multiple instances of ArtemisMQ.
 - you cannot use the same topic name twice, even if the topic resides in different message brokers.

## Configuration
OpenMessagingWrapperWrapper can be configured manually or via appsettings.json. Configuration differs for each message broker, while the subsequent usage will be hidden by OpenMessagingWrapperWrapper framework. 
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
OpenMessagingWrapperWrapper manages allows you to add several message brokers, and simplify the message management.
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
Once configured, you can inject the `IMessageProducerFactory` instance that you'll use to send a single message

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
Receive a message could be quite a pain, depending on the specific broker implementation. 
OpenMessagingWrapperWrapper simplifies message management, you just need to provide, while adding a Consumer, a class, implementing `IMessageConsumer`. 
Whenever a message is available for the specified topic, the method ConsumeAsync will be triggered, providing you basic info about the message.
You just need to specify if the message has been correctly processed (returning `MessageAction.Complete`), if it needs to be reprocessed (`MessageAction.Requeue`), or it should be discarded (`MessageAction.Reject`).

``` csharp
    public class LoggerMqConsumer(ILogger<LoggerMqConsumer> logger) : IMessageConsumer
    {
        public Task<MessageAction> ConsumeAsync(MqMessage message)
        {
            logger.LogWarning("Message consumed: {0}", message.Body);
            return Task.FromResult(MessageAction.Complete);
        }
    }
```

## Bonus: infrastructure healthcheck
Sometimes, can happen that you message broker can fail, disconnected, crash, or whatever. When this happens, your services need to be restarted. 
If you're using a k8s cluster, you'll need to implement healthcheck for readiness and liveness, to tell your cluster when pods needs to be started again.
OpenMessagingWrapperWrapper exploits the amazing features provided by Microsoft [HealthChecks](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.diagnostics.healthchecks?view=net-8.0) and adds some checks for the provided brokers and topics. You just need to build a simple liveness/readiness probe to use them

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

## Licensee
Repository source code is available under MIT License, see license in the source.

OpenMessagingWrapperWrapper uses the **confluent-kafka-dotnet** library, which is distributed under Apache 2.0 license:
* [Official repository](https://github.com/confluentinc/confluent-kafka-dotnet)
* [License](https://github.com/confluentinc/confluent-kafka-dotnet/blob/master/LICENSE)
  
OpenMessagingWrapperWrapper uses the **Apache.NMS.ActiveMQ** library, which is distributed under Apache 2.0 license:
* [Official repository](https://github.com/apache/activemq-nms-openwire)
* [License](https://github.com/apache/activemq-nms-openwire/blob/main/LICENSE.txt)

## Contributing
Thank you for considering to help out with the source code!
If you'd like to contribute, please fork, fix, commit and send a pull request for the maintainers to review and merge into the main code base.

**Getting started with Git and GitHub**

 * [Setting up Git](https://docs.github.com/en/get-started/getting-started-with-git/set-up-git)
 * [Fork the repository](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/fork-a-repo)
 * [Open an issue](https://github.com/thesharpninjas/Ninja.Sharp.OpenMessagingWrapperMiddleware/issues) if you encounter a bug or have a suggestion for improvements/features
