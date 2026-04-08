using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskManagement.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ActionPlanController : ControllerBase
{
    private readonly RiskManagementDbContext _context;

    public ActionPlanController(RiskManagementDbContext context)
    {
        _context = context;
    }

    // GET: api/ActionPlan
    [HttpGet]
    [Authorize(Policy = "AnyAuthenticatedUser")]
    public async Task<IActionResult> GetAll()
    {
        var actionPlans = await _context.ActionPlans
            .Include(a => a.Risk)
            .Include(a => a.Incident)
            .Include(a => a.OwnerUser)
            .ToListAsync();
        return Ok(actionPlans);
    }

    // GET: api/ActionPlan/5
    [HttpGet("{id}")]
    [Authorize(Policy = "AnyAuthenticatedUser")]
    public async Task<IActionResult> Get(int id)
    {
        var actionPlan = await _context.ActionPlans
            .Include(a => a.Risk)
            .Include(a => a.Incident)
            .Include(a => a.OwnerUser)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (actionPlan == null) return NotFound();
        return Ok(actionPlan);
    }

    // POST: api/ActionPlan
    [HttpPost]
    
    public async Task<IActionResult> Create(ActionPlan actionPlan)
    {
        actionPlan.CreatedAt = DateTime.UtcNow;
        actionPlan.UpdatedAt = DateTime.UtcNow;
        _context.ActionPlans.Add(actionPlan);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = actionPlan.Id }, actionPlan);
    }

    // PUT: api/ActionPlan/5
    [HttpPut("{id}")]
    
    public async Task<IActionResult> Update(int id, ActionPlan actionPlan)
    {
        if (id != actionPlan.Id) return BadRequest();
        var existing = await _context.ActionPlans.FindAsync(id);
        if (existing == null) return NotFound();
        // Update fields
        existing.RiskId = actionPlan.RiskId;
        existing.IncidentId = actionPlan.IncidentId;
        existing.OwnerUserId = actionPlan.OwnerUserId;
        existing.SuggestedAction = actionPlan.SuggestedAction;
        existing.PlannedCompletionDate = actionPlan.PlannedCompletionDate;
        existing.ActionPlanStatus = actionPlan.ActionPlanStatus;
        existing.FollowUp = actionPlan.FollowUp;
        existing.Notes = actionPlan.Notes;
        existing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/ActionPlan/5
    [HttpDelete("{id}")]
    [Authorize(Policy = "SuperadminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var actionPlan = await _context.ActionPlans.FindAsync(id);
        if (actionPlan == null) return NotFound();
        _context.ActionPlans.Remove(actionPlan);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
