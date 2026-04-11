using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskManagement.Dtos.ActionPlan;
using RiskManagement.Models;
using RiskManagement.Services;

namespace RiskManagement.Controllers;

[ApiController]
[Route("api/action-plans")]
[Authorize(Policy = "AnyAuthenticatedUser")]
public class ActionPlanController(RiskManagementDbContext context, ICurrentUserService currentUserService) : ControllerBase
{

    // GET: api/ActionPlan
    [HttpGet]
    
    public async Task<ActionResult<List<ActionPlanResponse>>> GetAll()
    {
        var orgId = currentUserService.GetRequiredOrganizationId();

        var actionPlans = await context.ActionPlans
            .AsNoTracking()
            .Select(a => new ActionPlanResponse
            {
                Id = a.Id,
                RiskId = a.RiskId,
                IncidentId = a.IncidentId,
                OwnerUserId = a.OwnerUserId,
                SuggestedAction = a.SuggestedAction,
                PlannedCompletionDate = a.PlannedCompletionDate,
                ActionPlanStatus = a.ActionPlanStatus,
                FollowUp = a.FollowUp,
                Notes = a.Notes,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync();

        return Ok(actionPlans);
    }

    // GET: api/ActionPlan/5
    [HttpGet("{id}")]
    
    public async Task<ActionResult<ActionPlanResponse>> Get(int id)
    {
        var orgId = currentUserService.GetRequiredOrganizationId();

        var actionPlan = await context.ActionPlans
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new ActionPlanResponse
            {
                Id = a.Id,
                RiskId = a.RiskId,
                IncidentId = a.IncidentId,
                OwnerUserId = a.OwnerUserId,
                SuggestedAction = a.SuggestedAction,
                PlannedCompletionDate = a.PlannedCompletionDate,
                ActionPlanStatus = a.ActionPlanStatus,
                FollowUp = a.FollowUp,
                Notes = a.Notes,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (actionPlan == null) return NotFound();
        return Ok(actionPlan);
    }

    // POST: api/ActionPlan
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ActionPlanResponse>> Create([FromBody] CreateActionPlanRequest createDto)
    {
        var organizationId = currentUserService.GetRequiredOrganizationId();

        var ownerUserId = await ValidateOwnerAsync(organizationId, createDto.OwnerUserId);
        if (createDto.OwnerUserId.HasValue && ownerUserId is null)
        {
            return BadRequest("Owner user must belong to the same organization.");
        }

        var riskId = await ValidateRiskAsync(organizationId, createDto.RiskId);
        if (createDto.RiskId.HasValue && riskId is null)
        {
            return BadRequest("Risk must belong to the same organization.");
        }

        var incidentId = await ValidateIncidentAsync(organizationId, createDto.IncidentId);
        if (createDto.IncidentId.HasValue && incidentId is null)
        {
            return BadRequest("Incident must belong to the same organization.");
        }

        var entity = new ActionPlan
        {
            RiskId = riskId,
            IncidentId = incidentId,
            OwnerUserId = ownerUserId,
            SuggestedAction = createDto.SuggestedAction,
            PlannedCompletionDate = createDto.PlannedCompletionDate,
            ActionPlanStatus = createDto.ActionPlanStatus,
            FollowUp = createDto.FollowUp,
            Notes = createDto.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.ActionPlans.Add(entity);
        await context.SaveChangesAsync();

        var response = new ActionPlanResponse
        {
            Id = entity.Id,
            RiskId = entity.RiskId,
            IncidentId = entity.IncidentId,
            OwnerUserId = entity.OwnerUserId,
            SuggestedAction = entity.SuggestedAction,
            PlannedCompletionDate = entity.PlannedCompletionDate,
            ActionPlanStatus = entity.ActionPlanStatus,
            FollowUp = entity.FollowUp,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

        return CreatedAtAction(nameof(Get), new { id = entity.Id }, response);
    }

    // PUT: api/ActionPlan/5
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ActionPlanResponse>> Update(int id, [FromBody] UpdateActionPlanRequest updateDto)
    {
        var organizationId = currentUserService.GetRequiredOrganizationId();

        var existing = await context.ActionPlans
            .FirstOrDefaultAsync(a => a.Id == id);

        if (existing == null) return NotFound();

        var ownerUserId = await ValidateOwnerAsync(organizationId, updateDto.OwnerUserId);
        if (updateDto.OwnerUserId.HasValue && ownerUserId is null)
        {
            return BadRequest("Owner user must belong to the same organization.");
        }

        var riskId = await ValidateRiskAsync(organizationId, updateDto.RiskId);
        if (updateDto.RiskId.HasValue && riskId is null)
        {
            return BadRequest("Risk must belong to the same organization.");
        }

        var incidentId = await ValidateIncidentAsync(organizationId, updateDto.IncidentId);
        if (updateDto.IncidentId.HasValue && incidentId is null)
        {
            return BadRequest("Incident must belong to the same organization.");
        }

        existing.RiskId = riskId;
        existing.IncidentId = incidentId;
        existing.OwnerUserId = ownerUserId;
        existing.SuggestedAction = updateDto.SuggestedAction;
        existing.PlannedCompletionDate = updateDto.PlannedCompletionDate;
        existing.ActionPlanStatus = updateDto.ActionPlanStatus;
        existing.FollowUp = updateDto.FollowUp;
        existing.Notes = updateDto.Notes;
        existing.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        var response = new ActionPlanResponse
        {
            Id = existing.Id,
            RiskId = existing.RiskId,
            IncidentId = existing.IncidentId,
            OwnerUserId = existing.OwnerUserId,
            SuggestedAction = existing.SuggestedAction,
            PlannedCompletionDate = existing.PlannedCompletionDate,
            ActionPlanStatus = existing.ActionPlanStatus,
            FollowUp = existing.FollowUp,
            Notes = existing.Notes,
            CreatedAt = existing.CreatedAt,
            UpdatedAt = existing.UpdatedAt
        };

        return Ok(response);
    }

    // DELETE: api/ActionPlan/5
    [HttpDelete("{id}")]
    [Authorize(Policy = "SuperadminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var organizationId = currentUserService.GetRequiredOrganizationId();

        var actionPlan = await context.ActionPlans
            .FirstOrDefaultAsync(a => a.Id == id);

        if (actionPlan == null) return NotFound();

        context.ActionPlans.Remove(actionPlan);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<int?> ValidateOwnerAsync(int organizationId, int? ownerUserId)
    {
        if (!ownerUserId.HasValue) return null;
        var exists = await context.Users.AnyAsync(u => u.Id == ownerUserId.Value && u.OrganizationId == organizationId);
        return exists ? ownerUserId : null;
    }

    private async Task<int?> ValidateRiskAsync(int organizationId, int? riskId)
    {
        if (!riskId.HasValue) return null;
        var exists = await context.Risks.AnyAsync(r => r.Id == riskId.Value && r.OrganizationId == organizationId);
        return exists ? riskId : null;
    }

    private async Task<int?> ValidateIncidentAsync(int organizationId, int? incidentId)
    {
        if (!incidentId.HasValue) return null;
        var exists = await context.Incidents.AnyAsync(i => i.Id == incidentId.Value && i.OrganizationId == organizationId);
        return exists ? incidentId : null;
    }
}
