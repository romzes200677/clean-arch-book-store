namespace BookStore.User.Infrastructure.models;

public class RefreshTokenEntity
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiryDate { get;  set; }
    public Guid UserId { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; }

    
}