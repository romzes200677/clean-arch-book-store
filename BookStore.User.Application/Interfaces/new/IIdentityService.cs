using BookStore.User.Application.Dto;
using BookStore.User.Application.Queries;
using SharedKernel.BaseRsponse;

namespace BookStore.User.Application.Interfaces.@new;

public interface IIdentityService
{
    public Task<BaseAuthResult> CheckAuthData(string email, string password);
    public Task<string> GetProvider(Guid UserId);
    public Task<BaseAuthResult> ConfirmTwoFactorAsync(Guid UserId, string code, string provider);
    public Task<string> GenerateTokenForEmail(Guid userId);
    public Task<(Guid userId, string email, string token)> GenerateTwoFaToken(Guid userId, string provider);
    public Task<BaseOperationResult<bool>> EnableTwoFactor(Guid userId);
    public Task<BaseOperationResult<Guid>> RegisterAsync(string email, string password);
    public Task<BaseOperationResult<bool>> ConfirmEmailAsync(Guid userId, string tokenValue);
    public Task<bool> ResetPassword(string email, string encodedToken, string password);
    public Task<(string email, string token)?> PrepareResetAsync(string email);
  
    public Task ChangePasswordAsync(Guid userId,string userName, string password);
    public Task<UserProfileResponse> GetProfileAsync(Guid userId);

}