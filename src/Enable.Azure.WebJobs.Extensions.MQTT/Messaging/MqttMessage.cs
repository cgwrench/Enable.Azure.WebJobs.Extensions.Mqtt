// Copyright (c) Enable International Ltd. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Newtonsoft.Json;

namespace Enable.Azure.WebJobs.Mqtt.Messaging
{
    public class MqttMessage
    {
        [JsonProperty(PropertyName = "topic")]
        public string Topic { get; set; } // TODO Remove this property. Make this available as binding data.

        [JsonProperty(PropertyName = "payload")]
        public byte[] Payload { get; set; }

        [JsonProperty(PropertyName = "qosLevel")]
        public byte QualityOfServiceLevel { get; set; }

        [JsonProperty(PropertyName = "retain")]
        public bool Retain { get; set; }

        public override string ToString()
        {
            return $@"{{
    topic: {Topic},
    payload: {Payload},
    qosLevel: {QualityOfServiceLevel},
    retain: {Retain}
}}";
        }
    }
}
