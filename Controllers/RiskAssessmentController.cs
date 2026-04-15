using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskManagement.Dtos.RiskAssessment;
using RiskManagement.Models;
using RiskManagement.Services;

namespace RiskManagement.Controllers;

[ApiController]
[Route("api/risk-assessment")]
[Authorize]
public class RiskAssessmentController(RiskManagementDbContext context, ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RiskAssessmentResponse>>> GetAll()
    {
        var organizationId = currentUserService.GetRequiredOrganizationId();

        var riskAssessments = await context.RiskAssessments
            .AsNoTracking()
            .Where(ra => ra.OrganizationId == organizationId)
            .OrderByDescending(ra => ra.UpdatedAt)
            .Select(ra => new RiskAssessmentResponse
            {
                Id = ra.Id,
                OrganizationId = ra.OrganizationId,
                RiskId = ra.RiskId,
                AssessedByUserId = ra.AssessedByUserId,
                Notes = ra.Notes,
                RiskPhase = ra.RiskPhase,
                Likelihood = ra.Likelihood,
                Impact = ra.Impact,
                RiskScore = ra.RiskScore,
                EconomicalLoss = ra.EconomicalLoss,
                RiskMitigation = ra.RiskMitigation,
                RiskTransfer = ra.RiskTransfer,
                RiskAvoidance = ra.RiskAvoidance,
                RiskAcceptance = ra.RiskAcceptance,
                CreatedAt = ra.CreatedAt,
                UpdatedAt = ra.UpdatedAt
            })
            .ToListAsync();

        return Ok(riskAssessments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RiskAssessmentResponse>> Get(int id)
    {
        var organizationId = currentUserService.GetRequiredOrganizationId();

        var riskAssessment = await context.RiskAssessments
            .AsNoTracking()
            .Where(ra => ra.Id == id && ra.OrganizationId == organizationId)
            .Select(ra => new RiskAssessmentResponse
            {
                Id = ra.Id,
                OrganizationId = ra.OrganizationId,
                RiskId = ra.RiskId,
                AssessedByUserId = ra.AssessedByUserId,
                Notes = ra.Notes,
                RiskPhase = ra.RiskPhase,
                Likelihood = ra.Likelihood,
                Impact = ra.Impact,
                RiskScore = ra.RiskScore,
                EconomicalLoss = ra.EconomicalLoss,
                RiskMitigation = ra.RiskMitigation,
                RiskTransfer = ra.RiskTransfer,
                RiskAvoidance = ra.RiskAvoidance,
                RiskAcceptance = ra.RiskAcceptance,
                CreatedAt = ra.CreatedAt,
                UpdatedAt = ra.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (riskAssessment is null)
        {
            return NotFound();
        }

        return Ok(riskAssessment);
    }

    [HttpPost]
    public async Task<ActionResult<RiskAssessmentResponse>> Create([FromBody] CreateRiskAssessmentRequest createDto)
    {
        var organizationId = currentUserService.GetRequiredOrganizationId();
        var assessedByUserId = currentUserService.GetRequiredUserId();

        var isAssessorInOrganization = await ValidateAssessorAsync(organizationId, assessedByUserId);
        if (!isAssessorInOrganization)
        {
            return BadRequest("Authenticated user must belong to the same organization.");
        }

        var riskId = await ValidateRiskAsync(organizationId, createDto.RiskId);
        if (riskId is null)
        {
            return BadRequest("Risk must belong to the same organization.");
        }
        if( createDto.EconomicalLoss.ToLower() != "low" && createDto.EconomicalLoss.ToLower() != "medium" && createDto.EconomicalLoss.ToLower() != "high")
        {
            return BadRequest("Economical loss must be Low, Medium or High.");
        }

        var now = DateTime.UtcNow;
        var entity = new RiskAssessment
        {
            OrganizationId = organizationId,
            RiskId = riskId.Value,
            AssessedByUserId = assessedByUserId,
            Notes = createDto.Notes,
            RiskPhase = createDto.RiskPhase,
            Likelihood = createDto.Likelihood,
            Impact = createDto.Impact,
            RiskScore = createDto.Likelihood * createDto.Impact,
            EconomicalLoss = createDto.EconomicalLoss,
            RiskMitigation = createDto.RiskMitigation,
            RiskTransfer = createDto.RiskTransfer,
            RiskAvoidance = createDto.RiskAvoidance,
            RiskAcceptance = createDto.RiskAcceptance,
            CreatedAt = now,
            UpdatedAt = now
        };

        context.RiskAssessments.Add(entity);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = entity.Id }, Map(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RiskAssessmentResponse>> Update(int id, [FromBody] UpdateRiskAssessmentRequest updateDto)
    {
        var organizationId = currentUserService.GetRequiredOrganizationId();
        var assessedByUserId = currentUserService.GetRequiredUserId();

        var isAssessorInOrganization = await ValidateAssessorAsync(organizationId, assessedByUserId);
        if (!isAssessorInOrganization)
        {
            return BadRequest("Authenticated user must belong to the same organization.");
        }

        var existing = await context.RiskAssessments
            .FirstOrDefaultAsync(ra => ra.Id == id && ra.OrganizationId == organizationId);

        if (existing is null)
        {
            return NotFound();
        }
        if( updateDto.EconomicalLoss is not null && updateDto.EconomicalLoss.ToLower() != "low" && updateDto.EconomicalLoss.ToLower() != "medium" && updateDto.EconomicalLoss.ToLower() != "high")
        {
            return BadRequest("Economical loss must be Low, Medium or High.");
        }

        if (updateDto.Notes is not null) existing.Notes = updateDto.Notes;
        if (updateDto.RiskPhase is not null) existing.RiskPhase = updateDto.RiskPhase;
        if (updateDto.Likelihood.HasValue) existing.Likelihood = updateDto.Likelihood.Value;
        if (updateDto.Impact.HasValue) existing.Impact = updateDto.Impact.Value;
        if (updateDto.EconomicalLoss is not null) existing.EconomicalLoss = updateDto.EconomicalLoss;
        if (updateDto.RiskMitigation is not null) existing.RiskMitigation = updateDto.RiskMitigation;
        if (updateDto.RiskTransfer is not null) existing.RiskTransfer = updateDto.RiskTransfer;
        if (updateDto.RiskAvoidance is not null) existing.RiskAvoidance = updateDto.RiskAvoidance;
        if (updateDto.RiskAcceptance is not null) existing.RiskAcceptance = updateDto.RiskAcceptance;

        existing.RiskScore = existing.Likelihood * existing.Impact;
        existing.AssessedByUserId = assessedByUserId;
        existing.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return Ok(Map(existing));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "SuperadminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var organizationId = currentUserService.GetRequiredOrganizationId();
        var riskAssessment = await context.RiskAssessments
            .FirstOrDefaultAsync(ra => ra.Id == id && ra.OrganizationId == organizationId);

        if (riskAssessment is null)
        {
            return NotFound();
        }

        context.RiskAssessments.Remove(riskAssessment);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<int?> ValidateRiskAsync(int organizationId, int riskId)
    {
        if (riskId <= 0)
        {
            return null;
        }

        var exists = await context.Risks.AnyAsync(r => r.Id == riskId && r.OrganizationId == organizationId);
        return exists ? riskId : null;
    }

    private async Task<bool> ValidateAssessorAsync(int organizationId, int assessedByUserId)
    {
        return await context.Users.AnyAsync(u => u.Id == assessedByUserId && u.OrganizationId == organizationId);
    }

    private static RiskAssessmentResponse Map(RiskAssessment riskAssessment)
    {
        return new RiskAssessmentResponse
        {
            Id = riskAssessment.Id,
            OrganizationId = riskAssessment.OrganizationId,
            RiskId = riskAssessment.RiskId,
            AssessedByUserId = riskAssessment.AssessedByUserId,
            Notes = riskAssessment.Notes,
            RiskPhase = riskAssessment.RiskPhase,
            Likelihood = riskAssessment.Likelihood,
            Impact = riskAssessment.Impact,
            RiskScore = riskAssessment.RiskScore,
            EconomicalLoss = riskAssessment.EconomicalLoss,
            RiskMitigation = riskAssessment.RiskMitigation,
            RiskTransfer = riskAssessment.RiskTransfer,
            RiskAvoidance = riskAssessment.RiskAvoidance,
            RiskAcceptance = riskAssessment.RiskAcceptance,
            CreatedAt = riskAssessment.CreatedAt,
            UpdatedAt = riskAssessment.UpdatedAt
        };
    }
}
