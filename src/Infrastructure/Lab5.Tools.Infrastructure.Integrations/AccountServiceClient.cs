using Lab5.Tools.Application.Abstractions.Integrations;
using Lab5.Tools.Application.Models.ValueObjects;
using Users.Grpc.Contracts;

namespace Lab5.Tools.Infrastructure.Integrations;

public sealed class AccountServiceClient : IAccountServiceClient
{
    private readonly UserService.UserServiceClient _client;

    public AccountServiceClient(UserService.UserServiceClient client)
    {
        _client = client;
    }

    public async Task<bool> IsUserExists(UserId userId, CancellationToken cancellationToken)
    {
        var request = new GetUserByIdRequest
        {
            UserId = userId.Value,
        };

        GetUserByIdResponse response = await _client.IsExistsAsync(request, cancellationToken: cancellationToken);
        return response.IsExists;
    }
}