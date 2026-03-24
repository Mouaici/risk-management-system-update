using RiskManagement.Models;

namespace RiskManagement.Services;

public interface ITokenService
{
    TokenResponse CreateAccessToken(User user);
    string GenerateRefreshToken();
    string HashRefreshToken(string rawRefreshToken);
}

