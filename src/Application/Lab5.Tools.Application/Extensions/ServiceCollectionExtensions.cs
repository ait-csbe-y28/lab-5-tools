using Lab5.Tools.Application.Contracts.Accounts;
using Lab5.Tools.Application.Contracts.Invoices;
using Lab5.Tools.Application.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Lab5.Tools.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddScoped<IAccountService, AccountService>();
        collection.AddScoped<IInvoiceService, InvoiceService>();
        return collection;
    }
}