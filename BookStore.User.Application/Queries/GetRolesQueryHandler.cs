using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Queries;

public class GetRolesQueryHandler: IRequestHandler<GetRolesQuery,UserProfileResponse>
{
    private readonly IIdentityManageService _identityManageService;

    public GetRolesQueryHandler(IIdentityManageService identityManageService)
    {
        _identityManageService = identityManageService;
    }

    public async Task<UserProfileResponse> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var result = await _identityManageService.GetProfileAsync(request.UserId);
        return result;
    }
}