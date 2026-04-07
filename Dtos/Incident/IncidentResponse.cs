namespace RiskManagement.Dtos.Incident;

public class IncidentResponse
{
    public int Id { get; set; }
    public int? OrganizationId { get; set; }
    public int? ReportedByUserId { get; set; }
    public string Title { get; set; } = null!;
    public string Severity { get; set; } = null!;
    public DateTime OccuredOn { get; set; }
    public string IncidentStatus { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
