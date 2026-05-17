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

public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public InvoiceRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task Create(Invoice invoice, CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into invoices (id, amount, recipient_id, payer_id, accountant_id, status)
                           values (@InvoiceId, @Amount, @RecipientId, @PayerId, @AccountantId, @Status)
                           on conflict (id) do nothing;
                           """;

        IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("InvoiceId", invoice.Id.Value)
            .AddParameter("Amount", invoice.Amount.Value)
            .AddParameter("RecipientId", invoice.RecipientId.Value)
            .AddParameter("PayerId", invoice.PayerId.Value)
            .AddParameter("AccountantId", invoice.AccountantId?.Value)
            .AddParameter("Status", invoice.Status);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task Update(Invoice invoice, CancellationToken cancellationToken)
    {
        const string sql = """
                           update invoices
                           set amount = @Amount,
                               recipient_id = @RecipientId,
                               payer_id = @PayerId,
                               accountant_id = @AccountantId,
                               status = @Status
                           where id = @InvoiceId;
                           """;

        IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("InvoiceId", invoice.Id.Value)
            .AddParameter("Amount", invoice.Amount.Value)
            .AddParameter("RecipientId", invoice.RecipientId.Value)
            .AddParameter("PayerId", invoice.PayerId.Value)
            .AddParameter("AccountantId", invoice.AccountantId?.Value)
            .AddParameter("Status", invoice.Status);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<Invoice> Query(
        GetInvoiceQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           select id, amount, recipient_id, payer_id, accountant_id, status
                           from invoices
                           where
                               (cardinality(:Ids) = 0 or id = any(:Ids))
                               and (cardinality(:RecipientIds) = 0 or recipient_id = any(:RecipientIds))
                               and (cardinality(:PayerIds) = 0 or payer_id = any(:PayerIds))
                               and (cardinality(:Statuses) = 0 or status = any(:Statuses))
                           order by id;
                           """;

        IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("Ids", query.Ids.Select(id => id.Value).ToArray())
            .AddParameter("RecipientIds", query.RecipientIds.Select(id => id.Value).ToArray())
            .AddParameter("PayerIds", query.PayerIds.Select(id => id.Value).ToArray())
            .AddParameter("Statuses", query.Statuses);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            int accountantOrdinal = reader.GetOrdinal("accountant_id");
            UserId? accountantId = reader.IsDBNull(accountantOrdinal)
                ? null
                : new UserId(reader.GetInt64(accountantOrdinal));

            yield return new Invoice(
                id: new InvoiceId(reader.GetInt64("id")),
                amount: new Money(reader.GetDecimal("amount")),
                recipientId: new AccountId(reader.GetInt64("recipient_id")),
                payerId: new AccountId(reader.GetInt64("payer_id")),
                accountantId: accountantId,
                status: reader.GetFieldValue<InvoiceStatus>("status"));
        }
    }
}
