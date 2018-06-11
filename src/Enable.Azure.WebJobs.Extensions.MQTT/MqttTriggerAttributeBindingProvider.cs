// Copyright (c) Enable International Ltd. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
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
    internal class MqttTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        private readonly MqttConfiguration _config;

        internal MqttTriggerAttributeBindingProvider(MqttConfiguration config)
        {
            _config = config;
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var parameter = context.Parameter;
            var attribute = parameter.GetCustomAttribute<MqttTriggerAttribute>(inherit: false);

            if (attribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }

            if (!IsSupportedBindingType(parameter.ParameterType))
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Can't bind MqttTriggerAttribute to type '{0}'.",
                        parameter.ParameterType));
            }

            var binding = new MqttTriggerBinding(
                context.Parameter,
                _config,
                attribute.TopicName);

            return Task.FromResult<ITriggerBinding>(binding);
        }

        public bool IsSupportedBindingType(Type type)
        {
            // TODO Support user defined POCO types.
            return (type == typeof(MqttMessage) ||
                type == typeof(string) ||
                type == typeof(byte[]));
        }
    }
}
