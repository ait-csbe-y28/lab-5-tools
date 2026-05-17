using Lab5.Tools.Application.Abstractions.Persistence;
using Lab5.Tools.Application.Contracts.Accounts;
using Lab5.Tools.Application.Models;
using Microsoft.Extensions.Logging;

namespace Lab5.Tools.Application.Service;

internal sealed class AccountService : IAccountService
{
    private readonly IPersistenceContext _persistenceContext;

    public AccountService(IPersistenceContext persistenceContext, ILogger<IAccountService> logger)
    {
        _persistenceContext = persistenceContext;
    }

    public async Task<CreateAccount.Result> Create(CreateAccount.Request request, CancellationToken cancellationToken)
    {
        var account = new Account(request.AccountId, request.UserId);
        await _persistenceContext.Accounts.Create(account, cancellationToken);
        return new CreateAccount.Result.Success();
    }
}