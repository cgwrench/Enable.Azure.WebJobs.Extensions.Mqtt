// Copyright (c) Enable International Ltd. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.ManagedClient;

namespace Enable.Azure.WebJobs.Mqtt.Messaging
{
    public class MqttClient
    {
        private readonly MqttClientOptions _config;
        private readonly string _topicName;
        private readonly IMqttClientFactory _mqttFactory;
        private readonly IManagedMqttClient _innerMqttClient;

        private bool _started;
        private SemaphoreSlim _syncLock = new SemaphoreSlim(1);

        public MqttClient(
            MqttClientOptions config,
            string topicName)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (string.IsNullOrWhiteSpace(topicName))
            {
                throw new ArgumentException("Value cannot be empty.", nameof(topicName));
            }

            _config = config;
            _topicName = topicName;

            _mqttFactory = new MqttFactory();
            _innerMqttClient = _mqttFactory.CreateManagedMqttClient();
        }

        public async Task RegisterMessageHandler(
            Func<MqttMessage, CancellationToken, Task> messageHander)
        {
            if (!_started)
            {
                await _syncLock.WaitAsync();
                
                try
                {
                    if (!_started)
                    {
                        var topicFilter = new TopicFilterBuilder()
                            .WithTopic(_topicName)
                            .Build();

                        await _innerMqttClient.SubscribeAsync(topicFilter);

                        var options = new ManagedMqttClientOptionsBuilder()
                            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                            .WithClientOptions(new MqttClientOptionsBuilder()
                                .WithClientId(Guid.NewGuid().ToString())
                                .WithTcpServer(_config.Host, _config.Port)
                                .WithCredentials(_config.UserName, _config.Password)
                                .WithTls()
                                .Build())
                            .Build();

                        await _innerMqttClient.StartAsync(options);

                        _started = false;
                    }
                }
                finally
                {
                    _syncLock.Release();
                }
            }

            throw new NotImplementedException();
        }

        public Task SendAsync(
            IEnumerable<MqttMessage> messages)
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync()
        {
            throw new NotImplementedException();
        }
    }
}
