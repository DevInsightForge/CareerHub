using CareerHub.Application.Interfaces;
using CareerHub.Application.Utilities;
using CareerHub.Domain.Entities.User;
using CareerHub.Domain.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CareerHub.Application.Features.Authentication.Commands.RegisterUser;

public sealed record RegisterUserCommand : IRequest<TokenModel>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

internal sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, TokenModel>
{
    private readonly IGenericRepository<UserModel> _userRepository;
    private readonly IPasswordHasher<UserModel> _passwordHasher;
    private readonly TokenServices _jwtService;

    public RegisterUserCommandHandler(
        IGenericRepository<UserModel> userRepository,
        IPasswordHasher<UserModel> passwordHasher,
        TokenServices jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<TokenModel> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        UserModel user = UserModel.CreateUser(request.Email);
        user.SetPassword(_passwordHasher.HashPassword(user, request.Password));

        await _userRepository.AddAsync(user, cancellationToken);

        TokenModel tokenModel = new()
        {
            AccessToken= _jwtService.GenerateJwtToken(user)
        };

        return tokenModel;
    }
}
