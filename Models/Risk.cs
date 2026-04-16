namespace RiskManagement.Models;

public class Risk
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int? OwnerUserId { get; set; }
    public int? AssetId { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Organization Organization { get; set; } = null!;
    public User? OwnerUser { get; set; }
    public Asset? Asset { get; set; }
    public List<ActionPlan> ActionPlans { get; set; } = [];
    public List<RiskAssessment> RiskAssessments { get; set; } = [];
}

