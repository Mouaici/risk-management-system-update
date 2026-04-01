namespace RiskManagement.Services;

public class TokenResponse
{
    public required string AccessToken { get; set; }
    public required DateTime AccessTokenExpiresAt { get; set; }
}

