using System.ComponentModel.DataAnnotations;

namespace RiskManagement.Dtos.User;

public class CreateUserRequest
{
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(User|Admin|Superadmin)$")]
    public string Role { get; set; } = "User";

    [Required]
    [RegularExpression("^(Active|Inactive)$")]
    public string Status { get; set; } = "Active";

    [Required]
    public int OrganizationId { get; set; }
}
