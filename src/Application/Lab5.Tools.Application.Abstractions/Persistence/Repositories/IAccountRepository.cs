using Lab5.Tools.Application.Abstractions.Persistence.Queries;
using Lab5.Tools.Application.Models;

namespace Lab5.Tools.Application.Abstractions.Persistence.Repositories;

public interface IAccountRepository
{
    IAsyncEnumerable<Account> Query(
        GetAccountQuery query,
        CancellationToken cancellationToken);

    Task Create(Account account, CancellationToken cancellationToken);
}
