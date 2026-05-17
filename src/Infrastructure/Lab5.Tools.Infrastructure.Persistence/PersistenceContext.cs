using Lab5.Tools.Application.Abstractions.Persistence;
using Lab5.Tools.Application.Abstractions.Persistence.Repositories;

namespace Lab5.Tools.Infrastructure.Persistence;

public sealed class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(IAccountRepository accounts, IInvoiceRepository invoices)
    {
        Accounts = accounts;
        Invoices = invoices;
    }

    public IAccountRepository Accounts { get; }

    public IInvoiceRepository Invoices { get; }
}