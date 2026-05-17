using Lab5.Tools.Application.Abstractions.Integrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Users.Grpc.Contracts;

namespace Lab5.Tools.Infrastructure.Integrations.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAccountsServiceIntegration(this IServiceCollection collection)
    {
        collection.AddOptions<ClientOptions>().BindConfiguration("ServiceUrl");
        collection.AddGrpcClient<UserService.UserServiceClient>((provider, o) =>
        {
            IOptions<ClientOptions> options = provider.GetRequiredService<IOptions<ClientOptions>>();
            o.Address = new Uri(options.Value.BaseUrl);
        });

        collection.AddScoped<IUserServiceClient, UserServiceClient>();

        return collection;
    }
}