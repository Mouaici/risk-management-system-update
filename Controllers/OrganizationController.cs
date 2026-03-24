using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskManagement.Models;
using RiskManagement.Services;

namespace RiskManagement.Controllers;

[ApiController]
[Route("api/organization")]
[Authorize]
public class OrganizationController(RiskManagementDbContext context, ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOrganizations()
    {
        var role = currentUserService.GetRequiredRole();
        var organizationId = currentUserService.GetRequiredOrganizationId();

        if (IsSuperadmin(role))
        {
            var allOrganizations = await context.Organizations.AsNoTracking().ToListAsync();
            return Ok(allOrganizations);
        }

        var organization = await context.Organizations
            .AsNoTracking()
            .Where(x => x.Id == organizationId)
            .ToListAsync();

        return Ok(organization);
    }

    [HttpPost]
    public async Task<IActionResult> AddOrganization()
    {
        var role = currentUserService.GetRequiredRole();
        if (!IsSuperadmin(role))
        {
            return Forbid();
        }

        var now = DateTime.UtcNow;
        var org = new Organization
        {
            Name = "Test Org",
            IsoScope = "ISO9001",
            Status = "Active",
            CreatedAt = now,
            UpdatedAt = now
        };

        context.Organizations.Add(org);
        await context.SaveChangesAsync();

        return Ok(org);
    }

    private static bool IsSuperadmin(string role) => role.Equals("Superadmin", StringComparison.OrdinalIgnoreCase);
}
