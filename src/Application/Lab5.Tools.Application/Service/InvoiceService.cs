using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using Lab5.Tools.Application.Abstractions.Events;
using Lab5.Tools.Application.Abstractions.Integrations;
using Lab5.Tools.Application.Abstractions.Persistence;
using Lab5.Tools.Application.Abstractions.Persistence.Queries;
using Lab5.Tools.Application.Contracts.Invoices;
using Lab5.Tools.Application.Models;
using Lab5.Tools.Application.Models.Results;
using Lab5.Tools.Application.Models.ValueObjects;
using System.Data;

namespace Lab5.Tools.Application.Service;

internal sealed class InvoiceService : IInvoiceService
{
    private readonly IPersistenceContext _persistenceContext;
    private readonly IApprovalEventPublisher _approvalEventPublisher;
    private readonly IPersistenceTransactionProvider _transactionProvider;
    private readonly IUserServiceClient _client;

    public InvoiceService(
        IPersistenceContext persistenceContext,
        IApprovalEventPublisher approvalEventPublisher,
        IUserServiceClient client,
        IPersistenceTransactionProvider transactionProvider)
    {
        _persistenceContext = persistenceContext;
        _approvalEventPublisher = approvalEventPublisher;
        _transactionProvider = transactionProvider;
        _client = client;
    }

    public async Task<CreateInvoice.Result> Create(CreateInvoice.Request request, CancellationToken cancellationToken)
    {
        var getRecipientAccountQuery =
            GetAccountQuery.Build(builder => builder.WithId(new AccountId(request.RecipientId)));

        Account? recipientAccount = await _persistenceContext.Accounts
            .Query(getRecipientAccountQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (recipientAccount is null)
            return new CreateInvoice.Result.AccountNotFound($"Account with id: {request.RecipientId} not found");

        var getPayerAccountQuery = GetAccountQuery.Build(builder => builder.WithId(new AccountId(request.PayerId)));

        Account? payerAccount = await _persistenceContext.Accounts
            .Query(getPayerAccountQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (payerAccount is null)
            return new CreateInvoice.Result.AccountNotFound($"Account with id: {request.PayerId} not found");

        var invoice = new Invoice(
            new InvoiceId(request.Id),
            new Money(request.Amount),
            new AccountId(request.RecipientId),
            new AccountId(request.PayerId));

        await _persistenceContext.Invoices.Create(invoice, cancellationToken);

        return new CreateInvoice.Result.Success();
    }

    public async Task<AssignAccountant.Result> AssignAccountant(
        AssignAccountant.Request request,
        CancellationToken cancellationToken)
    {
        bool isExists = await IsUserExists(new UserId(request.UserId), cancellationToken);

        if (!isExists)
            return new AssignAccountant.Result.UserNotExist(request.UserId);

        var query = GetInvoiceQuery.Build(builder
            => builder.WithId(new InvoiceId(request.InvoiceId)));

        Invoice? invoice = await _persistenceContext.Invoices
            .Query(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (invoice is null)
            return new AssignAccountant.Result.InvoiceNotFound(request.InvoiceId);

        AssignAccountantResult result = invoice.AssignAccountant(new UserId(request.UserId));

        if (result is AssignAccountantResult.InvoiceNotPending)
            return new AssignAccountant.Result.InvoiceNotPending(request.InvoiceId, request.UserId);

        if (result is AssignAccountantResult.AlreadyAssigned)
            return new AssignAccountant.Result.AlreadyAssigned(request.InvoiceId, request.UserId);

        await _persistenceContext.Invoices.Update(invoice, cancellationToken);

        return new AssignAccountant.Result.Success();
    }

    public async Task<ApproveInvoice.Result> Approve(
        ApproveInvoice.Request request,
        CancellationToken cancellationToken)
    {
        var query = GetInvoiceQuery.Build(builder
            => builder.WithId(new InvoiceId(request.InvoiceId)));

        Invoice? invoice = await _persistenceContext.Invoices
            .Query(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (invoice is null)
            return new ApproveInvoice.Result.NotFound($"Invoice with id: {request.InvoiceId} not found");

        if (invoice.AccountantId == null || invoice.AccountantId.Value != request.UserId)
            return new ApproveInvoice.Result.NotAssigned($"Invoice with id: {request.InvoiceId} doesn't have accountant with accountant id: {request.UserId}");

        InvoiceChangeStatusResult result = invoice.Approve();

        if (result is InvoiceChangeStatusResult.InvalidState)
            return new ApproveInvoice.Result.InvalidState($"Invoice with id: {request.InvoiceId} is not pending");

        await using IPersistenceTransaction transaction = await _transactionProvider.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        await _persistenceContext.Invoices.Update(invoice, cancellationToken);

        var @event = new ApprovalInvoiceEvent(new InvoiceId(request.InvoiceId), ApprovalStatus.Approved);
        await _approvalEventPublisher.Publish([@event], cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new ApproveInvoice.Result.Success();
    }

    public async Task<DeclineInvoice.Result> Decline(
        DeclineInvoice.Request request,
        CancellationToken cancellationToken)
    {
        var query = GetInvoiceQuery.Build(builder
            => builder.WithId(new InvoiceId(request.InvoiceId)));

        Invoice? invoice = await _persistenceContext.Invoices
            .Query(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (invoice is null)
            return new DeclineInvoice.Result.NotFound($"Invoice with id: {request.InvoiceId} not found");

        if (invoice.AccountantId == null || invoice.AccountantId.Value != request.UserId)
            return new DeclineInvoice.Result.NotAssigned($"Invoice with id: {request.InvoiceId} doesn't have accountant with accountant id: {request.UserId}");

        InvoiceChangeStatusResult result = invoice.Decline();

        if (result is InvoiceChangeStatusResult.InvalidState)
            return new DeclineInvoice.Result.InvalidState($"Invoice with id: {request.InvoiceId} is not pending");

        await using IPersistenceTransaction transaction = await _transactionProvider.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        await _persistenceContext.Invoices.Update(invoice, cancellationToken);

        var @event = new ApprovalInvoiceEvent(new InvoiceId(request.InvoiceId), ApprovalStatus.Declined);
        await _approvalEventPublisher.Publish([@event], cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new DeclineInvoice.Result.Success();
    }

    private async Task<bool> IsUserExists(UserId userId, CancellationToken cancellationToken)
    {
        bool isExists = await _client.IsUserExists(userId, cancellationToken);
        return isExists;
    }
}