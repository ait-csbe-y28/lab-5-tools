using Itmo.Dev.Platform.Kafka.Producer;
using Lab5.Tools.Application.Abstractions.Events;
using ApprovalStatus = Lab5.Tools.Application.Abstractions.Events.ApprovalStatus;

namespace Lab5.Tools.Infrastructure.Kafka.Extensions;

public static class Mapper
{
    public static KafkaProducerMessage<ProtoApprovalResultKey, ProtoApprovalResultValue> ToMessage(
        this ApprovalInvoiceEvent evt)
    {
        var key = new ProtoApprovalResultKey
        {
            InvoiceId = evt.InvoiceId.Value,
        };

        var value = new ProtoApprovalResultValue
        {
            InvoiceId = evt.InvoiceId.Value,
            Status = evt.Status.MapToProto(),
        };

        return new KafkaProducerMessage<ProtoApprovalResultKey, ProtoApprovalResultValue>(
            Key: key,
            Value: value);
    }

    private static ProtoApprovalStatus MapToProto(this ApprovalStatus approvalStatus)
    {
        return approvalStatus switch
        {
            ApprovalStatus.Approved => ProtoApprovalStatus.Approved,
            ApprovalStatus.Declined => ProtoApprovalStatus.Declined,
            _ => throw new ArgumentOutOfRangeException(nameof(approvalStatus), approvalStatus, null),
        };
    }
}
