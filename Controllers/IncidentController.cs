using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskManagement.Dtos.Incident;
using RiskManagement.Models;
using RiskManagement.Services;

namespace RiskManagement.Controllers;

[ApiController]
[Route("api/incidents")]
[Authorize(Policy = "AnyAuthenticatedUser")]
public class IncidentController(RiskManagementDbContext context, ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<IncidentResponse>>> GetIncidents()
    {
        var orgId = currentUserService.GetRequiredOrganizationId();
        var incidents = await context.Incidents
            .AsNoTracking()
            .Where(i => i.OrganizationId == orgId)
            .OrderByDescending(i => i.OccuredOn)
            .ThenByDescending(i => i.UpdatedAt)
            .Select(i => new IncidentResponse
            {
                Id = i.Id,
                OrganizationId = i.OrganizationId,
                ReportedByUserId = i.ReportedByUserId,
                Title = i.Title,
                Severity = i.Severity,
                OccuredOn = i.OccuredOn,
                IncidentStatus = i.IncidentStatus,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .ToListAsync();
        return Ok(incidents);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<IncidentResponse>> GetIncidentById(int id)
    {
        var orgId = currentUserService.GetRequiredOrganizationId();
        var incident = await context.Incidents
            .AsNoTracking()
            .Where(i => i.OrganizationId == orgId && i.Id == id)
            .Select(i => new IncidentResponse
            {
                Id = i.Id,
                OrganizationId = i.OrganizationId,
                ReportedByUserId = i.ReportedByUserId,
                Title = i.Title,
                Severity = i.Severity,
                OccuredOn = i.OccuredOn,
                IncidentStatus = i.IncidentStatus,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .FirstOrDefaultAsync();
        if (incident is null) return NotFound();
        return Ok(incident);
    }

    [HttpPost]
    
    public async Task<ActionResult<IncidentResponse>> CreateIncident([FromBody] CreateIncidentRequest request)
    {
        var orgId = currentUserService.GetRequiredOrganizationId();
        var userId = currentUserService.GetRequiredUserId();
        var now = DateTime.UtcNow;
        var incident = new Incident
        {
            OrganizationId = orgId,
            ReportedByUserId = userId,
            Title = request.Title.Trim(),
            Severity = request.Severity,
            OccuredOn = request.OccuredOn,
            IncidentStatus = request.IncidentStatus,
            CreatedAt = now,
            UpdatedAt = now
        };
        context.Incidents.Add(incident);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetIncidentById), new { id = incident.Id }, Map(incident));
    }

    [HttpPut("{id:int}")]
    
    public async Task<ActionResult<IncidentResponse>> UpdateIncident(int id, [FromBody] UpdateIncidentRequest request)
    {
        var orgId = currentUserService.GetRequiredOrganizationId();
        var incident = await context.Incidents.FirstOrDefaultAsync(i => i.Id == id && i.OrganizationId == orgId);
        if (incident is null) return NotFound();
        incident.Title = request.Title.Trim();
        incident.Severity = request.Severity;
        incident.OccuredOn = request.OccuredOn;
        incident.IncidentStatus = request.IncidentStatus;
        incident.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return Ok(Map(incident));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "SuperadminOnly")]
    public async Task<IActionResult> DeleteIncident(int id)
    {
        var orgId = currentUserService.GetRequiredOrganizationId();
        var incident = await context.Incidents.FirstOrDefaultAsync(i => i.Id == id && i.OrganizationId == orgId);
        if (incident is null) return NotFound();
        context.Incidents.Remove(incident);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private static IncidentResponse Map(Incident i) => new()
    {
        Id = i.Id,
        OrganizationId = i.OrganizationId,
        ReportedByUserId = i.ReportedByUserId,
        Title = i.Title,
        Severity = i.Severity,
        OccuredOn = i.OccuredOn,
        IncidentStatus = i.IncidentStatus,
        CreatedAt = i.CreatedAt,
        UpdatedAt = i.UpdatedAt
    };
}
