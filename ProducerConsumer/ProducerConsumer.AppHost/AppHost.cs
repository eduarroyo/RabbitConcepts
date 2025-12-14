using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var broker = builder.AddRabbitMQ("Broker")
    .WithManagementPlugin();

#region Producer-Consumer

IResourceBuilder<ProjectResource> producer = builder.AddProject<ProducerConsumer_Producer>("ProducerConsumer-Producer")
    .WithEnvironment("QUEUE_NAME", "pc-letterbox")
    .WithReference(broker)
    .WaitFor(broker);

IResourceBuilder<ProjectResource> consumer = builder.AddProject<ProducerConsumer_Consumer>("ProducerConsumer-Consumer")
    .WithEnvironment("QUEUE_NAME", "pc-letterbox")
    .WithReference(broker)
    .WaitFor(producer);

#endregion

var app = builder.Build();

await app.RunAsync();