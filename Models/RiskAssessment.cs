namespace RiskManagement.Models;

public class RiskAssessment
{
    public int Id { get; set; }

    public int RiskId { get; set; }
    public int AssessedByUserId { get; set; }
    public int OrganizationId { get; set; }

    public string? Notes { get; set; }
    public string? RiskPhase { get; set; }
    public int? Likelihood1To5 { get; set; }
    public int? Impact1To5 { get; set; }
    public int? RiskScore { get; set; }
    public decimal? EconomicalLoss { get; set; }
    public string? RiskMitigation { get; set; }
    public string? RiskTransfer { get; set; }
    public string? RiskAvoidance { get; set; }
    public string? RiskAcceptance { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Risk Risk { get; set; } = null!;
    public User AssessedByUser { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
}

