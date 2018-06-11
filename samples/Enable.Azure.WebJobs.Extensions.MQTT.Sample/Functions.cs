// Copyright (c) Enable International Ltd. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;
using System.Threading.Tasks;
using Enable.Azure.WebJobs.Mqtt;
using Enable.Azure.WebJobs.Mqtt.Messaging;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Enable.Azure.WebJobs.Extensions.MQTT.Sample
{
    public static class Functions
    {
        /// <summary>
        /// This sample demonstrates how to subscribe to a topic in order to receive MQTT messages.
        /// </summary>
        public static async Task InputFunction(
            [MqttTrigger("topic/#")] MqttMessage message,
            ILogger log)
        {
            var body = Encoding.UTF8.GetString(message.Payload);

            log.LogInformation($"Processing MQTT message: '{body}'.");

            await Task.Delay(500); // Simulate processing the message.

            log.LogInformation("Message processing complete.");
        }

        /// <summary>
        /// This sample demonstrates how to receive MQTT message payloads as strings.
        /// </summary>
        public static async Task InputFunction_StringInput(
            [MqttTrigger("topic/#")] string message,
            ILogger log)
        {
            log.LogInformation($"Processing MQTT message: '{message}'.");

            await Task.Delay(500); // Simulate processing the message.

            log.LogInformation("Message processing complete.");
        }

        /// <summary>
        /// This sample demonstrates how to receive MQTT message payloads as byte arrays.
        /// </summary>
        public static async Task InputFunction_ByteArrayInput(
            [MqttTrigger("topic/#")] byte[] message,
            ILogger log)
        {
            var body = Encoding.UTF8.GetString(message);

            log.LogInformation($"Processing MQTT message: '{body}'.");

            await Task.Delay(500); // Simulate processing the message.

            log.LogInformation("Message processing complete.");
        }

        /// <summary>
        /// This sample demonstrates how to bind additional paramters to properties of the MQTT message.
        /// </summary>
        public static async Task InputFunction_BindingData(
            [MqttTrigger("topic/#")] string message,
            string topic,
            bool retain,
            byte qualityOfServiceLevel,
            ILogger log)
        {
            log.LogInformation($"Processing MQTT message for topic '{topic}': '{message}' (Retain: {retain}, QoS: {qualityOfServiceLevel}).");

            await Task.Delay(500); // Simulate processing the message.

            log.LogInformation("Message processing complete.");
        }


        /// <summary>
        /// This sample demonstrates how to subscribe to a topic in order to receive MQTT messages.
        /// Once the incoming message is processed a message is then published on a separate topic.
        /// </summary>
        public static async Task InputOutputFunction(
            [MqttTrigger("topic/one")] MqttMessage input,
            [Mqtt("topic/two")] MqttMessage output,
            ILogger log)
        {
            var body = Encoding.UTF8.GetString(input.Payload);

            log.LogInformation($"Processing MQTT message: '{body}'.");

            await Task.Delay(500); // Simulate processing the message.

            log.LogInformation("Message processing complete.");

            output = new MqttMessage
            {
                Topic = "topic/two", // TODO Remove this property, or allow it to override what is specified on the `Mqtt` attribute.
                Payload = input.Payload,   // Reflect the incoming message.
                QualityOfServiceLevel = 0,
                Retain = false
            };

            // TODO Demonstrate how to perform some logic to set the queue name.
            // https://github.com/Azure/azure-webjobs-sdk/wiki/Queues#use-webjobs-sdk-attributes-in-the-body-of-a-function

            log.LogInformation("Message processing complete.");
        }

        /// <summary>
        /// This sample demonstrates how to publish strings to a topic.
        /// </summary>
        public static async Task InputOutputFunction_StringOutput(
            [MqttTrigger("topic/one")] MqttMessage input,
            [Mqtt("topic/two")] string output,
            ILogger log)
        {
            var body = Encoding.UTF8.GetString(input.Payload);

            log.LogInformation($"Processing MQTT message: '{body}'.");

            await Task.Delay(500); // Simulate processing the message.

            output = body; // Reflect the incoming message.

            log.LogInformation("Message processing complete.");
        }

        /// <summary>
        /// This sample demonstrates how to publish byte arrays to a topic.
        /// </summary>
        public static async Task InputOutputFunction_ByteArrayOutput(
            [MqttTrigger("topic/one")] MqttMessage input,
            [Mqtt("topic/two")] byte[] output,
            ILogger log)
        {
            var body = Encoding.UTF8.GetString(input.Payload);

            log.LogInformation($"Processing MQTT message: '{body}'.");

            await Task.Delay(500); // Simulate processing the message.

            output = input.Payload; // Reflect the incoming message.

            log.LogInformation("Message processing complete.");
        }

        /// <summary>
        /// This sample demonstrates publishing multiple messages to a MQTT topic from a single function call.
        /// </summary>
        public static void TimerFunction( 
            [TimerTrigger("0 * * * * *")] TimerInfo timerInfo,
            [Mqtt("topic/two")] ICollector<MqttMessage> messages,
            ILogger logger)
        {
            var body = Encoding.UTF8.GetBytes($"It is currently {DateTimeOffset.UtcNow}.");

            messages.Add(
                new MqttMessage
                {
                    Topic = "topic/one", // TODO Remove this property, or allow it to override what is specified on the `Mqtt` attribute.
                    Payload = body,
                    QualityOfServiceLevel = 0,
                    Retain = false
                });
                
            messages.Add(
                new MqttMessage
                {
                    Topic = "topic/two", // TODO Remove this property, or allow it to override what is specified on the `Mqtt` attribute.
                    Payload = body,
                    QualityOfServiceLevel = 0,
                    Retain = false
                });
        }
    }
}
