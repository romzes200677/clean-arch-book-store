using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookStore.User.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BookStore.User.Infrastructure.services;

public class SecurityService: ISecurityService
{
    private readonly IConfiguration _config;

    public SecurityService(IConfiguration config)
    {
        _config = config;
    }

    public List<Claim> BuildClaims(Guid userId,string email, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            // Уникальный идентификатор пользователя (Id)
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        
            // Email пользователя
            new Claim(ClaimTypes.Email, email ?? string.Empty),
        
            // standard JWT claim: Subject (обычно email или id)
            new Claim(JwtRegisteredClaimNames.Sub, email ?? string.Empty),
        
            // Jti (JWT ID) — уникальный идентификатор самого токена. 
            // Помогает защититься от атак повторения (replay attacks).
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Добавляем все роли пользователя в claims
        // Это позволит работать атрибуту [Authorize(Roles = "admin")]
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }
    
    
    public string GenerateJwtToken(List<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT Key is not configured")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}