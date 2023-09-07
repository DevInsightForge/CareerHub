using CareerHub.Application.Interfaces;
using CareerHub.Domain.Entities.User;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CareerHub.Application.Features.Authentication.Commands.RegisterUser
{
    public partial record RegisterUserCommand : IRequest<UserModel>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    internal sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserModel>
    {
        private readonly IGenericRepository<UserModel> _userRepository;
        private readonly IPasswordHasher<UserModel> _passwordHasher;

        public RegisterUserCommandHandler(
            IGenericRepository<UserModel> userRepository,
            IPasswordHasher<UserModel> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserModel> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            UserModel? existingUser = await _userRepository.GetWhereAsync(u => u.NormalizedEmail == request.Email.ToUpperInvariant());

            if (existingUser is not null)
                throw new Exception($"User already exists with this email {request.Email}");

            UserModel user = UserModel.CreateUser(request.Email);
            user.SetPassword(_passwordHasher.HashPassword(user, request.Password));

            await _userRepository.AddAsync(user, cancellationToken);

            return user;
        }
    }
}
