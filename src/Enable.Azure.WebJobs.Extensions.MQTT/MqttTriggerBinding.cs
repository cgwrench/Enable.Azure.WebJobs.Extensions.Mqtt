// Copyright (c) Enable International Ltd. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Enable.Azure.WebJobs.Mqtt.Messaging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Bindings;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Enable.Azure.WebJobs.Mqtt
{
    internal class MqttTriggerBinding : ITriggerBinding
    {
        private readonly ParameterInfo _parameter;
        private readonly MqttConfiguration _config;
        private readonly string _topicName;
        private readonly IReadOnlyDictionary<string, Type> _bindingContract;

        public MqttTriggerBinding(
            ParameterInfo parameter,
            MqttConfiguration config,
            string topicName)
        {
            _parameter = parameter;
            _config = config;
            _topicName = topicName;
            _bindingContract = CreateBindingDataContract();
        }

        public IReadOnlyDictionary<string, Type> BindingDataContract => _bindingContract;

        public Type TriggerValueType => typeof(MqttMessage);

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            // TODO Use converters to convert to `MqttMessage`.
            // var message = value as MqttMessage;
            // if (message == null &&
            //     !_converter.TryConvert(value, out message))
            // {
            //     throw new InvalidOperationException(
            //         $"Uanble to bind {value} to type '{_parameter.ParameterType}'.");
            // }

            var message = value as MqttMessage;

            if (message == null)
            {
                if (value is string stringValue)
                {
                    message = new MqttMessage
                    {
                        Payload = Encoding.UTF8.GetBytes(stringValue),
                        QualityOfServiceLevel = 0,
                        Retain = false,
                        Topic = _topicName
                    };
                }
            }

            if (message == null)
            {
                throw new InvalidOperationException(
                    $"Uanble to bind {value} to type '{_parameter.ParameterType}'.");
            }

            var bindingData = CreateBindingData(message);

            // TODO Use converters to convert to `_parameter.ParameterType`.
            // TODO Support user-defined POCO types.
            object argument;
            if (_parameter.ParameterType == typeof(string))
            {
                argument = Encoding.UTF8.GetString(message.Payload);
            }
            else if (_parameter.ParameterType == typeof(byte[]))
            {
                argument = message.Payload;
            }
            else
            {
                argument = message;
            }

            var valueBinder = MqttValueBinder.Create(message, _parameter, argument);

            return Task.FromResult<ITriggerData>(new TriggerData(valueBinder, bindingData));
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            var listener = new MqttListener(_config, _topicName, context.Executor);

            return Task.FromResult<IListener>(listener);
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new MqttTriggerParameterDescriptor
            {
                Name = _parameter.Name,
                TopicName = _topicName,
                DisplayHints = new ParameterDisplayHints
                {
                    Prompt = "Enter the queue message body",
                    Description = string.Format(CultureInfo.CurrentCulture, "dequeue from '{0}'", _topicName),
                    DefaultValue = null

                }
            };
        }

        private IReadOnlyDictionary<string, object> CreateBindingData(MqttMessage value)
        {
            var bindingData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            bindingData.Add("QualityOfServiceLevel", value.QualityOfServiceLevel);
            bindingData.Add("Retain", value.Retain);
            bindingData.Add("Topic", value.Topic);

            return bindingData;
        }

        private IReadOnlyDictionary<string, Type> CreateBindingDataContract()
        {
            var contract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            contract.Add("QualityOfServiceLevel", typeof(byte));
            contract.Add("Retain", typeof(bool));
            contract.Add("Topic", typeof(string));

            return contract;
        }
    }
}
