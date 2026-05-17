using Lab5.Tools.Application.Models.ValueObjects;

namespace Lab5.Tools.Application.Contracts.Invoices;

public static class ApproveInvoice
{
    public sealed record Request(InvoiceId InvoiceId, UserId UserId);

    public abstract record Result
    {
        public sealed record Success() : Result;

        public sealed record NotFound(string Message) : Result;

        public sealed record NotAssigned(string Message) : Result;

        public sealed record InvalidState(string Message) : Result;
    }
}