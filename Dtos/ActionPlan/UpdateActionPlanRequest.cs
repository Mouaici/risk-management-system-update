using System.ComponentModel.DataAnnotations;

namespace RiskManagement.Dtos.ActionPlan;

public class UpdateActionPlanRequest
{
    public int? RiskId { get; set; }
    public int? IncidentId { get; set; }
    public int? OwnerUserId { get; set; }
    
    public string? SuggestedAction { get; set; }
    public DateTime? PlannedCompletionDate { get; set; }
    
    [RegularExpression("^(Open|InProgress|Closed)$",
     ErrorMessage = "Status must be Open, InProgress, or Closed")]
    public string? ActionPlanStatus { get; set; } 
    public string? FollowUp { get; set; }
    public string? Notes { get; set; }
}