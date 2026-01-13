namespace BookStore.User.Domain;

public class RefreshToken
{
    public string Token { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsRevoked { get; private set; }

    public RefreshToken(string token, DateTime expiryDate, Guid userId, bool isRevoked)
    {
        Token = token;
        ExpiryDate = expiryDate;
        UserId = userId;
        IsRevoked = isRevoked;
    }
    

    public void Revoke()
    {
        IsRevoked = true;
    }
    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiryDate;
    }

    public bool IsActive()
    {
        return !IsRevoked &&  !IsExpired();
    }

    public static RefreshToken CreateToken(string token, Guid userId)
    {
        var expiry = DateTime.UtcNow.AddDays(7);
        return new RefreshToken(token, expiry, userId, false);
    }
}