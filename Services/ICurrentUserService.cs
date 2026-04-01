namespace RiskManagement.Services;

public interface ICurrentUserService
{
    int GetRequiredUserId();
    int GetRequiredOrganizationId();
    string GetRequiredRole();
}

