using Itmo.Dev.Platform.Persistence.Postgres.Plugins;
using Lab5.Tools.Application.Models;
using Npgsql;

namespace Lab5.Tools.Infrastructure.Persistence.Plugins;

/// <summary>
///     Plugin for configuring NpgsqlDataSource's mappings
///     ie: enums, composite types
/// </summary>
public class MappingPlugin : IPostgresDataSourcePlugin
{
    public void Configure(NpgsqlDataSourceBuilder dataSource)
    {
        dataSource.MapEnum<InvoiceStatus>("invoice_status");
    }
}