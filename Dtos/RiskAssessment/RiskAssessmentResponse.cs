namespace RiskManagement.Dtos.RiskAssessment;

public class RiskAssessmentResponse
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public int RiskId { get; set; }
    public int AssessedByUserId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string RiskPhase { get; set; } = string.Empty;
    public int Likelihood { get; set; }
    public int Impact { get; set; }
    public int RiskScore { get; set; }
    public string EconomicalLoss { get; set; } = string.Empty;
    public string RiskMitigation { get; set; } = string.Empty;
    public string RiskTransfer { get; set; } = string.Empty;
    public string RiskAvoidance { get; set; } = string.Empty;
    public string RiskAcceptance { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
