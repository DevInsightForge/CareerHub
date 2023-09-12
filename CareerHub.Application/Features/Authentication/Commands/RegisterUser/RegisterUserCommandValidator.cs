using CareerHub.Application.Interfaces;
using CareerHub.Domain.Entities.User;
using FluentValidation;

namespace CareerHub.Application.Features.Authentication.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly IGenericRepository<UserModel> _repository;
    public RegisterUserCommandValidator(IGenericRepository<UserModel> repository)
    {
        _repository = repository;

        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MustAsync(BeUniqueEmail).WithMessage("Email is already in use.");

        RuleFor(command => command.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"\d").WithMessage("Password must contain at least one digit.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _repository.AnyAsync(u => u.Email == email);
    }
}
