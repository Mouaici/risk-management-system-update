namespace RiskManagement.Dtos.Organization
{
    public class AuditDetailsResponse
    {
        public int OrganizationId { get; set; }
        public DateTime? AuditExpirationDate { get; set; }
        public DateTime? NextAuditRevisionDate { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}