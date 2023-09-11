using CareerHub.Application.Features.Common.Services;
using CareerHub.Application.Features.Common.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CareerHub.Application.Features.Authentication.Queries.GetTokenUserInfo
{
    public sealed record GetTokenUserInfoQuery : IRequest<TokenUserModel> {}

    internal sealed class GetTokenUserInfoQueryHandler : IRequestHandler<GetTokenUserInfoQuery, TokenUserModel>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly TokenServices _jwtService;
        public GetTokenUserInfoQueryHandler(IHttpContextAccessor contextAccessor, TokenServices jwtService)
        {
            _contextAccessor = contextAccessor;
            _jwtService = jwtService;

        }

        public Task<TokenUserModel> Handle(GetTokenUserInfoQuery request, CancellationToken cancellationToken)
        {
            var claims = _contextAccessor?.HttpContext?.User ?? throw new UnauthorizedAccessException();
            return Task.FromResult(TokenServices.TokenUserFromClaims(claims));
        }

    }
}
