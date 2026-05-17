namespace Lab5.Tools.Application.Models.Results;

public abstract record AssignAccountantResult
{
    public sealed record Success() : AssignAccountantResult;

    public sealed record AlreadyAssigned() : AssignAccountantResult;

    public sealed record InvoiceNotPending() : AssignAccountantResult;
}