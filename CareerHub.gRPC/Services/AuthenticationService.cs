using CareerHub.Application.Features.Authentication.Commands.AuthenticateUser;
using CareerHub.Application.Features.Authentication.Commands.RegisterUser;
using CareerHub.Shared.Protos;
using Grpc.Core;
using MediatR;

namespace CareerHub.gRPC.Services
{
    public class AuthenticationService : Authentication.AuthenticationBase
    {
        private readonly ISender _sender;
        public AuthenticationService(ISender sender)
        {
            _sender = sender;
        }
        public override async Task<UserResponse> RegisterUser(UserRegistrationRequest request, ServerCallContext context)
        {
            return await _sender.Send(new RegisterUserCommand(request));
        }

        public override async Task<UserResponse> AuthenticateUser(UserLoginRequest request, ServerCallContext context)
        {
            return await _sender.Send(new AuthenticateUserCommand(request));
        }
    }
}
