using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Mapster;
using Microsoft.AspNetCore.Identity;
using MediatR;
using CareerHub.Application.Behaviours;
using CareerHub.Application.Configurations;
using CareerHub.Domain.Entities.User;

namespace CareerHub.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Inject and Configure Mediatr
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            });

            // Inject and Configure Fluent Validation
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            // Configure Mapster
            TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);
            MappingConfigurations.ConfigureMappings();

            // Inject password hasher
            services.AddScoped<IPasswordHasher<UserModel>, PasswordHasher<UserModel>>();

            return services;
        }
    }
}
