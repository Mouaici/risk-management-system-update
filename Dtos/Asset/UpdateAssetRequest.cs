using System.ComponentModel.DataAnnotations;

namespace RiskManagement.Dtos.Asset;

public class UpdateAssetRequest
{
    [Required]
    [MinLength(2)]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [MaxLength(2000)]
    public string? Definition { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string AssetType { get; set; } = null!;

    [MaxLength(100)]
    public string? Classification { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    [RegularExpression("^(Open|InProgress|Closed)$",
        ErrorMessage = "Status must be Open, InProgress, or Closed")]
    public string Status { get; set; } = null!;
}

