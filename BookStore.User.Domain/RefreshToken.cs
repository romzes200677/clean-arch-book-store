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
        
    }
    public void IsExpired()
    {
        
    }
}