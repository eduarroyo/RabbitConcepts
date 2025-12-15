using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var paymentsBindingKey = "payments";
var analyticsBindingKey = "analytics";
var bothBindingKey = "both";

var broker = builder.AddRabbitMQ("Broker")
    .WithManagementPlugin();

var routingExchhangeName = "Routing";
var publisher = builder.AddProject<Routing_Publisher>("Routing-Publisher")
    .WithEnvironment("BINDING_KEYS", string.Join(",", analyticsBindingKey, paymentsBindingKey, bothBindingKey))
    .WithEnvironment("EXCHANGE_NAME", routingExchhangeName)
    .WithReference(broker)
    .WaitFor(broker);

var analyticsSubscriber = builder.AddProject<Routing_Subscriber>("Routing-Subscriber-Analytics")
    .WithEnvironment("EXCHANGE_NAME", routingExchhangeName)
    .WithEnvironment("BINDING_KEYS", string.Join(",", analyticsBindingKey, bothBindingKey))
    .WithReference(broker)
    .WaitFor(broker);

var paymentsSubscriber = builder.AddProject<Routing_Subscriber>("Routing-Subscriber-Payments")
    .WithEnvironment("EXCHANGE_NAME", routingExchhangeName)
    .WithEnvironment("BINDING_KEYS", string.Join(",", paymentsBindingKey, bothBindingKey))
    .WithReference(broker)
    .WaitFor(broker);

var app = builder.Build();
await app.RunAsync();