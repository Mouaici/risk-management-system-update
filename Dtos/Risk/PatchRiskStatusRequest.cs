using System.ComponentModel.DataAnnotations;

namespace RiskManagement.Dtos.Risk;

public class PatchRiskStatusRequest
{
    [Required]
    [RegularExpression("^(Open|InProgress|Mitigated|Accepted|Closed)$")]
    public string Status { get; set; } = string.Empty;
}

