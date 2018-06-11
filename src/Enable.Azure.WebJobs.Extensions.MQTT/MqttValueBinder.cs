// Copyright (c) Enable International Ltd. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Enable.Azure.WebJobs.Mqtt.Messaging;
using Microsoft.Azure.WebJobs.Extensions.Bindings;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Enable.Azure.WebJobs.Mqtt
{
    internal class MqttValueBinder : ValueBinder
    {
        private readonly object _value;
        private readonly string _invokeString;

        private MqttValueBinder(
            ParameterInfo parameter,
            object value,
            string invokeString)

            : base(parameter.ParameterType)
        {
            _value = value;
            _invokeString = invokeString;
        }

        public static MqttValueBinder Create(
            MqttMessage message,
            ParameterInfo parameter,
            object value)
        {
            var invokeString = message?.Payload != null
                ? string.Format(CultureInfo.InvariantCulture, "byte[{0}]", message.Payload.Length)
                : "null";

            return new MqttValueBinder(parameter, value, invokeString);
        }

        public override Task<object> GetValueAsync()
        {
            return Task.FromResult<object>(_value);
        }

        public override string ToInvokeString()
        {
            return _invokeString;
        }
    }
}
