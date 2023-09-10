using CareerHub.Application.Features.Authentication.Commands.AuthenticateUser;
using CareerHub.Application.Features.Authentication.Commands.RegisterUser;
using CareerHub.gRPC.Protos;
using Grpc.Core;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace CareerHub.gRPC.Services
{
    [AllowAnonymous]
    public class AuthenticationService : Authentication.AuthenticationBase
    {
        private readonly ISender _sender;
        public AuthenticationService(ISender sender)
        {
            _sender = sender;
        }
        public override async Task<TokenResponse> RegisterUser(UserRegistrationRequest registrationRequest, ServerCallContext context)
        {
            var request = registrationRequest.Adapt<RegisterUserCommand>();
            var token = await _sender.Send(request);
            return token.Adapt<TokenResponse>();
        }

        public override async Task<TokenResponse> AuthenticateUser(UserLoginRequest loginRequest, ServerCallContext context)
        {
            var request = loginRequest.Adapt<AuthenticateUserCommand>();
            var token = await _sender.Send(request);
            return token.Adapt<TokenResponse>();
        }
    }
}
