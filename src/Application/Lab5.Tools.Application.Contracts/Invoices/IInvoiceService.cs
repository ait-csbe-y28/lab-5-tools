namespace Lab5.Tools.Application.Contracts.Invoices;

public interface IInvoiceService
{
    Task<CreateInvoice.Result> Create(CreateInvoice.Request request, CancellationToken cancellationToken);

    Task<AssignAccountant.Result> AssignAccountant(AssignAccountant.Request request, CancellationToken cancellationToken);

    Task<ApproveInvoice.Result> Approve(ApproveInvoice.Request request, CancellationToken cancellationToken);

    Task<DeclineInvoice.Result> Decline(DeclineInvoice.Request request, CancellationToken cancellationToken);
}