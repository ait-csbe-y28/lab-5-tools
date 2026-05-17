using FluentMigrator;

namespace Lab5.Tools.Infrastructure.Persistence.Migrations;

[Migration(202605170002)]
public sealed class CreateInvoicesTable : Migration
{
    public override void Up()
    {
        Execute.Sql(
            """
            create type invoice_status as enum ('pending', 'approved', 'declined');

            create table invoices
            (
                id            bigint         not null primary key,
                amount        decimal        not null,
                recipient_id  bigint         not null,
                payer_id      bigint         not null,
                accountant_id bigint         null,
                status        invoice_status not null default 'pending',

                constraint fk_invoices_recipient foreign key (recipient_id) references accounts (account_id),
                constraint fk_invoices_payer     foreign key (payer_id)     references accounts (account_id)
            );
            """);
    }

    public override void Down()
    {
        Execute.Sql(
            """
            DROP TABLE IF EXISTS invoices;
            DROP TYPE  IF EXISTS invoice_status;
            """);
    }
}
