namespace RiskManagement.Dtos.Asset;

public class AssetResponse
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Definition { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public string? Classification { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

