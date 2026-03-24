namespace RiskManagement.Dtos.User;

public class UserProfileChangeRequestResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int OrganizationId { get; set; }
    public string? RequestedFirstName { get; set; }
    public string? RequestedLastName { get; set; }
    public string? RequestedEmail { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? ReviewedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
}
