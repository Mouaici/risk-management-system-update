using System.ComponentModel.DataAnnotations;

namespace RiskManagement.Dtos.Incident;

public class CreateIncidentRequest
{
    [Required]
    [MinLength(3)]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [Required]
    [RegularExpression("^(Low|Medium|High)$",
        ErrorMessage = "Severity must be Low, Medium, or High")]
    public string Severity { get; set; } = null!;

    public DateTime OccuredOn { get; set; }

    [Required]
    [RegularExpression("^(Open|InProgress|Closed)$",
        ErrorMessage = "Status must be Open, InProgress, or Closed")]
    public string IncidentStatus { get; set; } = null!;
}