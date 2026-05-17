#pragma warning disable CA1506

using Itmo.Dev.Platform.Common.Extensions;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.MessagePersistence;
using Itmo.Dev.Platform.MessagePersistence.Postgres.Extensions;
using Itmo.Dev.Platform.Observability;
using Lab5.Tools.Application.Extensions;
using Lab5.Tools.Infrastructure.Integrations.Extensions;
using Lab5.Tools.Infrastructure.Kafka.Extensions;
using Lab5.Tools.Infrastructure.Persistence.Extensions;
using Lab5.Tools.Presentation.Grpc.Extensions;
using Lab5.Tools.Presentation.Kafka;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddPlatform(platform => platform
    .WithNewtonsoftSerialization());

builder.AddPlatformObservability();

builder.Services.AddApplication();
builder.Services.AddInfrastructurePersistence();
builder.Services.AddAccountsServiceIntegration();
builder.Services.AddEventPublisher();
builder.Services.AddPresentationGrpc();

builder.Services.AddPlatformKafka(kafka => kafka
    .ConfigureOptions(builder.Configuration.GetSection("Presentation:Kafka"))
    .AddInfrastructureKafkaProducers(builder.Configuration)
    .AddPresentationKafkaConsumers(builder.Configuration));

builder.Services.AddPlatformMessagePersistence(step => step
    .WithDefaultPublisherOptions("MessagePersistence:Publisher:Default")
    .UsePostgresPersistence(configurator => configurator.ConfigureOptions("MessagePersistence:Postgres")));

builder.Services.AddUtcDateTimeProvider();

WebApplication app = builder.Build();

app.UseRouting();

app.UsePlatformObservability();

app.UsePresentationGrpc();

await app.RunAsync();