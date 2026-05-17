namespace Lab5.Tools.Application.Models.ValueObjects;

public sealed record UserId
{
    public long Value { get; }

    public UserId(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value, "UserId");
        Value = value;
    }
}