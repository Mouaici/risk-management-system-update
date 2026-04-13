using System;

namespace RiskManagement.Models;

public class ActionPlan
{
    public int Id { get; set; }
    public int? RiskId { get; set; }
    public int? IncidentId { get; set; }
    public int? OwnerUserId { get; set; }
    public int OrganizationId { get; set; }
    public string? SuggestedAction { get; set; }
    public DateTime? PlannedCompletionDate { get; set; }
    public string? ActionPlanStatus { get; set; }
    public string? FollowUp { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Risk? Risk { get; set; }
    public Incident? Incident { get; set; }
    public User? OwnerUser { get; set; }
    public Organization Organization { get; set; } = null!;
}
