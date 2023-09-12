using CareerHub.Application.Features.Authentication.Commands.AuthenticateUser;
using CareerHub.Application.Features.Authentication.Commands.RegisterUser;
using CareerHub.Application.Features.Authentication.Queries.GetTokenUserInfo;
using CareerHub.gRPC.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace CareerHub.gRPC.Services;

public class AuthenticationService : Authentication.AuthenticationBase
{
    private readonly ISender _sender;
    public AuthenticationService(ISender sender)
    {
        _sender = sender;
    }

    [AllowAnonymous]
    public override async Task<TokenResponse> RegisterUser(UserRegistrationRequest registrationRequest, ServerCallContext context)
    {
        var request = registrationRequest.Adapt<RegisterUserCommand>();
        var token = await _sender.Send(request, context.CancellationToken);
        return token.Adapt<TokenResponse>();
    }

    [AllowAnonymous]
    public override async Task<TokenResponse> AuthenticateUser(UserLoginRequest loginRequest, ServerCallContext context)
    {
        var request = loginRequest.Adapt<AuthenticateUserCommand>();
        var token = await _sender.Send(request, context.CancellationToken);
        return token.Adapt<TokenResponse>();
    }

    public override async Task<UserResponse> GetTokenUserInfo(Empty empty, ServerCallContext context)
    {
        var request = new GetTokenUserInfoQuery();
        var user = await _sender.Send(request, context.CancellationToken);
        return user.Adapt<UserResponse>();
    }
}
