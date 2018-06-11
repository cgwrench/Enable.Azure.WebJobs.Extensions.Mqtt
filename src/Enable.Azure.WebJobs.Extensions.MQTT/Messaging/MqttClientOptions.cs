// Copyright (c) Enable International Ltd. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Enable.Azure.WebJobs.Mqtt.Messaging
{
    public class MqttClientOptions
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
