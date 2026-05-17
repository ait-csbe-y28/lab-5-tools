namespace Lab5.Tools.Application.Models.ValueObjects;

public sealed record Money
{
    public Money(decimal value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        Value = value;
    }

    public decimal Value { get; }
}