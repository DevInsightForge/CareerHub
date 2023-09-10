using CareerHub.Application.Features.Authentication.Common;
using CareerHub.Application.Interfaces;
using CareerHub.Domain.Entities.User;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CareerHub.Application.Features.Authentication.Commands.AuthenticateUser
{
    public record AuthenticateUserCommand : IRequest<TokenModel>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    internal sealed class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, TokenModel>
    {
        private readonly IGenericRepository<UserModel> _userRepository;
        private readonly IPasswordHasher<UserModel> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthenticateUserCommandHandler(
            IGenericRepository<UserModel> userRepository,
            IPasswordHasher<UserModel> passwordHasher,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<TokenModel> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            UserModel? user = await _userRepository.GetWhereAsync(u => u.NormalizedEmail == request.Email.ToUpperInvariant());

            if (user is null || _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password) != PasswordVerificationResult.Success)
            {
                throw new Exception("Invalid Credentials!");
            }

            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user, cancellationToken);

            TokenModel tokenModel = new()
            {
                AccessToken = JwtService.GenerateJwtToken(user, _configuration)
            };

            return tokenModel;
        }
    }
}
