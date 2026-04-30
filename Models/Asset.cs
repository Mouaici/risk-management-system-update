namespace RiskManagement.Models
{
    public class Asset
    {
        public int Id { get; set; }

        public int OrganizationId { get; set; }


        public required string Name { get; set; } = null!;

        public string? Definition { get; set; }

        public required string AssetType { get; set; }

        public string? Classification { get; set; }

        public required string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Organization Organization { get; set; } = null!;
        public List<Risk> Risks { get; set; } = [];

    }
}
