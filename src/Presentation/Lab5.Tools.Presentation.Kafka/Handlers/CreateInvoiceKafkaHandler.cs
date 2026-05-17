using Itmo.Dev.Platform.Kafka.Consumer;
using Lab5.Tools.Application.Contracts.Invoices;
using Microsoft.Extensions.Logging;

namespace Lab5.Tools.Presentation.Kafka.Handlers;

public sealed class CreateInvoiceKafkaHandler : IKafkaConsumerHandler<ProtoInvoiceCreationKey, ProtoInvoiceCreationValue>
{
    private readonly IInvoiceService _invoiceService;
    private readonly ILogger<IKafkaConsumerHandler<ProtoInvoiceCreationKey, ProtoInvoiceCreationValue>> _logger;

    public CreateInvoiceKafkaHandler(
        IInvoiceService invoiceService,
        ILogger<IKafkaConsumerHandler<ProtoInvoiceCreationKey, ProtoInvoiceCreationValue>> logger)
    {
        _invoiceService = invoiceService;
        _logger = logger;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaConsumerMessage<ProtoInvoiceCreationKey, ProtoInvoiceCreationValue>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaConsumerMessage<ProtoInvoiceCreationKey, ProtoInvoiceCreationValue> message in messages)
        {
            if (!Validate(message))
            {
                continue;
            }

            var request = new CreateInvoice.Request(
                Id: message.Value.InvoiceId,
                RecipientId: message.Value.RecipientId,
                PayerId: message.Value.PayerId,
                Amount: message.Value.Payment.DecimalValue);

            CreateInvoice.Result result = await _invoiceService.Create(request, cancellationToken);
            HandleResult(result);
        }
    }

    private bool Validate(IKafkaConsumerMessage<ProtoInvoiceCreationKey, ProtoInvoiceCreationValue> message)
    {
        return message.Value.Payment.DecimalValue >= 0;
    }

    private void HandleResult(CreateInvoice.Result result)
    {
        if (result is CreateInvoice.Result.AccountNotFound failure)
        {
            _logger.LogError(failure.Message);
        }
    }
}