using FluentMigrator;

namespace Lab5.Tools.Infrastructure.Persistence.Migrations;

[Migration(202605170001)]
public sealed class CreateAccountsTable : Migration
{
    public override void Up()
    {
        Execute.Sql(
            """
            create table accounts
            (
                account_id bigint not null primary key,
                user_id    bigint not null
            );
            """);
    }

    public override void Down()
    {
        Execute.Sql("drop table if exists accounts;");
    }
}
