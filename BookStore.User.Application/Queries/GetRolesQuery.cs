using MediatR;

namespace BookStore.User.Application.Queries;

public record GetRolesQuery(
Guid UserId
    ):IRequest<UserProfileResponse>;
