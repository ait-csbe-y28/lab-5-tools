using Grpc.Core;
using Invoices.Grpc.Contracts;
using Lab5.Tools.Application.Contracts.Invoices;
using System.Diagnostics;

namespace Lab5.Tools.Presentation.Grpc.Controllers;

public sealed class InvoiceController : InvoiceService.InvoiceServiceBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public override async Task<ProtoApproveInvoiceResponse> ApproveInvoice(
        ProtoApproveInvoiceRequest request,
        ServerCallContext context)
    {
        var approveInvoiceRequest = new ApproveInvoice.Request(request.InvoiceId, request.UserId);

        ApproveInvoice.Result result = await _invoiceService.Approve(
            approveInvoiceRequest,
            context.CancellationToken);

        return result switch
        {
            ApproveInvoice.Result.NotFound notFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                notFound.Message)),
            ApproveInvoice.Result.NotAssigned notAssigned => throw new RpcException(new Status(
                StatusCode.FailedPrecondition,
                notAssigned.Message)),
            ApproveInvoice.Result.InvalidState invalidState => throw new RpcException(new Status(
                StatusCode.FailedPrecondition,
                invalidState.Message)),
            ApproveInvoice.Result.Success success => new ProtoApproveInvoiceResponse(),
            _ => throw new UnreachableException(),
        };
    }

    public override async Task<ProtoDeclineInvoiceResponse> DeclineInvoice(
        ProtoDeclineInvoiceRequest request,
        ServerCallContext context)
    {
        var declineInvoiceRequest = new DeclineInvoice.Request(request.InvoiceId, request.UserId);

        DeclineInvoice.Result result = await _invoiceService.Decline(
            declineInvoiceRequest,
            context.CancellationToken);

        return result switch
        {
            DeclineInvoice.Result.NotFound notFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                notFound.Message)),
            DeclineInvoice.Result.NotAssigned notAssigned => throw new RpcException(new Status(
                StatusCode.FailedPrecondition,
                notAssigned.Message)),
            DeclineInvoice.Result.InvalidState invalidState => throw new RpcException(new Status(
                StatusCode.FailedPrecondition,
                invalidState.Message)),
            DeclineInvoice.Result.Success success => new ProtoDeclineInvoiceResponse(),
            _ => throw new UnreachableException(),
        };
    }

    public override async Task<AssignAccountantResponse> AssignAccountant(
        AssignAccountantRequest request,
        ServerCallContext context)
    {
        var assignAccountantRequest = new AssignAccountant.Request(request.InvoiceId, request.UserId);

        AssignAccountant.Result result = await _invoiceService.AssignAccountant(
            assignAccountantRequest,
            context.CancellationToken);

        return result switch
        {
            AssignAccountant.Result.AlreadyAssigned alreadyAssigned => throw new RpcException(new Status(
                StatusCode.FailedPrecondition,
                alreadyAssigned.InvoiceId.ToString())),
            AssignAccountant.Result.InvoiceNotFound invoiceNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                invoiceNotFound.InvoiceId.ToString())),
            AssignAccountant.Result.InvoiceNotPending invoiceNotPending => throw new RpcException(new Status(
                StatusCode.FailedPrecondition,
                $"Invoice with id {invoiceNotPending.InvoiceId} is not pending")),
            AssignAccountant.Result.Success success => new AssignAccountantResponse(),
            AssignAccountant.Result.UserNotExist userNotExist => throw new RpcException(new Status(
                StatusCode.NotFound,
                userNotExist.UserId.ToString())),
            _ => throw new UnreachableException(),
        };
    }
}