# Enable.Azure.WebJobs.Extensions.Mqtt

// TODO Adapt code to follow: https://github.com/Azure/azure-webjobs-sdk/wiki/Creating-custom-input-and-output-bindings.

MQTT bindings for Azure Functions and WebJobs.

MQTT is a messaging system designed for for high-latency or unreliable networks. This project adds WebJobs bindings for MQTT messaging with similar semantics to how you bind to Azure Queues or Azure Service Bus Queues.

## Simple example

Here's a basic example of bindings. Just as with [Queue] bindings, the bindings will automatically JSON-serialized the user-defined type, `Payload`, to and from the native type, `MqttMessage`. SendEvents() will emit one message to the event hub. Trigger() will listen on one event and emit a new event with an incremented counter.

Note this sample is binding against EventHubs without ever referring directly to the event hubs SDK.

    using Enable.Azure.WebJobs.Mqtt;

    namespace Enable.Azure.WebJobs.Extensions.MQTT.Sample
    {
        public class BasicTest
        {
            public class Payload
            {
                public int Counter { get; set; }
            }

            public static void SendEvents([Mqtt("TopicName")] out Payload input)
            {
                input = new Payload { Counter = 0 };
            }

            public static void Trigger(
                [MqttTrigger("TopicName")] Payload input,
                [Mqtt("TopicName")] out Payload output)
            {
                input.Counter++;
                output = input;
            }
        }
    }

## Configuration

Provide the MQTT connection settings to the initial `JobHostConfiguration` object

    var config = new JobHostConfiguration{
        DashboardConnectionString = "UseDevelopmentStorage=true",
        StorageConnectionString = "UseDevelopmentStorage=true"
    };


    var mqttConfiguration = new MqttConfiguration
    {
        Host = "{Enter hostname here}",
        Port = 1234,
        UserName = "{Enter username here}",
        Password = "{Enter password here}"
    }

    config.UseEventHub(eventHubConfig);

    var host = new JobHost(config);

## Receiving events

Use the `[MqttTrigger]` attribute to listen on topics.

Examples of bindings to receive one event:

    void Handle([MqttTrigger("TopicName")] MqttMessage message);
    void Handle([MqttTrigger("TopicName")] string message); // As a string.
    void Handle([MqttTrigger("TopicName")] byte[] message); // As a byte array.
    void Handle([MqttTrigger("TopicName")] MyUserType message); // As a PCOC, via JSON serialization.

// TODO Support receiving a batch of events. This is important if you need to handle a high volume of events.

## Sending Events

Use the [Mqtt] attribute to send messages out to an MQTT topic.

Binding via out parameters to send a single message:

    void Send([Mqtt("TopicName")] out MqttMessage message);
    void Send([Mqtt("TopicName")] out string message);
    void Send([Mqtt("TopicName")] out MyUserType message);

Binding via an out-array to send multiple events

    void SendBatch([Mqtt("TopicName")] out MqttMessage[] output);
    void SendBatch([Mqtt("TopicName")] out string[] output);
    void SendBatch([Mqtt("TopicName")] out MyUserType[] output);

Binding via collector interfaces:

    Task SendBatchAsync([Mqtt("TopicName")] IAsyncCollector<MqttMessage> output);
    void SendBatch([Mqtt("TopicName")] ICollector<MqttMessage> output);

    Task SendBatchAsync([Mqtt("TopicName")] IAsyncCollector<string> output);
    void SendBatch([Mqtt("TopicName")] ICollector<MyUserType> output);
