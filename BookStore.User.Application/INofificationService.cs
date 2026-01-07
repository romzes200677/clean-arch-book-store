namespace BookStore.User.Application;

public interface INofificationService
{
    Task NotifyAsync(Guid userId,string token);
}