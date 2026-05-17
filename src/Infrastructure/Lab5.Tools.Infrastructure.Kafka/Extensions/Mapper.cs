using ApprovalResult.Kafka.Contracts;
using Itmo.Dev.Platform.Kafka.Producer;
using Lab5.Tools.Application.Abstractions.Events;
using ApprovalStatus = Lab5.Tools.Application.Abstractions.Events.ApprovalStatus;

namespace Lab5.Tools.Infrastructure.Kafka.Extensions;

public static class Mapper
{
    public static KafkaProducerMessage<ApprovalResultKey, ApprovalResultValue> ToMessage(
        this ApprovalInvoiceEvent approvalInvoiceEvent)
    {
        return new KafkaProducerMessage<ApprovalResultKey, ApprovalResultValue>(
            Key: new ApprovalResultKey
            {
                InvoiceId = approvalInvoiceEvent.InvoiceId.Value,
            },
            Value: new ApprovalResultValue
            {
                InvoiceId = approvalInvoiceEvent.InvoiceId.Value,
                Status = approvalInvoiceEvent.Status == ApprovalStatus.Approved
                    ? ApprovalResult.Kafka.Contracts.ApprovalStatus.Approved
                    : ApprovalResult.Kafka.Contracts.ApprovalStatus.Declined,
            });
    }
}