using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskManagement.Dtos.Auth;
using RiskManagement.Models;
using RiskManagement.Services;

namespace RiskManagement.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    RiskManagementDbContext context,
    IPasswordService passwordService,
    ITokenService tokenService,
    IConfiguration configuration) : ControllerBase
{
    private const string RefreshCookieName = "refreshToken";

    private readonly int _refreshTokenDays = int.TryParse(configuration["Jwt:RefreshTokenDays"], out var days)
        ? days
        : 7;

    [HttpPost("register")]

    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var organizationExists = await context.Organizations
            .AnyAsync(organization => organization.Id == request.OrganizationId);

        if (!organizationExists)
        {
            return BadRequest("Invalid organization.");
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var emailExists = await context.Users.AnyAsync(user => user.Email == email);
        if (emailExists)
        {
            return Conflict("Email already exists.");
        }

        var now = DateTime.UtcNow;
        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            PasswordHash = string.Empty,
            Role = "User",
            Status = "Active",
            OrganizationId = request.OrganizationId,
            CreatedAt = now,
            UpdatedAt = now
        };

        user.PasswordHash = passwordService.HashPassword(user, request.Password);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(Register), new { user.Id }, new
        {
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role,
            user.OrganizationId
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (user is null)
        {
            return Unauthorized("Invalid credentials.");
        }

        var isPasswordValid = passwordService.VerifyPasswordOrUpgradeLegacy(
            user,
            request.Password,
            user.PasswordHash,
            out var upgradedHash);

        if (!isPasswordValid)
        {
            return Unauthorized("Invalid credentials.");
        }

        if (!string.IsNullOrWhiteSpace(upgradedHash))
        {
            user.PasswordHash = upgradedHash;
            user.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }

        var authResponse = await CreateSessionAsync(user, HttpContext.Connection.RemoteIpAddress?.ToString());
        return Ok(authResponse);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh()
    {
        if (!Request.Cookies.TryGetValue(RefreshCookieName, out var rawRefreshToken) ||
            string.IsNullOrWhiteSpace(rawRefreshToken))
        {
            return Unauthorized("Missing refresh token.");
        }

        var tokenHash = tokenService.HashRefreshToken(rawRefreshToken);
        var existingToken = await context.RefreshTokens
            .Include(refreshToken => refreshToken.User)
            .FirstOrDefaultAsync(refreshToken => refreshToken.TokenHash == tokenHash);

        if (existingToken is null || existingToken.RevokedAt.HasValue || existingToken.ExpiresAt <= DateTime.UtcNow)
        {
            return Unauthorized("Invalid refresh token.");
        }

        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        var authResponse = await CreateSessionAsync(
            existingToken.User,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            existingToken);

        return Ok(authResponse);
    }

    [HttpPost("logout")]
    [Authorize(Policy = "AnyAuthenticatedUser")]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue(RefreshCookieName, out var rawRefreshToken) &&
            !string.IsNullOrWhiteSpace(rawRefreshToken))
        {
            var tokenHash = tokenService.HashRefreshToken(rawRefreshToken);
            var token = await context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash);

            if (token is not null && !token.RevokedAt.HasValue)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                await context.SaveChangesAsync();
            }
        }

        Response.Cookies.Delete(RefreshCookieName);
        return NoContent();
    }

    private async Task<AuthResponse> CreateSessionAsync(User user, string? ipAddress, RefreshToken? replacedToken = null)
    {
        var rawRefreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenHash = tokenService.HashRefreshToken(rawRefreshToken);
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenDays);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = refreshTokenExpiresAt,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };

        context.RefreshTokens.Add(refreshToken);

        if (replacedToken is not null)
        {
            replacedToken.ReplacedByTokenHash = refreshTokenHash;
        }

        await context.SaveChangesAsync();

        SetRefreshCookie(rawRefreshToken, refreshTokenExpiresAt);

        var accessToken = tokenService.CreateAccessToken(user);
        return new AuthResponse
        {
            AccessToken = accessToken.AccessToken,
            AccessTokenExpiresAt = accessToken.AccessTokenExpiresAt,
            Email = user.Email,
            Role = user.Role,
            OrganizationId = user.OrganizationId
        };
    }

    private void SetRefreshCookie(string refreshToken, DateTime expiresAt)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = expiresAt
        };

        Response.Cookies.Append(RefreshCookieName, refreshToken, cookieOptions);
    }
}

