using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RiskManagement.Models;

namespace RiskManagement.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    private const int MinSecretByteLength = 32;

    private readonly string _issuer = configuration["Jwt:Issuer"]
        ?? throw new InvalidOperationException("Missing JWT issuer configuration.");
    private readonly string _audience = configuration["Jwt:Audience"]
        ?? throw new InvalidOperationException("Missing JWT audience configuration.");
    private readonly string _secret = configuration["Jwt:Secret"]
        ?? throw new InvalidOperationException("Missing JWT secret configuration.");

    private readonly int _accessTokenMinutes = int.TryParse(configuration["Jwt:AccessTokenMinutes"], out var minutes)
        ? minutes
        : 30;

    public TokenResponse CreateAccessToken(User user)
    {
        var secretByteCount = Encoding.UTF8.GetByteCount(_secret);
        if (secretByteCount < MinSecretByteLength)
        {
            throw new InvalidOperationException(
                $"Jwt:Secret must be at least {MinSecretByteLength} bytes for HS256. Current size: {secretByteCount} bytes.");
        }

        var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_accessTokenMinutes);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("organizationId", user.OrganizationId.ToString())
        };

        var jwtToken = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: accessTokenExpiresAt,
            signingCredentials: credentials);

        return new TokenResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            AccessTokenExpiresAt = accessTokenExpiresAt
        };
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public string HashRefreshToken(string rawRefreshToken)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawRefreshToken));
        return Convert.ToHexString(hashBytes);
    }
}

