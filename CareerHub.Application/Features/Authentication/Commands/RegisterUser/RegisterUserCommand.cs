using CareerHub.Application.Features.Authentication.Common;
using CareerHub.Application.Interfaces;
using CareerHub.Domain.Entities.User;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CareerHub.Application.Features.Authentication.Commands.RegisterUser
{
    public partial record RegisterUserCommand : IRequest<TokenModel>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    internal sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, TokenModel>
    {
        private readonly IGenericRepository<UserModel> _userRepository;
        private readonly IPasswordHasher<UserModel> _passwordHasher;
        private readonly IConfiguration _configuration;

        public RegisterUserCommandHandler(
            IGenericRepository<UserModel> userRepository,
            IPasswordHasher<UserModel> passwordHasher,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<TokenModel> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            UserModel? existingUser = await _userRepository.GetWhereAsync(u => u.NormalizedEmail == request.Email.ToUpperInvariant());

            if (existingUser is not null)
                throw new Exception($"User already exists with this email {request.Email}");

            UserModel user = UserModel.CreateUser(request.Email);
            user.SetPassword(_passwordHasher.HashPassword(user, request.Password));

            await _userRepository.AddAsync(user, cancellationToken);

            TokenModel tokenModel = new()
            {
                AccessToken= JwtService.GenerateJwtToken(user, _configuration)
            };

            return tokenModel;
        }
    }
}
