using Lab5.Tools.Presentation.Grpc.Controllers;
using Microsoft.AspNetCore.Builder;

namespace Lab5.Tools.Presentation.Grpc.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UsePresentationGrpc(this IApplicationBuilder builder)
    {
        builder.UseEndpoints(routeBuilder =>
        {
            routeBuilder.MapGrpcService<InvoiceController>();
            routeBuilder.MapGrpcReflectionService();
        });

        return builder;
    }
}