using CareerHub.Application.Features.Authentication.Commands.AuthenticateUser;
using CareerHub.Application.Features.Authentication.Commands.RegisterUser;
using CareerHub.gRPC.Protos;
using Grpc.Core;
using Mapster;
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
        public override async Task<UserResponse> RegisterUser(UserRegistrationRequest registrationRequest, ServerCallContext context)
        {
            var request = registrationRequest.Adapt<RegisterUserCommand>();
            var user = await _sender.Send(request);
            return user.Adapt<UserResponse>();
        }

        public override async Task<UserResponse> AuthenticateUser(UserLoginRequest loginRequest, ServerCallContext context)
        {
            var request = loginRequest.Adapt<AuthenticateUserCommand>();
            var user = await _sender.Send(request);
            return user.Adapt<UserResponse>();
        }
    }
}
