namespace RiskManagement.Models;

public class UserProfileChangeRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int OrganizationId { get; set; }
    public string? RequestedFirstName { get; set; }
    public string? RequestedLastName { get; set; }
    public string? RequestedEmail { get; set; }
    public string Status { get; set; } = "Pending";
    public int? ReviewedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public User User { get; set; } = null!;
    public User? ReviewedByUser { get; set; }
}
