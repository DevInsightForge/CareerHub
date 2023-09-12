using CareerHub.Application.Utilities;
using CareerHub.Domain.ViewModels;
using MediatR;

namespace CareerHub.Application.Features.Authentication.Queries.GetTokenUserInfo;

public sealed record GetTokenUserInfoQuery : IRequest<TokenUserModel> {}

internal sealed class GetTokenUserInfoQueryHandler : IRequestHandler<GetTokenUserInfoQuery, TokenUserModel>
{
    private readonly TokenServices _tokenService;
    public GetTokenUserInfoQueryHandler(TokenServices tokenServices)
    {
        _tokenService = tokenServices;
    }

    public Task<TokenUserModel> Handle(GetTokenUserInfoQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_tokenService.GetLoggedInUser());
    }

}
