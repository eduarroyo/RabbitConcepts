using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var broker = builder.AddRabbitMQ("Broker")
    .WithManagementPlugin();

var publisherSubscriberExchangeName = "PublisherSubscriber";
var publisher = builder.AddProject<PublisherSubscriber_Publisher>("PublisherSubscriber-Publisher")
    .WithEnvironment("EXCHANGE_NAME", publisherSubscriberExchangeName)
    .WithReference(broker)
    .WaitFor(broker);

_ = builder.AddProject<PublisherSubscriber_Subscriber>("PublisherSubscriber-Subscriber1")
    .WithEnvironment("EXCHANGE_NAME", publisherSubscriberExchangeName)
    .WithReference(broker)
    .WaitFor(publisher);

_ = builder.AddProject<PublisherSubscriber_Subscriber>("PublisherSubscriber-Subscriber2")
    .WithEnvironment("EXCHANGE_NAME", publisherSubscriberExchangeName)
    .WithReference(broker)
    .WaitFor(publisher);

_ = builder.AddProject<PublisherSubscriber_Subscriber>("PublisherSubscriber-Subscriber3")
    .WithEnvironment("EXCHANGE_NAME", publisherSubscriberExchangeName)
    .WithReference(broker)
    .WaitFor(publisher);

var app = builder.Build();

await app.RunAsync();