﻿using Ninja.Sharp.OpenMessagingMiddleware.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninja.Sharp.OpenMessagingMiddleware.Interfaces
{
    public static class MessageProducerExtensions
    {
        public static async Task<string> SendAsync<T>(this IMessageProducer producer, T message)
        {
            string serializedData = message.Serialize();
            return await producer.SendAsync(serializedData);
        }
    }
}
