namespace Lab5.Tools.Application.Models.ValueObjects;

public sealed record AccountId
{
    public long Value { get; }

    public AccountId(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value, "AccountId");
        Value = value;
    }
}