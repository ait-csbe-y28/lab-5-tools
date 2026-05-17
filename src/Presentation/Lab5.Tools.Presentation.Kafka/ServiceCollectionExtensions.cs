using Itmo.Dev.Platform.Kafka.Configuration;
using Itmo.Dev.Platform.Kafka.Extensions;
using Lab5.Tools.Presentation.Kafka.Handlers;
using Microsoft.Extensions.Configuration;

namespace Lab5.Tools.Presentation.Kafka;

public static class ServiceCollectionExtensions
{
    public static IKafkaConfigurationBuilder AddPresentationKafkaConsumers(
        this IKafkaConfigurationBuilder kafka,
        IConfiguration configuration)
    {
        const string consumerKey = "Presentation:Kafka:Consumers";
        configuration = configuration.GetSection(consumerKey);

        kafka.AddConsumer(b => b
            .WithKey<ProtoAccountCreationKey>()
            .WithValue<ProtoAccountCreationValue>()
            .WithConfiguration(configuration.GetSection("AccountCreated"))
            .DeserializeKeyWithProto()
            .DeserializeValueWithProto()
            .HandleWith<CreateAccountKafkaHandler>());

        kafka.AddConsumer(b => b
            .WithKey<ProtoInvoiceCreationKey>()
            .WithValue<ProtoInvoiceCreationValue>()
            .WithConfiguration(configuration.GetSection("InvoiceCreated"))
            .DeserializeKeyWithProto()
            .DeserializeValueWithProto()
            .HandleWith<CreateInvoiceKafkaHandler>());

        return kafka;
    }
}