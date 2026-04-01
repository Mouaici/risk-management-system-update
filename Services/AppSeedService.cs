using Microsoft.EntityFrameworkCore;
using RiskManagement.Models;

namespace RiskManagement.Services;

public class AppSeedService(
    RiskManagementDbContext context,
    IPasswordService passwordService,
    IConfiguration configuration)
{
    public async Task SeedAsync()
    {
        var seedEnabled = configuration.GetValue("SeedData:Enabled", true);
        if (!seedEnabled)
        {
            return;
        }

        var updateExistingUsers = configuration.GetValue("SeedData:UpdateExistingUsers", true);

        var now = DateTime.UtcNow;

        var organizationName = configuration["SeedData:Organization:Name"] ?? "Demo Organization";
        var organizationIsoScope = configuration["SeedData:Organization:IsoScope"] ?? "ISO27001";
        var organizationStatus = configuration["SeedData:Organization:Status"] ?? "Active";

        var usersToSeed = new[]
        {
            new SeedUser(
                FirstName: configuration["SeedData:User:FirstName"] ?? "Demo",
                LastName: configuration["SeedData:User:LastName"] ?? "User",
                Email: (configuration["SeedData:User:Email"] ?? "demo.user@example.com").Trim().ToLowerInvariant(),
                Password: configuration["SeedData:User:Password"] ?? "ChangeThisPassword123!",
                Role: configuration["SeedData:User:Role"] ?? "User",
                Status: configuration["SeedData:User:Status"] ?? "Active"
            ),
            new SeedUser(
                FirstName: configuration["SeedData:AdminUser:FirstName"] ?? "Demo",
                LastName: configuration["SeedData:AdminUser:LastName"] ?? "Admin",
                Email: (configuration["SeedData:AdminUser:Email"] ?? "demo.admin@example.com").Trim().ToLowerInvariant(),
                Password: configuration["SeedData:AdminUser:Password"] ?? "ChangeThisAdminPassword123!",
                Role: configuration["SeedData:AdminUser:Role"] ?? "Admin",
                Status: configuration["SeedData:AdminUser:Status"] ?? "Active"
            ),
            new SeedUser(
                FirstName: configuration["SeedData:SuperadminUser:FirstName"] ?? "Demo",
                LastName: configuration["SeedData:SuperadminUser:LastName"] ?? "Superadmin",
                Email: (configuration["SeedData:SuperadminUser:Email"] ?? "demo.superadmin@example.com").Trim().ToLowerInvariant(),
                Password: configuration["SeedData:SuperadminUser:Password"] ?? "ChangeThisSuperadminPassword123!",
                Role: configuration["SeedData:SuperadminUser:Role"] ?? "Superadmin",
                Status: configuration["SeedData:SuperadminUser:Status"] ?? "Active"
            )
        };

        var organization = await context.Organizations
            .FirstOrDefaultAsync(x => x.Name == organizationName);

        if (organization is null)
        {
            organization = new Organization
            {
                Name = organizationName,
                IsoScope = organizationIsoScope,
                Status = organizationStatus,
                CreatedAt = now,
                UpdatedAt = now
            };

            context.Organizations.Add(organization);
            await context.SaveChangesAsync();
        }

        foreach (var seedUser in usersToSeed)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(x => x.Email == seedUser.Email);

            if (existingUser is not null)
            {
                if (updateExistingUsers)
                {
                    existingUser.FirstName = seedUser.FirstName;
                    existingUser.LastName = seedUser.LastName;
                    existingUser.Role = seedUser.Role;
                    existingUser.Status = seedUser.Status;
                    existingUser.OrganizationId = organization.Id;
                    existingUser.UpdatedAt = now;

                    var passwordMatches = passwordService.VerifyPassword(existingUser, seedUser.Password, existingUser.PasswordHash);
                    if (!passwordMatches)
                    {
                        existingUser.PasswordHash = passwordService.HashPassword(existingUser, seedUser.Password);
                    }

                    await context.SaveChangesAsync();
                }

                continue;
            }

            var user = new User
            {
                FirstName = seedUser.FirstName,
                LastName = seedUser.LastName,
                Email = seedUser.Email,
                PasswordHash = string.Empty, // set after hash
                Role = seedUser.Role,
                Status = seedUser.Status,
                OrganizationId = organization.Id,
                CreatedAt = now,
                UpdatedAt = now
            };

            user.PasswordHash = passwordService.HashPassword(user, seedUser.Password);
            context.Users.Add(user);
        }

        await context.SaveChangesAsync();
    }

    private readonly record struct SeedUser(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string Role,
        string Status);
}

