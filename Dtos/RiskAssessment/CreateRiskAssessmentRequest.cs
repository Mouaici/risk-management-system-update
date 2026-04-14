namespace RiskManagement.Dtos.RiskAssessment;

public class CreateRiskAssessmentRequest
{
    public int RiskId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string RiskPhase { get; set; } = string.Empty;
    public int Likelihood { get; set; }
    public int Impact { get; set; }
    public int RiskScore { get; set; }
    public decimal EconomicalLoss { get; set; }
    public string RiskMitigation { get; set; } = string.Empty;
    public string RiskTransfer { get; set; } = string.Empty;
    public string RiskAvoidance { get; set; } = string.Empty;
    public string RiskAcceptance { get; set; } = string.Empty;
}
