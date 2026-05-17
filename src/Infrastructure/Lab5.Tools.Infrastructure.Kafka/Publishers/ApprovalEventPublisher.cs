using Itmo.Dev.Platform.Kafka.Producer;
using Lab5.Tools.Application.Abstractions.Events;
using Lab5.Tools.Infrastructure.Kafka.Extensions;

namespace Lab5.Tools.Infrastructure.Kafka.Publishers;

internal sealed class ApprovalEventPublisher : IApprovalEventPublisher
{
    private readonly IKafkaMessageProducer<ProtoApprovalResultKey, ProtoApprovalResultValue> _producer;

    public ApprovalEventPublisher(IKafkaMessageProducer<ProtoApprovalResultKey, ProtoApprovalResultValue> producer)
    {
        _producer = producer;
    }

    public async Task Publish(
        IReadOnlyList<ApprovalInvoiceEvent> approvalInvoiceEvent,
        CancellationToken cancellationToken)
    {
        IAsyncEnumerable<KafkaProducerMessage<ProtoApprovalResultKey, ProtoApprovalResultValue>> messages = approvalInvoiceEvent
            .Select(ev => ev.ToMessage())
            .ToAsyncEnumerable();

        await _producer.ProduceAsync(messages, cancellationToken);
    }
}