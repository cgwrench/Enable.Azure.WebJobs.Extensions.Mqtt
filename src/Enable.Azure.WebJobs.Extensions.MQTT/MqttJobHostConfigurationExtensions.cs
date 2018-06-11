// Copyright (c) Enable International Ltd. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Enable.Azure.WebJobs.Mqtt;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.WindowsAzure.Storage.Table;

namespace Enable.Azure.WebJobs
{
    public static class MqttJobHostConfigurationExtensions
    {
        public static void UseMqtt(this JobHostConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var mqttConfig = new MqttConfiguration();

            config.UseMqtt(mqttConfig);
        }

        /// <summary>
        /// Enable sending and receiving MQTT messages. This call is required to use the <see cref="MqttAttribute"/> 
        /// and <see cref="MqttTriggerAttribute"/> attributes on parameter bindings.
        /// </summary>
        /// <param name="config">Job host configuration.</param>
        /// <param name="eventHubConfig">MQTT configuration.</param>
        public static void UseMqtt(
            this JobHostConfiguration config,
            MqttConfiguration mqttConfiguration)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (mqttConfiguration == null)
            {
                throw new ArgumentNullException(nameof(mqttConfiguration));
            }

            var extensions = config.GetService<IExtensionRegistry>();
            extensions.RegisterExtension<IExtensionConfigProvider>(mqttConfiguration);
        }
    }
}
