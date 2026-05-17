namespace Lab5.Tools.Application.Models.ValueObjects;

public sealed record InvoiceId
{
    public InvoiceId(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value, "InvoiceId");
        Value = value;
    }

    public long Value { get; init; }

    public static InvoiceId Default => new(0);
}