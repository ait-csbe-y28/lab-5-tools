using Accounts.Kafka.Contracts;
using Invoices.Kafka.Contracts;
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
            .WithKey<AccountCreationKey>()
            .WithValue<AccountCreationValue>()
            .WithConfiguration(configuration.GetSection("AccountCreation"))
            .DeserializeKeyWithProto()
            .DeserializeValueWithProto()
            .HandleWith<CreateAccountHandler>());

        kafka.AddConsumer(b => b
            .WithKey<InvoiceCreationKey>()
            .WithValue<InvoiceCreationValue>()
            .WithConfiguration(configuration.GetSection("InvoiceCreation"))
            .DeserializeKeyWithProto()
            .DeserializeValueWithProto()
            .HandleWith<CreateInvoiceHandler>());

        return kafka;
    }
}