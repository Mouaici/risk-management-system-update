using RiskManagement.Models;

namespace RiskManagement.Services;

public interface IPasswordService
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string password, string passwordHash);
    bool VerifyPasswordOrUpgradeLegacy(User user, string password, string storedPassword, out string? upgradedHash);
}

