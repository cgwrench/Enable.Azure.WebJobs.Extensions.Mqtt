// Copyright (c) Enable International Ltd. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Enable.Azure.WebJobs.Extensions.MQTT.Sample
{
    public static class Program
    {
        public static void Main()
        {
            var config = new JobHostConfiguration{
                DashboardConnectionString = "UseDevelopmentStorage=true",
                StorageConnectionString = "UseDevelopmentStorage=true"
            };

            config.UseCore();
            config.UseTimers();
            
            config.UseMqtt(/* TODO */); // Here is where the magic happens.

            config.LoggerFactory = new LoggerFactory().AddConsole();

            var host = new JobHost(config);
            host.RunAndBlock();
        }
    }
}
