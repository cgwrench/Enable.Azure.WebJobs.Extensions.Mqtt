// Copyright (c) Enable International Ltd. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using Enable.Azure.WebJobs.Mqtt.Messaging;

namespace Enable.Azure.WebJobs.Mqtt.Messaging
{
    public class MessagingProvider
    {
        private readonly MqttClientOptions _config;
        private readonly ConcurrentDictionary<string, MqttClient> _messageClientCache = new ConcurrentDictionary<string, MqttClient>();

        public MessagingProvider(MqttConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _config = new MqttClientOptions
            {
                Host = _config.Host,
                Port = _config.Port,
                UserName = _config.UserName,
                Password = _config.Password
            };;
        }

        public MqttClient GetMessageClient(string topicName)
        {
            if (string.IsNullOrWhiteSpace(topicName))
            {
                throw new ArgumentException("Value cannot be empty.", nameof(topicName));
            }

            return GetOrAddMessageClient(topicName);
        }

        private MqttClient GetOrAddMessageClient(string topicName)
        {
            string cacheKey = topicName;

            return _messageClientCache.GetOrAdd(
                cacheKey,
                new MqttClient(_config, topicName));
        }
    }
}
