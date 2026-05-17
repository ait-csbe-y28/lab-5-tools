using Lab5.Tools.Application.Models.ValueObjects;

namespace Lab5.Tools.Application.Contracts.Invoices;

public static class CreateInvoice
{
    public sealed record Request(
        InvoiceId Id,
        Money Amount,
        AccountId RecipientId,
        AccountId PayerId);

    public abstract record Result
    {
        public sealed record Success() : Result;

        public sealed record AccountNotFound(string Message) : Result;
    }
}