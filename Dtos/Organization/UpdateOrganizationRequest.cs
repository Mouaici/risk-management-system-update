using System.ComponentModel.DataAnnotations;

namespace RiskManagement.Dtos.Organization;

public class UpdateOrganizationRequest
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
    [RegularExpression("^(Open|InProgress|Closed)$",
        ErrorMessage = "Status must be Open, InProgress, or Closed")]
    public string Status { get; set; } = string.Empty;
}

