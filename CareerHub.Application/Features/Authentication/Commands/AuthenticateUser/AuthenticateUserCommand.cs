using CareerHub.Application.Features.Common.Services;
using CareerHub.Application.Features.Common.ViewModels;
using CareerHub.Application.Interfaces;
using CareerHub.Domain.Entities.User;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CareerHub.Application.Features.Authentication.Commands.AuthenticateUser
{
    public sealed record AuthenticateUserCommand : IRequest<TokenModel>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    internal sealed class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, TokenModel>
    {
        private readonly IGenericRepository<UserModel> _userRepository;
        private readonly IPasswordHasher<UserModel> _passwordHasher;
        private readonly TokenServices _jwtService;

        public AuthenticateUserCommandHandler(
            IGenericRepository<UserModel> userRepository,
            IPasswordHasher<UserModel> passwordHasher,
            TokenServices jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
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
                AccessToken = _jwtService.GenerateJwtToken(user)
            };

            return tokenModel;
        }
    }
}
