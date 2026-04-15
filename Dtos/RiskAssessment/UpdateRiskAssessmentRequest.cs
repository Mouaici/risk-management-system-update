using System.ComponentModel.DataAnnotations;

namespace RiskManagement.Dtos.RiskAssessment;

public class UpdateRiskAssessmentRequest
{
    public string? Notes { get; set; }
    public string? RiskPhase { get; set; }

    [Range(1, 5)]
    public int? Likelihood { get; set; }
    [Range(1, 5)]
    public int? Impact { get; set; }

    [RegularExpression("^(Low|Medium|High)$")]
    public string? EconomicalLoss { get; set; }
    public string? RiskMitigation { get; set; }
    public string? RiskTransfer { get; set; }
    public string? RiskAvoidance { get; set; }
    public string? RiskAcceptance { get; set; }
}
