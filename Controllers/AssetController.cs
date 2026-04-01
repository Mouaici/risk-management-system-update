using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskManagement.Dtos.Asset;
using RiskManagement.Models;
using RiskManagement.Services;

namespace RiskManagement.Controllers;

[ApiController]
[Route("api/asset")]
[Authorize(Policy = "AnyAuthenticatedUser")]
public class AssetController(RiskManagementDbContext context, ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AssetResponse>>> GetAllAssets()
    {
        var role = currentUserService.GetRequiredRole();
        var organizationId = currentUserService.GetRequiredOrganizationId();

        var query = context.Assets.AsNoTracking();
        if (!IsSuperadmin(role))
        {
            query = query.Where(a => a.OrganizationId == organizationId);
        }

        var assets = await query
            .OrderBy(a => a.Id)
            .Select(a => new AssetResponse
            {
                Id = a.Id,
                OrganizationId = a.OrganizationId,
                Name = a.Name,
                Definition = a.Definition,
                AssetType = a.AssetType,
                Classification = a.Classification,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync();

        return Ok(assets);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AssetResponse>> GetAssetById(int id)
    {
        var role = currentUserService.GetRequiredRole();
        var organizationId = currentUserService.GetRequiredOrganizationId();

        var asset = await context.Assets
            .AsNoTracking()
            .Where(a => a.Id == id && (IsSuperadmin(role) || a.OrganizationId == organizationId))
            .Select(a => new AssetResponse
            {
                Id = a.Id,
                OrganizationId = a.OrganizationId,
                Name = a.Name,
                Definition = a.Definition,
                AssetType = a.AssetType,
                Classification = a.Classification,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (asset is null)
        {
            return NotFound();
        }

        return Ok(asset);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Superadmin")]
    public async Task<ActionResult<AssetResponse>> CreateAsset([FromBody] CreateAssetRequest request)
    {
        var role = currentUserService.GetRequiredRole();
        var organizationId = currentUserService.GetRequiredOrganizationId();

        var targetOrganizationId = request.OrganizationId ?? organizationId;
        if (!IsSuperadmin(role) && targetOrganizationId != organizationId)
        {
            return Forbid();
        }

        if (IsSuperadmin(role) && request.OrganizationId.HasValue)
        {
            var organizationExists = await context.Organizations
                .AsNoTracking()
                .AnyAsync(o => o.Id == request.OrganizationId.Value);
            if (!organizationExists)
            {
                return BadRequest("Invalid organization.");
            }
        }

        var now = DateTime.UtcNow;
        var asset = new Asset
        {
            OrganizationId = targetOrganizationId,
            Name = request.Name.Trim(),
            Definition = request.Definition?.Trim(),
            AssetType = request.AssetType.Trim(),
            Classification = request.Classification?.Trim(),
            Status = request.Status.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        context.Assets.Add(asset);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAssetById), new { id = asset.Id }, Map(asset));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Superadmin")]
    public async Task<ActionResult<AssetResponse>> UpdateAsset(int id, [FromBody] UpdateAssetRequest request)
    {
        var role = currentUserService.GetRequiredRole();
        var organizationId = currentUserService.GetRequiredOrganizationId();

        var asset = await context.Assets.FirstOrDefaultAsync(a => a.Id == id);
        if (asset is null)
        {
            return NotFound();
        }

        if (!IsSuperadmin(role) && asset.OrganizationId != organizationId)
        {
            return Forbid();
        }

        asset.Name = request.Name.Trim();
        asset.Definition = request.Definition?.Trim();
        asset.AssetType = request.AssetType.Trim();
        asset.Classification = request.Classification?.Trim();
        asset.Status = request.Status.Trim();
        asset.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return Ok(Map(asset));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Superadmin")]
    public async Task<IActionResult> DeleteAsset(int id)
    {
        var asset = await context.Assets.FirstOrDefaultAsync(a => a.Id == id);
        if (asset is null)
        {
            return NotFound();
        }

        context.Assets.Remove(asset);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private static bool IsSuperadmin(string role) => role.Equals("Superadmin", StringComparison.OrdinalIgnoreCase);

    private static AssetResponse Map(Asset asset)
    {
        return new AssetResponse
        {
            Id = asset.Id,
            OrganizationId = asset.OrganizationId,
            Name = asset.Name,
            Definition = asset.Definition,
            AssetType = asset.AssetType,
            Classification = asset.Classification,
            Status = asset.Status,
            CreatedAt = asset.CreatedAt,
            UpdatedAt = asset.UpdatedAt
        };
    }
}