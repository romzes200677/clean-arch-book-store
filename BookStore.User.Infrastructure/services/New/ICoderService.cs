using System.Text;
using BookStore.User.Application.Interfaces.@new;
using Microsoft.AspNetCore.WebUtilities;

namespace BookStore.User.Infrastructure.services.New;

public class CoderService : ICoderService
{
    public string DecodeToken(string encodedBase64String)
    {
        byte[] decodedTokenBytes = WebEncoders.Base64UrlDecode(encodedBase64String);
        string originalToken = Encoding.UTF8.GetString(decodedTokenBytes);
        return originalToken;
    }

    public string EncodeToken(string token)
    {
        throw new NotImplementedException();
    }
}