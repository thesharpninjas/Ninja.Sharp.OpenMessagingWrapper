using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingMiddleware.Model
{
    /// <summary>
    /// Da capire quali parametri sono trasversali a tutti i messaggi
    /// </summary>
    public class Message
    {
        public string Id { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }

}
