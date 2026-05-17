using Itmo.Dev.Platform.Kafka.Configuration;
using Itmo.Dev.Platform.Kafka.Extensions;
using Lab5.Tools.Application.Abstractions.Events;
using Lab5.Tools.Infrastructure.Kafka.Publishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lab5.Tools.Infrastructure.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IKafkaConfigurationBuilder AddInfrastructureKafkaProducers(
        this IKafkaConfigurationBuilder kafka,
        IConfiguration configuration)
    {
        const string producerKey = "Presentation:Kafka:Producers";
        configuration = configuration.GetSection(producerKey);

        kafka.AddProducer(b => b
            .WithKey<ProtoApprovalResultKey>()
            .WithValue<ProtoApprovalResultValue>()
            .WithConfiguration(configuration.GetSection("ApprovalResult"))
            .SerializeKeyWithProto()
            .SerializeValueWithProto()
            .WithOutbox());

        return kafka;
    }

    public static IServiceCollection AddEventPublisher(this IServiceCollection collection)
    {
        collection.AddScoped<IApprovalEventPublisher, ApprovalEventPublisher>();
        return collection;
    }
}