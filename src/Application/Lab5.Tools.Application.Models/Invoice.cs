using Lab5.Tools.Application.Models.Results;
using Lab5.Tools.Application.Models.ValueObjects;

namespace Lab5.Tools.Application.Models;

public sealed class Invoice
{
    public InvoiceId Id { get; }

    public Money Amount { get; }

    public AccountId RecipientId { get; }

    public AccountId PayerId { get; }

    public UserId? AccountantId { get; private set; }

    public InvoiceStatus Status { get; private set; }

    public Invoice(
        InvoiceId id,
        Money amount,
        AccountId recipientId,
        AccountId payerId,
        UserId? accountantId = null,
        InvoiceStatus status = InvoiceStatus.Pending)
    {
        Id = id;
        Amount = amount;
        RecipientId = recipientId;
        PayerId = payerId;
        Status = status;
        AccountantId = accountantId;
    }

    public AssignAccountantResult AssignAccountant(UserId accountantId)
    {
        if (AccountantId is not null)
            return new AssignAccountantResult.AlreadyAssigned();

        if (Status != InvoiceStatus.Pending)
            return new AssignAccountantResult.InvoiceNotPending();

        AccountantId = accountantId;
        return new AssignAccountantResult.Success();
    }

    public InvoiceChangeStatusResult Approve()
        => ChangeStatus(InvoiceStatus.Approved);

    public InvoiceChangeStatusResult Decline()
        => ChangeStatus(InvoiceStatus.Declined);

    private InvoiceChangeStatusResult ChangeStatus(InvoiceStatus status)
    {
        if (Status != InvoiceStatus.Pending)
        {
            return new InvoiceChangeStatusResult.InvalidState();
        }

        Status = status;
        return new InvoiceChangeStatusResult.Success();
    }
}