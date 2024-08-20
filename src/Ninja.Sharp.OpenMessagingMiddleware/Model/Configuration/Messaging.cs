using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingMiddleware.Model.Configuration
{
    internal class Messaging
    {
        public ArtemisConfig Artemis { get; set; } = new();
        public KafkaConfig Kafka { get; set; } = new();
    }
}
