using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using MediatR;
using CareerHub.Application.Behaviours;
using CareerHub.Domain.Entities.User;
using CareerHub.Application.Utilities;
using Microsoft.Extensions.Configuration;
using CareerHub.Application.Configurations;

namespace CareerHub.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
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

            // Inject Configuration Models
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // Inject password hasher
            services.AddScoped<IPasswordHasher<UserModel>, PasswordHasher<UserModel>>();
            services.AddScoped<TokenServices>();

            return services;
        }
    }
}
