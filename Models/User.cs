namespace RiskManagement.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string Role { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;
        public List<Risk> OwnedRisks { get; set; } = [];
        public List<RefreshToken> RefreshTokens { get; set; } = [];
        public List<UserProfileChangeRequest> ProfileChangeRequests { get; set; } = [];
        public List<UserProfileChangeRequest> ReviewedProfileChangeRequests { get; set; } = [];
        public List<ActionPlan> ActionPlans { get; set; } = [];
        
    }
}
