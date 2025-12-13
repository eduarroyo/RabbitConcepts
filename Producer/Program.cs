using Producer;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>(srv =>
    new ConnectionFactory
    {
        HostName = builder.Configuration["BROKER_HOST"]!,
        Port = builder.Configuration.GetValue<int>("BROKER_PORT"),
        UserName = builder.Configuration["BROKER_USERNAME"]!,
        Password = builder.Configuration["BROKER_PASSWORD"]!
    });
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();