namespace RiskManagement.Dtos.RiskAssessment;

public class UpdateRiskAssessmentRequest
{
    public int? RiskId { get; set; }
    public string? Notes { get; set; }
    public string? RiskPhase { get; set; }
    public int? Likelihood { get; set; }
    public int? Impact { get; set; }
    public int? RiskScore { get; set; }
    public decimal? EconomicalLoss { get; set; }
    public string? RiskMitigation { get; set; }
    public string? RiskTransfer { get; set; }
    public string? RiskAvoidance { get; set; }
    public string? RiskAcceptance { get; set; }
}
