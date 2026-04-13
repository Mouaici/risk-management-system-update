using System.ComponentModel.DataAnnotations.Schema;

namespace RiskManagement.Models;

public class Incident
{
    public int Id { get; set; }

    public int? OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public int? ReportedByUserId { get; set; }
    public User? ReportedByUser { get; set; }
    public List<ActionPlan> ActionPlans { get; set; } = [];

    public required string Title { get; set; }

    public required string Severity { get; set; } // Use string for enum-like values (Low, Medium, High)

    public required DateTime OccuredOn { get; set; }

    public required string IncidentStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
