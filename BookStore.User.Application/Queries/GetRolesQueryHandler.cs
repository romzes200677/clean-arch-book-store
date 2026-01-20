using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Queries;

public class GetRolesQueryHandler: IRequestHandler<GetRolesQuery,UserProfileResponse>
{
    private readonly IPostAuthService  _postAuthService;

    public GetRolesQueryHandler(IPostAuthService postAuthService)
    {
        _postAuthService = postAuthService;
    }


    public async Task<UserProfileResponse> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var result = await _postAuthService.GetProfileAsync(request.UserId);
        return result;
    }
}