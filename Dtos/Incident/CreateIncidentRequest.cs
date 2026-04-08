namespace RiskManagement.Dtos.Incident;

public class CreateIncidentRequest
{
    public string Title { get; set; } = null!;
    public string Severity { get; set; } = null!;
    public DateTime OccuredOn { get; set; }
    public string IncidentStatus { get; set; } = null!;
}
