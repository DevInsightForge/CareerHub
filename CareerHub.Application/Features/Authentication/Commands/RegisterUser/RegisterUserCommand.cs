using CareerHub.Application.Interfaces;
using CareerHub.Domain.Entities.User;
using CareerHub.Shared.Protos;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CareerHub.Application.Features.Authentication.Commands.RegisterUser
{
    public partial record RegisterUserCommand : UserRegistrationRequest, IRequest<UserResponse>
    {
    }

    internal sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserResponse>
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

        public async Task<UserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            UserModel? existingUser = await _userRepository.GetWhereAsync(u => u.NormalizedEmail == request.Dto.Email.ToUpperInvariant());

            if (existingUser is not null)
                throw new Exception($"User already exists with this email {request.Dto.Email}");

            UserModel user = UserModel.CreateUser(request.Dto.Email);
            user.SetPassword(_passwordHasher.HashPassword(user, request.Dto.Password));

            await _userRepository.AddAsync(user, cancellationToken);

            return user.Adapt<UserResponse>();
        }
    }
}
