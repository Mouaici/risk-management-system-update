using System.ComponentModel.DataAnnotations;

namespace RiskManagement.Dtos.Asset;

public class UpdateAssetRequest
{
    [Required]
    [MinLength(2)]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Definition { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string AssetType { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Classification { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
}

