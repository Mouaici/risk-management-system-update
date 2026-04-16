using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskManagement.Dtos.Risk;
using RiskManagement.Models;
using RiskManagement.Services;

namespace RiskManagement.Controllers;

[ApiController]
[Route("api/risks")]
[Authorize]
public class RiskController(RiskManagementDbContext context, ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RiskResponse>>> GetRisks(
        [FromQuery] string? status,
        [FromQuery] int? ownerUserId)
    {
        var organizationId = currentUserService.GetRequiredOrganizationId();

        var query = context.Risks
            .AsNoTracking()
            .Where(r => r.OrganizationId == organizationId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(r => r.Status == status);
        }

        if (ownerUserId.HasValue)
        {
            query = query.Where(r => r.OwnerUserId == ownerUserId.Value);
        }


        var risks = await query
            .OrderByDescending(r => r.UpdatedAt)
            .Select(r => new RiskResponse
            {
                Id = r.Id,
                OrganizationId = r.OrganizationId,
                Title = r.Title,
                Description = r.Description,
                OwnerUserId = r.OwnerUserId,
                AssetId = r.AssetId,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .ToListAsync();

        return Ok(risks);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RiskResponse>> GetRiskById(int id)
    {
        var organizationId = currentUserService.GetRequiredOrganizationId();

        var risk = await context.Risks
            .AsNoTracking()
            .Where(r => r.OrganizationId == organizationId && r.Id == id)
            .Select(r => new RiskResponse
            {
                Id = r.Id,
                OrganizationId = r.OrganizationId,
                Title = r.Title,
                Description = r.Description,
                OwnerUserId = r.OwnerUserId,
                AssetId = r.AssetId,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (risk is null)
        {
            return NotFound();
        }

        return Ok(risk);
    }

    [HttpPost]
    public async Task<ActionResult<RiskResponse>> CreateRisk([FromBody] CreateRiskRequest request)
    {
        var organizationId = currentUserService.GetRequiredOrganizationId();
        var ownerUserId = await ValidateOwnerAsync(organizationId, request.OwnerUserId);
        if (request.OwnerUserId.HasValue && ownerUserId is null)
        {
            return BadRequest("Owner user must belong to the same organization.");
        }
        var assetId = await ValidateAssetAsync(organizationId, request.AssetId);
        if (request.AssetId.HasValue && assetId is null)
        {
            return BadRequest("Asset must belong to the same organization.");
        }

        var now = DateTime.UtcNow;
        var risk = new Risk
        {
            OrganizationId = organizationId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            OwnerUserId = ownerUserId,
            AssetId = assetId,
            Status = request.Status,
            CreatedAt = now,
            UpdatedAt = now
        };

        context.Risks.Add(risk);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRiskById), new { id = risk.Id }, Map(risk));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RiskResponse>> UpdateRisk(int id, [FromBody] UpdateRiskRequest request)
    {
        var organizationId = currentUserService.GetRequiredOrganizationId();
        var risk = await context.Risks.FirstOrDefaultAsync(r => r.Id == id && r.OrganizationId == organizationId);
        if (risk is null)
        {
            return NotFound();
        }

        var ownerUserId = await ValidateOwnerAsync(organizationId, request.OwnerUserId);
        if (request.OwnerUserId.HasValue && ownerUserId is null)
        {
            return BadRequest("Owner user must belong to the same organization.");
        }
        var assetId = await ValidateAssetAsync(organizationId, request.AssetId);
        if (request.AssetId.HasValue && assetId is null)
        {
            return BadRequest("Asset must belong to the same organization.");
        }

        risk.Title = request.Title.Trim();
        risk.Description = request.Description?.Trim();
        risk.OwnerUserId = ownerUserId;
        risk.AssetId = assetId;
        risk.Status = request.Status;
        risk.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return Ok(Map(risk));
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<RiskResponse>> PatchRiskStatus(int id, [FromBody] PatchRiskStatusRequest request)
    {
        var organizationId = currentUserService.GetRequiredOrganizationId();
        var risk = await context.Risks.FirstOrDefaultAsync(r => r.Id == id && r.OrganizationId == organizationId);
        if (risk is null)
        {
            return NotFound();
        }

        risk.Status = request.Status;
        risk.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return Ok(Map(risk));
    }

    private async Task<int?> ValidateOwnerAsync(int organizationId, int? ownerUserId)
    {
        if (!ownerUserId.HasValue)
        {
            return null;
        }

        var exists = await context.Users.AnyAsync(u => u.Id == ownerUserId.Value && u.OrganizationId == organizationId);
        return exists ? ownerUserId : null;
    }

    private async Task<int?> ValidateAssetAsync(int organizationId, int? assetId)
    {
        if (!assetId.HasValue)
        {
            return null;
        }

        var exists = await context.Assets.AnyAsync(a => a.Id == assetId.Value && a.OrganizationId == organizationId);
        return exists ? assetId : null;
    }

    private static RiskResponse Map(Risk risk)
    {
        return new RiskResponse
        {
            Id = risk.Id,
            OrganizationId = risk.OrganizationId,
            Title = risk.Title,
            Description = risk.Description,
            OwnerUserId = risk.OwnerUserId,
            AssetId = risk.AssetId,
            Status = risk.Status,
            CreatedAt = risk.CreatedAt,
            UpdatedAt = risk.UpdatedAt
        };
    }
}

