namespace Lab5.Tools.Application.Contracts.Invoices;

public static class AssignAccountant
{
    public sealed record Request(long InvoiceId, long UserId);

    public abstract record Result
    {
        public sealed record Success() : Result;

        public sealed record AlreadyAssigned(long InvoiceId, long UserId) : Result;

        public sealed record InvoiceNotPending(long InvoiceId, long UserId) : Result;

        public sealed record InvoiceNotFound(long InvoiceId) : Result;

        public sealed record UserNotExist(long UserId) : Result;
    }
}