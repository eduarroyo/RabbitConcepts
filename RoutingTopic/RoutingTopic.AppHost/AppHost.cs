using Projects;

var builder = DistributedApplication.CreateBuilder(args);

string[] topics =
[
    "accountability.europe.analytics", // Should be received by subscriber 2 
    "user.europe.analytics", // Should be received by subscribers 1 and 2
    "user.asia", // Should be received by subscriber 1
    "transaction.america.payments", // Should be received by subscriber 3
    "user.europe.payments", // Should be received by all subscribers
    "user", // Should be received by subscriber 1
    "payments", // will it be received by subscriber 3?
    "a.b.c" // no subscriber should receive this
];
var
    topicSubscription1 =
        "user.#"; // subscribe to any topic whose first tag is "user", followed by any number of fragments
var topicSubscription2 = "*.europe.*"; // subscribe to any topic with three tags, the second being "europe"
var topicSubscription3 = "#.payments"; // subscribe to any topic whose last tag is "payments"


var broker = builder.AddRabbitMQ("Broker")
    .WithManagementPlugin();

var routingExchangeName = "RoutingTopic";
var publisher = builder.AddProject<RoutingTopic_Publisher>("Routing-Publisher")
    .WithEnvironment("TOPICS", string.Join(",", topics))
    .WithEnvironment("EXCHANGE_NAME", routingExchangeName)
    .WithReference(broker)
    .WaitFor(broker);

var subscriber1 = builder.AddProject<RoutingTopic_Subscriber>("Routing-Consumer-1")
    .WithEnvironment("EXCHANGE_NAME", routingExchangeName)
    .WithEnvironment("TOPIC", topicSubscription1)
    .WithReference(broker)
    .WaitFor(broker);

var subscriber2 = builder.AddProject<RoutingTopic_Subscriber>("Routing-Consumer-2")
    .WithEnvironment("EXCHANGE_NAME", routingExchangeName)
    .WithEnvironment("TOPIC", topicSubscription2)
    .WithReference(broker)
    .WaitFor(broker);

var subscriber3 = builder.AddProject<RoutingTopic_Subscriber>("Routing-Consumer-3")
    .WithEnvironment("EXCHANGE_NAME", routingExchangeName)
    .WithEnvironment("TOPIC", topicSubscription3)
    .WithReference(broker)
    .WaitFor(broker);

var app = builder.Build();
await app.RunAsync();