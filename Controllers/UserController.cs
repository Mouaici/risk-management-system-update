using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskManagement.Dtos.User;
using RiskManagement.Models;
using RiskManagement.Services;

namespace RiskManagement.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController(
    RiskManagementDbContext context,
    ICurrentUserService currentUserService,
    IPasswordService passwordService) : ControllerBase
{
    [HttpGet]
   
    public async Task<ActionResult<List<UserResponse>>> GetUsers()
    {
        var role = currentUserService.GetRequiredRole();
        var organizationId = currentUserService.GetRequiredOrganizationId();

        if (IsUser(role))
        {
            return Forbid();
        }

        var query = context.Users.AsNoTracking();
        if (!IsSuperadmin(role))
        {
            query = query.Where(user => user.OrganizationId == organizationId);
        }

        var users = await query
            .OrderBy(user => user.Id)
            .Select(user => new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Status = user.Status,
                OrganizationId = user.OrganizationId,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            })
            .ToListAsync();

        return Ok(users);
    }


    [HttpGet("{id:int}")]
    [Authorize(Policy = "AdminOrSuperadmin")]
    public async Task<ActionResult<UserResponse>> GetUserById(int id)
    {
        var role = currentUserService.GetRequiredRole();
        var currentUserId = currentUserService.GetRequiredUserId();
        var organizationId = currentUserService.GetRequiredOrganizationId();

        if (IsUser(role) && id != currentUserId)
        {
            return Forbid();
        }

        var user = await context.Users
            .AsNoTracking()
            .Where(user => user.Id == id && (IsSuperadmin(role) || user.OrganizationId == organizationId))
            .Select(user => new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Status = user.Status,
                OrganizationId = user.OrganizationId,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var userId = currentUserService.GetRequiredUserId();

        var user = await context.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(user => new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Status = user.Status,
                OrganizationId = user.OrganizationId,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    [Authorize(Policy = "SuperadminOnly")]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request)
    {
        // Prevent creating Superadmins
        if (request.Role.Equals("Superadmin", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Creating Superadmin users is not allowed.");
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailExists = await context.Users
            .AnyAsync(x => x.Email == normalizedEmail);

        if (emailExists)
        {
            return Conflict("Email already exists.");
        }

        var now = DateTime.UtcNow;

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail,
            Role = request.Role,
            Status = request.Status,
            OrganizationId = request.OrganizationId,
            CreatedAt = now,
            UpdatedAt = now,
            PasswordHash = ""
        };

        user.PasswordHash = passwordService.HashPassword(user, request.Password);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return Ok(Map(user));
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponse>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        var role = currentUserService.GetRequiredRole();
        var currentUserId = currentUserService.GetRequiredUserId();
        var organizationId = currentUserService.GetRequiredOrganizationId();

        var isUser = IsUser(role);
        if (isUser && id != currentUserId)
        {
            return Forbid();
        }

        if (isUser && (!string.IsNullOrWhiteSpace(request.Role) || !string.IsNullOrWhiteSpace(request.Status)))
        {
            return BadRequest("Role and status cannot be changed by users.");
        }

        var user = await context.Users.FirstOrDefaultAsync(user => user.Id == id);
        if (user is null)
        {
            return NotFound();
        }

        if (!IsSuperadmin(role) && user.OrganizationId != organizationId)
        {
            return Forbid();
        }
        // Prevent Admin & Superadmin from changing their own role
        if (!string.IsNullOrWhiteSpace(request.Role) && id == currentUserId)
        {
            return BadRequest("You cannot change your own role.");
        }
        // Only Superadmin can assign Superadmin role
        if (!string.IsNullOrWhiteSpace(request.Role) &&
            request.Role.Equals("Superadmin", StringComparison.OrdinalIgnoreCase) &&
            !IsSuperadmin(role))
        {
            return BadRequest("Only Superadmin can assign Superadmin role.");
        }
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var duplicateEmail = await context.Users.AnyAsync(x => x.Email == normalizedEmail && x.Id != id);
            if (duplicateEmail)
            {
                return Conflict("Email already exists.");
            }

            user.Email = normalizedEmail;
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            user.FirstName = request.FirstName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            user.LastName = request.LastName.Trim();
        }

        if (!isUser)
        {
            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                user.Role = request.Role;
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                user.Status = request.Status;
            }
        }

        var passwordChangeRequested =
            !string.IsNullOrWhiteSpace(request.NewPassword) ||
            !string.IsNullOrWhiteSpace(request.CurrentPassword);

        if (passwordChangeRequested)
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("NewPassword is required to change password.");
            }

            if (isUser)
            {
                if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                {
                    return BadRequest("CurrentPassword is required to change password.");
                }

                var currentPasswordOk = passwordService.VerifyPasswordOrUpgradeLegacy(
                    user,
                    request.CurrentPassword,
                    user.PasswordHash,
                    out var upgradedHash);

                if (!currentPasswordOk)
                {
                    return BadRequest("Current password is incorrect.");
                }

                if (!string.IsNullOrWhiteSpace(upgradedHash))
                {
                    user.PasswordHash = upgradedHash;
                }
            }

            user.PasswordHash = passwordService.HashPassword(user, request.NewPassword);
        }

        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return Ok(Map(user));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "SuperadminOnly")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await context.Users.FirstOrDefaultAsync(existingUser => existingUser.Id == id);
        if (user is null)
        {
            return NotFound();
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private static bool IsSuperadmin(string role) => role.Equals("Superadmin", StringComparison.OrdinalIgnoreCase);
    private static bool IsUser(string role) => role.Equals("User", StringComparison.OrdinalIgnoreCase);

    private static UserResponse Map(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            Status = user.Status,
            OrganizationId = user.OrganizationId,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}