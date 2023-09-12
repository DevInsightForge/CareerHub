using CareerHub.gRPC.Extensions;

namespace CareerHub.gRPC;

public static class DependencyInjection
{
    public static IServiceCollection AddgRPCServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Authentication
        services.AddAuthenticationService(configuration);

        // Add services to the container.
        services.AddGrpc(options => {
            options.Interceptors.Add<ExceptionInterceptor>();
        });
        services.AddGrpcReflection();

        // Configure Mapster
        MappingServiceExtension.ConfigureMappings();

        return services;
    }
}
