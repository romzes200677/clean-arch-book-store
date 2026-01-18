namespace BookStore.User.Application.Interfaces.Features;

public interface IForgotPassword
{
    public Task<(Guid userId, string token)?> PrepareResetAsync(string email);
}