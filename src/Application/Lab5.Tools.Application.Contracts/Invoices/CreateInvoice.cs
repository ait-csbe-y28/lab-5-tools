namespace Lab5.Tools.Application.Contracts.Invoices;

public static class CreateInvoice
{
    public sealed record Request(
        long Id,
        decimal Amount,
        long RecipientId,
        long PayerId);

    public abstract record Result
    {
        public sealed record Success() : Result;

        public sealed record AccountNotFound(string Message) : Result;
    }
}