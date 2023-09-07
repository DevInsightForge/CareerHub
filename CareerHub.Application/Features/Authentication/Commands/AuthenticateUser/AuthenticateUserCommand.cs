using CareerHub.Application.Interfaces;
using CareerHub.Domain.Entities.User;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CareerHub.Application.Features.Authentication.Commands.AuthenticateUser
{
    public record AuthenticateUserCommand : IRequest<UserModel>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    internal sealed class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, UserModel>
    {
        private readonly IGenericRepository<UserModel> _userRepository;
        private readonly IPasswordHasher<UserModel> _passwordHasher;

        public AuthenticateUserCommandHandler(
            IGenericRepository<UserModel> userRepository,
            IPasswordHasher<UserModel> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserModel> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            UserModel? user = await _userRepository.GetWhereAsync(u => u.NormalizedEmail == request.Email.ToUpperInvariant());

            if (user is null || _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password) != PasswordVerificationResult.Success)
            {
                throw new Exception("Invalid Credentials!");
            }

            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user, cancellationToken);

            return user;
        }
    }
}
