using Accounts.Kafka.Contracts;
using Itmo.Dev.Platform.Kafka.Consumer;
using Lab5.Tools.Application.Contracts.Accounts;
using Lab5.Tools.Application.Models.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Lab5.Tools.Presentation.Kafka.Handlers;

public sealed class CreateAccountHandler : IKafkaConsumerHandler<AccountCreationKey, AccountCreationValue>
{
    private readonly IAccountService _accountService;
    private readonly ILogger<CreateAccountHandler> _logger;

    public CreateAccountHandler(IAccountService accountService, ILogger<CreateAccountHandler> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaConsumerMessage<AccountCreationKey, AccountCreationValue>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaConsumerMessage<AccountCreationKey, AccountCreationValue> message in messages)
        {
            if (IsNeedToSkip(message))
                continue;

            var request = new CreateAccount.Request(
                new UserId(message.Value.UserId),
                new AccountId(message.Value.AccountId));

            CreateAccount.Result result = await _accountService.Create(request, cancellationToken);
            HandleResult(result);
        }
    }

    private void HandleResult(CreateAccount.Result result)
    {
        if (result is CreateAccount.Result.Failure failure)
        {
            _logger.LogError("Error during creating account: {Failure}", failure);
        }
    }

    private bool IsNeedToSkip(IKafkaConsumerMessage<AccountCreationKey, AccountCreationValue> message)
    {
        return message.Value.AccountType != AccountType.Corporate;
    }
}