// Copyright (c) Enable International Ltd. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace Enable.Azure.WebJobs.Mqtt
{
    internal class MqttTriggerParameterDescriptor : TriggerParameterDescriptor
    {
        public string TopicName { get; set; }

        public override string GetTriggerReason(IDictionary<string, string> arguments)
        {
            return $"New MQTT message detected on '{TopicName}'.";
        }
    }
}
