namespace Lab5.Tools.Application.Abstractions.Events;

public interface IApprovalEventPublisher
{
    Task Publish(IReadOnlyList<ApprovalInvoiceEvent> approvalInvoiceEvent, CancellationToken cancellationToken);
}