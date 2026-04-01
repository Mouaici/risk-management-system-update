using Microsoft.AspNetCore.Identity;
using RiskManagement.Models;

namespace RiskManagement.Services;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string password, string passwordHash)
    {
        var verificationResult = _passwordHasher.VerifyHashedPassword(user, passwordHash, password);
        return verificationResult != PasswordVerificationResult.Failed;
    }

    public bool VerifyPasswordOrUpgradeLegacy(User user, string password, string storedPassword, out string? upgradedHash)
    {
        upgradedHash = null;

        try
        {
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, storedPassword, password);
            return verificationResult != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            // Legacy plaintext password support: authenticate once and immediately upgrade to hash.
            if (!string.Equals(storedPassword, password, StringComparison.Ordinal))
            {
                return false;
            }

            upgradedHash = _passwordHasher.HashPassword(user, password);
            return true;
        }
    }
}

