using BookStore.User.Application.Queries;

namespace BookStore.User.Application.Interfaces;


public record ConfirmEmailResult(bool Success); 

public interface IPostAuthService
{

    Task<Guid> RegisterAsync(string email, string password);
    Task<ConfirmEmailResult> ConfirmEmailAsync(Guid userId, string tokenValue);
    public Task<(string email, string token)?> PrepareResetAsync(string email);
    public Task<bool> ResetPassword(string email, string encodedToken, string password);
    Task ChangePasswordAsync(Guid userId,string userName, string password);
    public Task<UserProfileResponse> GetProfileAsync(Guid userId);
    public Task EnableTwoFactor(Guid userId);
}