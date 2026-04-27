using System.ComponentModel.DataAnnotations;

namespace RiskManagement.Dtos.Organization;

public class CreateOrganizationRequest
{
    [Required]
    [MinLength(2)]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    public string IsoScope { get; set; } = string.Empty;

    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    [RegularExpression("^(Active|InActive)$",
        ErrorMessage = "Status must be Active or InActive")]
    public string Status { get; set; } = string.Empty;
}

