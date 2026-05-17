using Lab5.Tools.Application.Models.ValueObjects;

namespace Lab5.Tools.Application.Abstractions.Events;

public sealed record ApprovalInvoiceEvent(InvoiceId InvoiceId, ApprovalStatus Status);