namespace RiskManagement.Dtos.Organization
{
    public class AuditDetailsRequest
    {
        public DateTime? AuditExpirationDate { get; set; }
        public DateTime? NextAuditRevisionDate { get; set; }
    }
}