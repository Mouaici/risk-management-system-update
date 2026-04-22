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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrganizationById(int id)
    {
        var role = currentUserService.GetRequiredRole();
        var currentOrgId = currentUserService.GetRequiredOrganizationId();

        // Only Superadmin has full access
        if (!IsSuperadmin(role) && id != currentOrgId)
        {
            return Forbid();
        }

        var organization = await context.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);

        if (organization is null)
            return NotFound();

        return Ok(organization);
    }

    [HttpPost]
    [Authorize(Policy = "SuperadminOnly")]
    public async Task<ActionResult<Organization>> AddOrganization([FromBody] RiskManagement.Dtos.Organization.CreateOrganizationRequest request)
    {
        var now = DateTime.UtcNow;
        var org = new Organization
        {
            Name = request.Name.Trim(),
            IsoScope = request.IsoScope.Trim(),
            Status = request.Status.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        context.Organizations.Add(org);
        await context.SaveChangesAsync();

        return Ok(org);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "SuperadminOnly")]
    public async Task<ActionResult<Organization>> UpdateOrganization(int id, [FromBody] RiskManagement.Dtos.Organization.UpdateOrganizationRequest request)
    {
        var org = await context.Organizations.FirstOrDefaultAsync(o => o.Id == id);
        if (org is null)
        {
            return NotFound();
        }

        org.Name = request.Name.Trim();
        org.IsoScope = request.IsoScope.Trim();
        org.Status = request.Status.Trim();
        org.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return Ok(org);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "SuperadminOnly")]
    public async Task<IActionResult> DeleteOrganization(int id)
    {
        var org = await context.Organizations.FirstOrDefaultAsync(o => o.Id == id);
        if (org is null)
        {
            return NotFound();
        }

        context.Organizations.Remove(org);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private static bool IsSuperadmin(string role) => role.Equals("Superadmin", StringComparison.OrdinalIgnoreCase);
}
