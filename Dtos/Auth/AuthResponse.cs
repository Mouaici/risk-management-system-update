namespace RiskManagement.Dtos.Auth;

public class AuthResponse
{
    public required string AccessToken { get; set; }
    public required DateTime AccessTokenExpiresAt { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
    public int OrganizationId { get; set; }
}

