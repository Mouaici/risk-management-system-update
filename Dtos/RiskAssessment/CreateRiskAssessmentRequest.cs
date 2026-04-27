using System.ComponentModel.DataAnnotations;

namespace RiskManagement.Dtos.RiskAssessment;

public class CreateRiskAssessmentRequest
{
    public int RiskId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string RiskPhase { get; set; } = string.Empty;
    [Range(1, 5)]
    public int Likelihood { get; set; }
    [Range(1, 5)]
    public int Impact { get; set; }
    [Required]
    [RegularExpression("^(Low|Medium|High)$",
        ErrorMessage = "EconomicalLoss must be Low, Medium, or High")]
    public string EconomicalLoss { get; set; } = null!;
    public string RiskMitigation { get; set; } = string.Empty;
    public string RiskTransfer { get; set; } = string.Empty;
    public string RiskAvoidance { get; set; } = string.Empty;
    public string RiskAcceptance { get; set; } = string.Empty;
}
