using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enable.Azure.WebJobs.Mqtt;
using Enable.Azure.WebJobs.Mqtt.Messaging;
using Microsoft.Azure.WebJobs;

namespace Enable.Azure.WebJobs.Mqtt
{
    internal class MqttMessageAsyncCollector : IAsyncCollector<MqttMessage>
    {
        private readonly MqttClient _messageSender;
        private readonly IList<MqttMessage> _messages = new List<MqttMessage>();

        public MqttMessageAsyncCollector(MqttClient messageSender)
        {
            if (messageSender == null)
            {
                throw new ArgumentNullException(nameof(messageSender));
            }

            _messageSender = messageSender;
        }

        public Task AddAsync(
            MqttMessage item,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _messages.Add(item);

            return Task.CompletedTask;
        }

        public Task FlushAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            _messageSender.SendAsync(_messages);

            return Task.CompletedTask;
        }
    }
}
