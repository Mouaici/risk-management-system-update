namespace RiskManagement.Models;

public class RiskAssessment
{
    public int Id { get; set; }

    public required int RiskId { get; set; }
    public required int AssessedByUserId { get; set; }
    public required int OrganizationId { get; set; }

    public required string Notes { get; set; }
    public required string RiskPhase { get; set; }
    public required int Likelihood { get; set; }
    public required int Impact { get; set; }
    public int RiskScore { get; set; }
    public required decimal EconomicalLoss { get; set; }
    public required string RiskMitigation { get; set; }
    public required string RiskTransfer { get; set; }
    public required string RiskAvoidance { get; set; }
    public required string RiskAcceptance { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Risk Risk { get; set; } = null!;
    public User AssessedByUser { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
}

