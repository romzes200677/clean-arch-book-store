namespace BookStore.User.Application.Queries;

public record UserProfileResponse(
    Guid UserId, 
    string Email, 
    List<string> Roles);