using BookStore.User.Application.Queries;

namespace BookStore.User.Application.Interfaces;

public interface IIdentityManageService
{
    Task ChangePasswordAsync(Guid userId,string userName, string password);
    public Task<UserProfileResponse> GetProfileAsync(Guid userId);

}