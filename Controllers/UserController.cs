using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskManagement.Dtos.User;
using RiskManagement.Models;
using RiskManagement.Services;

namespace RiskManagement.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Policy = "AnyAuthenticatedUser")]
public class UserController(RiskManagementDbContext context, ICurrentUserService currentUserService) : ControllerBase
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

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponse>> UpdateUser(int id, [FromBody] AdminUpdateUserRequest request)
    {
        var role = currentUserService.GetRequiredRole();
        var organizationId = currentUserService.GetRequiredOrganizationId();

        if (IsUser(role))
        {
            return Forbid();
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var duplicateEmail = await context.Users.AnyAsync(user => user.Email == normalizedEmail && user.Id != id);
        if (duplicateEmail)
        {
            return Conflict("Email already exists.");
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

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Email = normalizedEmail;
        user.Role = request.Role;
        user.Status = request.Status;
        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return Ok(Map(user));
    }

    [HttpPost("me/change-requests")]
    public async Task<ActionResult<UserProfileChangeRequestResponse>> CreateProfileChangeRequest([FromBody] CreateUserProfileChangeRequest request)
    {
        var userId = currentUserService.GetRequiredUserId();
        var organizationId = currentUserService.GetRequiredOrganizationId();

        var hasAnyField =
            !string.IsNullOrWhiteSpace(request.FirstName) ||
            !string.IsNullOrWhiteSpace(request.LastName) ||
            !string.IsNullOrWhiteSpace(request.Email);
        if (!hasAnyField)
        {
            return BadRequest("At least one personal field must be provided.");
        }

        var changeRequest = new UserProfileChangeRequest
        {
            UserId = userId,
            OrganizationId = organizationId,
            RequestedFirstName = request.FirstName?.Trim(),
            RequestedLastName = request.LastName?.Trim(),
            RequestedEmail = request.Email?.Trim().ToLowerInvariant(),
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        context.UserProfileChangeRequests.Add(changeRequest);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMyProfileChangeRequests), Map(changeRequest));
    }

    [HttpGet("me/change-requests")]
    public async Task<ActionResult<List<UserProfileChangeRequestResponse>>> GetMyProfileChangeRequests()
    {
        var userId = currentUserService.GetRequiredUserId();

        var requests = await context.UserProfileChangeRequests
            .AsNoTracking()
            .Where(request => request.UserId == userId)
            .OrderByDescending(request => request.CreatedAt)
            .Select(request => new UserProfileChangeRequestResponse
            {
                Id = request.Id,
                UserId = request.UserId,
                OrganizationId = request.OrganizationId,
                RequestedFirstName = request.RequestedFirstName,
                RequestedLastName = request.RequestedLastName,
                RequestedEmail = request.RequestedEmail,
                Status = request.Status,
                ReviewedByUserId = request.ReviewedByUserId,
                CreatedAt = request.CreatedAt,
                ReviewedAt = request.ReviewedAt
            })
            .ToListAsync();

        return Ok(requests);
    }

    [HttpGet("change-requests")]
    public async Task<ActionResult<List<UserProfileChangeRequestResponse>>> GetProfileChangeRequests()
    {
        var role = currentUserService.GetRequiredRole();
        var organizationId = currentUserService.GetRequiredOrganizationId();

        if (IsUser(role))
        {
            return Forbid();
        }

        var query = context.UserProfileChangeRequests.AsNoTracking();
        if (!IsSuperadmin(role))
        {
            query = query.Where(request => request.OrganizationId == organizationId);
        }

        var requests = await query
            .OrderByDescending(request => request.CreatedAt)
            .Select(request => new UserProfileChangeRequestResponse
            {
                Id = request.Id,
                UserId = request.UserId,
                OrganizationId = request.OrganizationId,
                RequestedFirstName = request.RequestedFirstName,
                RequestedLastName = request.RequestedLastName,
                RequestedEmail = request.RequestedEmail,
                Status = request.Status,
                ReviewedByUserId = request.ReviewedByUserId,
                CreatedAt = request.CreatedAt,
                ReviewedAt = request.ReviewedAt
            })
            .ToListAsync();

        return Ok(requests);
    }

    [HttpPost("change-requests/{id:int}/approve")]
    public async Task<ActionResult<UserProfileChangeRequestResponse>> ApproveProfileChangeRequest(int id)
    {
        return await ReviewProfileChangeRequest(id, approved: true);
    }

    [HttpPost("change-requests/{id:int}/reject")]
    public async Task<ActionResult<UserProfileChangeRequestResponse>> RejectProfileChangeRequest(int id)
    {
        return await ReviewProfileChangeRequest(id, approved: false);
    }

    private async Task<ActionResult<UserProfileChangeRequestResponse>> ReviewProfileChangeRequest(int id, bool approved)
    {
        var role = currentUserService.GetRequiredRole();
        var reviewerUserId = currentUserService.GetRequiredUserId();
        var organizationId = currentUserService.GetRequiredOrganizationId();

        if (IsUser(role))
        {
            return Forbid();
        }

        var request = await context.UserProfileChangeRequests
            .FirstOrDefaultAsync(changeRequest =>
                changeRequest.Id == id &&
                (IsSuperadmin(role) || changeRequest.OrganizationId == organizationId));

        if (request is null)
        {
            return NotFound();
        }

        if (request.Status != "Pending")
        {
            return BadRequest("Only pending requests can be reviewed.");
        }

        request.Status = approved ? "Approved" : "Rejected";
        request.ReviewedByUserId = reviewerUserId;
        request.ReviewedAt = DateTime.UtcNow;

        if (approved)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (user is null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(request.RequestedEmail))
            {
                var duplicateEmail = await context.Users.AnyAsync(x => x.Email == request.RequestedEmail && x.Id != user.Id);
                if (duplicateEmail)
                {
                    return Conflict("Requested email already exists.");
                }

                user.Email = request.RequestedEmail;
            }

            if (!string.IsNullOrWhiteSpace(request.RequestedFirstName))
            {
                user.FirstName = request.RequestedFirstName;
            }

            if (!string.IsNullOrWhiteSpace(request.RequestedLastName))
            {
                user.LastName = request.RequestedLastName;
            }

            user.UpdatedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
        return Ok(Map(request));
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

    private static UserProfileChangeRequestResponse Map(UserProfileChangeRequest request)
    {
        return new UserProfileChangeRequestResponse
        {
            Id = request.Id,
            UserId = request.UserId,
            OrganizationId = request.OrganizationId,
            RequestedFirstName = request.RequestedFirstName,
            RequestedLastName = request.RequestedLastName,
            RequestedEmail = request.RequestedEmail,
            Status = request.Status,
            ReviewedByUserId = request.ReviewedByUserId,
            CreatedAt = request.CreatedAt,
            ReviewedAt = request.ReviewedAt
        };
    }
}