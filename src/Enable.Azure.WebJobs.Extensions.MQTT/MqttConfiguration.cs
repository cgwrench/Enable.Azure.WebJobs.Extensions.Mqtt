// Copyright (c) Enable International Ltd. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;
using System.Threading.Tasks;
using Enable.Azure.WebJobs.Mqtt;
using Enable.Azure.WebJobs.Mqtt.Messaging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Enable.Azure.WebJobs.Mqtt
{
    /// <summary>
    /// Configuration options for the MQTT extension.
    /// </summary>
    public class MqttConfiguration : IExtensionConfigProvider
    {
        private readonly MessagingProvider _messagingProvider;

        public MqttConfiguration()
        {
            _messagingProvider = new MessagingProvider(this);
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        internal MessagingProvider MessagingProvider => _messagingProvider;

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context
                .AddConverter<MqttMessage, byte[]>(ConvertMqttMessageToByteArray)
                .AddConverter<MqttMessage, string>(ConvertMqttMessageToString)
                .AddConverter<MqttMessage, string>(ConvertMqttMessageToString);

            // Register our binding provider.
            var binding = context.AddBindingRule<MqttAttribute>();

            binding
                .AddConverter<byte[], MqttMessage>(ConvertByteArrayToMqttMessage)
                .AddConverter<string, MqttMessage>(ConvertStringToMqttMessage)
                .AddConverter<JObject, MqttMessage>(ConvertJObjectToMqttMessage)
                .AddOpenConverter<OpenType.Poco, MqttMessage>(ConvertPocoToMqttMessage);

            binding.AddValidator(ValidateMqttAttribute);
            binding.BindToInput<MqttMessage>(BuildMessageFromAttribute);
            binding.BindToCollector<MqttMessage>(BuildCollectorFromAttribute);

            // Register our trigger binding provider.            
            var triggerBindingProvider = new MqttTriggerAttributeBindingProvider(this);

            context.AddBindingRule<MqttTriggerAttribute>()
                .BindToTrigger(triggerBindingProvider);
        }

        private MqttMessage BuildMessageFromAttribute(MqttAttribute attribute)
        {
            return new MqttMessage { Topic = attribute.TopicName };
        }

        private IAsyncCollector<MqttMessage> BuildCollectorFromAttribute(MqttAttribute attribute)
        {
            var topicName = attribute.TopicName;

            var client = _messagingProvider.GetMessageClient(topicName);

            return new MqttMessageAsyncCollector(client);
        }

        private static byte[] ConvertMqttMessageToByteArray(MqttMessage message)
            => message.Payload;

        private static string ConvertMqttMessageToString(MqttMessage message) 
            => Encoding.UTF8.GetString(ConvertMqttMessageToByteArray(message));

        private static MqttMessage ConvertByteArrayToMqttMessage(byte[] input) 
            => new MqttMessage { Payload = input, QualityOfServiceLevel = 0, Retain = false };

        private static MqttMessage ConvertStringToMqttMessage(string input) 
            => ConvertByteArrayToMqttMessage(Encoding.UTF8.GetBytes(input));

        private MqttMessage ConvertJObjectToMqttMessage(JObject input)
            => input.ToObject<MqttMessage>();

        private static Task<object> ConvertPocoToMqttMessage(
            object input,
            Attribute attribute,
            ValueBindingContext context)
        {
            return Task.FromResult<object>(
                ConvertStringToMqttMessage(
                    JsonConvert.SerializeObject(input)));
        }

        private static void ValidateMqttAttribute(MqttAttribute attribute, Type parameterType)
        {
            if (string.IsNullOrWhiteSpace(attribute?.TopicName))
            {
                throw new ArgumentException("Value cannot be empty.", nameof(attribute.TopicName));
            }
        }
    }
}
