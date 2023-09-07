using CareerHub.Application.Interfaces;
using CareerHub.Domain.Entities.User;
using CareerHub.Shared.Protos;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CareerHub.Application.Features.Authentication.Commands.AuthenticateUser
{
    public record AuthenticateUserCommand(UserLoginRequest Input) : IRequest<UserResponse>
    {
    }

    internal sealed class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, UserResponse>
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

        public async Task<UserResponse> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            UserModel? user = await _userRepository.GetWhereAsync(u => u.NormalizedEmail == request.Input.Email.ToUpperInvariant());

            if (user is null || _passwordHasher.VerifyHashedPassword(user, user.Password, request.Input.Password) != PasswordVerificationResult.Success)
            {
                throw new Exception("Invalid Credentials!");
            }

            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user, cancellationToken);

            return user.Adapt<UserResponse>();
        }
    }
}
