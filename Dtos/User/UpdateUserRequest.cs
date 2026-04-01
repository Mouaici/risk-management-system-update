using System.ComponentModel.DataAnnotations;

namespace RiskManagement.Dtos.User;

public class UpdateUserRequest
{
    [MinLength(2)]
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MinLength(2)]
    [MaxLength(100)]
    public string? LastName { get; set; }

    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }

    [MinLength(8)]
    public string? NewPassword { get; set; }

    [MinLength(8)]
    public string? CurrentPassword { get; set; }

    [RegularExpression("^(User|Admin|Superadmin)$")]
    public string? Role { get; set; }

    [RegularExpression("^(Active|Inactive)$")]
    public string? Status { get; set; }
}
