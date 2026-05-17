using Lab5.Tools.Application.Models.ValueObjects;

namespace Lab5.Tools.Application.Contracts.Invoices;

public static class AssignAccountant
{
    public sealed record Request(InvoiceId InvoiceId, UserId UserId);

    public abstract record Result
    {
        public sealed record Success() : Result;

        public sealed record AlreadyAssigned(InvoiceId InvoiceId, UserId UserId) : Result;

        public sealed record InvoiceNotPending(InvoiceId InvoiceId, UserId UserId) : Result;

        public sealed record InvoiceNotFound(InvoiceId InvoiceId) : Result;

        public sealed record UserNotExist(UserId UserId) : Result;
    }
}