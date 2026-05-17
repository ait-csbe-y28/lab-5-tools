namespace Lab5.Tools.Application.Contracts.Accounts;

public interface IAccountService
{
    Task<CreateAccount.Result> Create(CreateAccount.Request request, CancellationToken cancellationToken);
}