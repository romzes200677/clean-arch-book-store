namespace BookStore.User.Application.Interfaces;

public interface IIdentityRecoveryService
{
    public Task<bool> ResetPassword(Guid userId, string token, string password);
}