using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using Lab5.Tools.Application.Abstractions.Persistence.Queries;
using Lab5.Tools.Application.Abstractions.Persistence.Repositories;
using Lab5.Tools.Application.Models;
using Lab5.Tools.Application.Models.ValueObjects;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Lab5.Tools.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public AccountRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async IAsyncEnumerable<Account> Query(
        GetAccountQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           select account_id, user_id
                           from accounts
                           where cardinality(:Ids) = 0 or account_id = any(:Ids)
                           order by account_id;
                           """;

        IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("Ids", Array.ConvertAll(query.Ids, x => x.Value));

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Account(
                new AccountId(reader.GetInt64("account_id")),
                new UserId(reader.GetInt64("user_id")));
        }
    }

    public async Task Create(Account account, CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into accounts (account_id, user_id)
                           values (:AccountId, :UserId)
                           on conflict (account_id) do nothing;
                           """;

        IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("AccountId", account.Id.Value)
            .AddParameter("UserId", account.UserId.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
