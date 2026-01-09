namespace BookStore.User.Application.Interfaces;

public interface INofificationService
{
    Task NotifyAsync(Guid userId,string token);
}