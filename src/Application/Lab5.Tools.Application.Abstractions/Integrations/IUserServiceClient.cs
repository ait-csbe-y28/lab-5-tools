using Lab5.Tools.Application.Models.ValueObjects;

namespace Lab5.Tools.Application.Abstractions.Integrations;

public interface IUserServiceClient
{
    Task<bool> IsUserExists(UserId userId, CancellationToken cancellationToken);
}