using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingMiddleware.Model.Configuration
{
    public class ArtemisConfig
    {
        public ICollection<ArtemisEndpoint> Endpoints { get; set; } = [];

    }

    public class ArtemisEndpoint
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; } = 0;
        public string Host { get; set; } = string.Empty;
    }
}
