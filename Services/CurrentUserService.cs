using System.Security.Claims;

namespace RiskManagement.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public int GetRequiredUserId()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? httpContextAccessor.HttpContext?.User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException("Missing user id claim.");

        return int.TryParse(userIdClaim, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("Invalid user id claim.");
    }

    public int GetRequiredOrganizationId()
    {
        var organizationIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue("organizationId")
            ?? throw new UnauthorizedAccessException("Missing organizationId claim.");

        return int.TryParse(organizationIdClaim, out var organizationId)
            ? organizationId
            : throw new UnauthorizedAccessException("Invalid organizationId claim.");
    }

    public string GetRequiredRole()
    {
        return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role)
            ?? throw new UnauthorizedAccessException("Missing role claim.");
    }
}

