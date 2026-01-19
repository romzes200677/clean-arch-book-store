namespace BookStore.User.Application.Interfaces;

public interface IIdentityManageService
{
    Task ChangePasswordAsync(Guid userId,string userName, string password);
}