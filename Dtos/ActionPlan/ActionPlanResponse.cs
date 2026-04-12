namespace RiskManagement.Dtos.ActionPlan;

public class ActionPlanResponse
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public int? RiskId { get; set; }
    public int? IncidentId { get; set; }
    public int? OwnerUserId { get; set; }
    public string? SuggestedAction { get; set; }
    public DateTime? PlannedCompletionDate { get; set; }
    public string? ActionPlanStatus { get; set; }
    public string? FollowUp { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}