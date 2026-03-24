using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiskManagement.Services;

namespace RiskManagement.Controllers;

[ApiController]
[Route("api/asset")]
[Authorize]
public class AssetController(ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllAssets()
    {
        var role = currentUserService.GetRequiredRole();
        var hasAccess =
            role.Equals("User", StringComparison.OrdinalIgnoreCase) ||
            role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
            role.Equals("Superadmin", StringComparison.OrdinalIgnoreCase);

        if (!hasAccess)
        {
            return Forbid();
        }

        return Ok("API working");
    }
}