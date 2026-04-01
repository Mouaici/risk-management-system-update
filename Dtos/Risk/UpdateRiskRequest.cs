using System.ComponentModel.DataAnnotations;

namespace RiskManagement.Dtos.Risk;

public class UpdateRiskRequest
{
    [Required]
    [MinLength(3)]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public int? OwnerUserId { get; set; }
    public int? AssetId { get; set; }

    [Required]
    [RegularExpression("^(Open|InProgress|Mitigated|Accepted|Closed)$")]
    public string Status { get; set; } = "Open";

    [Range(1, 5)]
    public int Likelihood { get; set; }

    [Range(1, 5)]
    public int Impact { get; set; }
}

