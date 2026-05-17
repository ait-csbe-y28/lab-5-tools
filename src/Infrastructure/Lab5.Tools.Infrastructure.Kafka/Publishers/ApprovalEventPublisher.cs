using ApprovalResult.Kafka.Contracts;
using Itmo.Dev.Platform.Kafka.Producer;
using Lab5.Tools.Application.Abstractions.Events;
using Lab5.Tools.Infrastructure.Kafka.Extensions;

namespace Lab5.Tools.Infrastructure.Kafka.Publishers;

internal sealed class ApprovalEventPublisher : IApprovalEventPublisher
{
    private readonly IKafkaMessageProducer<ApprovalResultKey, ApprovalResultValue> _producer;

    public ApprovalEventPublisher(IKafkaMessageProducer<ApprovalResultKey, ApprovalResultValue> producer)
    {
        _producer = producer;
    }

    public async Task Publish(
        IReadOnlyList<ApprovalInvoiceEvent> approvalInvoiceEvent,
        CancellationToken cancellationToken)
    {
        IAsyncEnumerable<KafkaProducerMessage<ApprovalResultKey, ApprovalResultValue>> messages = approvalInvoiceEvent
            .Select(ev => ev.ToMessage())
            .ToAsyncEnumerable();

        await _producer.ProduceAsync(messages, cancellationToken);
    }
}