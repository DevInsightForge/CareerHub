namespace CareerHub.gRPC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddgRPCServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpc();
            services.AddGrpcReflection();

            return services;
        }
    }
}
