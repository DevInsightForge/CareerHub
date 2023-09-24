using CareerHub.Application.Interfaces;
using CareerHub.Infrastructure.DataAccess;
using CareerHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CareerHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure DbContext with SQLite provider
        services.AddDbContext<DatabaseContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("SqliteDatabase")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        return services;
    }
}
