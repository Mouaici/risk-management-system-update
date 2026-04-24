namespace RiskManagement.Models
{
    public class Organization
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string IsoScope { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? AuditExpirationDate { get; set; }
        public DateTime? NextAuditRevisionDate { get; set; }

        public List<User> Users { get; set; } = new();
        public List<Asset> Assets { get; set; } = new();
        public List<Risk> Risks { get; set; } = new();
        public List<Incident> Incidents { get; set; } = new();
        public List<ActionPlan> ActionPlans { get; set; } = new();
    }
}
