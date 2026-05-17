namespace Lab5.Tools.Application.Models;

public abstract record InvoiceChangeStatusResult
{
    public sealed record Success() : InvoiceChangeStatusResult;

    public sealed record InvalidState() : InvoiceChangeStatusResult;
}