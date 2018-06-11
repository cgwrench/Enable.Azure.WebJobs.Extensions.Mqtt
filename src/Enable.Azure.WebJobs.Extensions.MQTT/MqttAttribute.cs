// Copyright (c) Enable International Ltd. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Description;

namespace Enable.Azure.WebJobs.Mqtt
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public sealed class MqttAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MqttAttribute"/> class.
        /// </summary>
        /// <param name="topicName">The name of the topic to subscribe to.</param>
        public MqttAttribute(string topicName)
        {
            if (string.IsNullOrWhiteSpace(topicName))
            {
                throw new ArgumentException("Value cannot be empty.", nameof(topicName));
            }

            TopicName = topicName;
        }

        /// <summary>
        /// Gets the name of the topic to subscribe to.
        /// </summary>
        public string TopicName { get; private set; }
    }
}
