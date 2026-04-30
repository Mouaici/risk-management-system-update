using System.ComponentModel.DataAnnotations;

namespace RiskManagement.Dtos.Risk;

public class CreateRiskRequest
{
    [Required]
    [MinLength(3)]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public int? OwnerUserId { get; set; }
    public int? AssetId { get; set; }

}

