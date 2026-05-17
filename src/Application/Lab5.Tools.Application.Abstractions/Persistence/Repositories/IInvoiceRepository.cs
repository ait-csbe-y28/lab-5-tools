using Lab5.Tools.Application.Abstractions.Persistence.Queries;
using Lab5.Tools.Application.Models;

namespace Lab5.Tools.Application.Abstractions.Persistence.Repositories;

public interface IInvoiceRepository
{
    Task Create(Invoice invoice, CancellationToken cancellationToken);

    Task Update(Invoice invoice, CancellationToken cancellationToken);

    IAsyncEnumerable<Invoice> Query(
        GetInvoiceQuery query,
        CancellationToken cancellationToken);
}
