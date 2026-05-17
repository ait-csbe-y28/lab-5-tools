namespace Lab5.Tools.Application.Contracts.Accounts;

public static class CreateAccount
{
    public sealed record Request(long UserId, long AccountId);

    public abstract record Result
    {
        public sealed record Success() : Result;

        public sealed record Failure() : Result;
    }
}