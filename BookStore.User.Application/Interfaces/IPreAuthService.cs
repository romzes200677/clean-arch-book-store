using BookStore.User.Application.Dto;

namespace BookStore.User.Application.Interfaces;
public record CheckAuthResult(Guid userId,bool twoFactorEnabled,string provider);

public interface IPreAuthService
{

    public Task<CheckAuthResult> CheckAuthData(string email, string password);
    public Task<SuccessAuthResult> VerifyCode(Guid UserId, string Code);
    public Task SendConfirmEmail(string email);

}