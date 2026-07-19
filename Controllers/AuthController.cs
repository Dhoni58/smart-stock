using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Helpers;

namespace WarehouseSystem.Controllers;

public record LoginRequest(string Pin);
public record UserResponse(int Id, string Name, string Role);

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<Models.User> _hasher;

    public AuthController(AppDbContext db, IPasswordHasher<Models.User> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var activeUsers = await _db.Users.Where(u => u.IsActive).ToListAsync();

        foreach (var user in activeUsers)
        {
            if (user.LockoutUntil is { } until && until > DateTime.UtcNow)
                continue;

            var result = _hasher.VerifyHashedPassword(user, user.PinHash, request.Pin);

            if (result == PasswordVerificationResult.Success ||
                result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                HttpContext.Session.SetInt32(SessionKeys.UserId, user.Id);
                HttpContext.Session.SetString(SessionKeys.UserName, user.Name);
                HttpContext.Session.SetString(SessionKeys.UserRole, user.Role.ToString());

                return Ok(new UserResponse(user.Id, user.Name, user.Role.ToString()));
            }
        }

        return Unauthorized(new { message = "Nesprávný PIN. Zkuste znovu." });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok();
    }

    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
        if (userId == null)
            return Unauthorized();

        var name = HttpContext.Session.GetString(SessionKeys.UserName) ?? "";
        var role = HttpContext.Session.GetString(SessionKeys.UserRole) ?? "";

        return Ok(new UserResponse(userId.Value, name, role));
    }
}