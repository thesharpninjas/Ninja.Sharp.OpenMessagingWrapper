using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Ninja.Sharp.OpenMessaging.Interfaces;
using System.Net;

namespace Ninja.Sharp.OpenMessaging.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyApiController(IMessageProducerFactory producerFactory, HealthCheckService healthCheckService, ILogger<MyApiController> logger) : ControllerBase
    {
        private readonly IMessageProducerFactory producerFactory = producerFactory;
        private readonly HealthCheckService healthCheckService = healthCheckService;
        private readonly ILogger<MyApiController> _logger = logger;

        [HttpPost("Send/{topic}")]
        public async Task<string> Get([FromRoute]string topic, [FromBody]Tester payload)
        {
            var producer = producerFactory.Producer(topic);
            var msgId = await producer.SendAsync(payload);
            _logger.LogInformation("Sent message with ID {0}", msgId);
            return msgId;
        }

        [HttpGet]
        [Route("liveness")]
        [Route("readiness")]
        public async Task<IActionResult> Get()
        {
            HealthReport report = await healthCheckService.CheckHealthAsync();

            return report.Status == HealthStatus.Healthy ? Ok(report) : StatusCode((int)HttpStatusCode.ServiceUnavailable, report);
        }
    }
}
