using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Helpers;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages;

public class LoginModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _hasher;

    public LoginModel(AppDbContext db, IPasswordHasher<User> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    [BindProperty]
    public string Pin { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(5);

    public async Task<IActionResult> OnPostAsync()
     {
        var activeUsers = await _db.Users.Where(u => u.IsActive).ToListAsync();

        foreach (var user in activeUsers)
        {
            if (user.LockoutUntil is { } until && until > DateTime.UtcNow)
                continue; // skip locked-out users, they can't match anyway

            var result = _hasher.VerifyHashedPassword(user, user.PinHash, Pin);

            if (result == PasswordVerificationResult.Success ||
                result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.FailedLoginAttempts = 0;
                user.LockoutUntil = null;
                await _db.SaveChangesAsync();

                HttpContext.Session.SetInt32(SessionKeys.UserId, user.Id);
                HttpContext.Session.SetString(SessionKeys.UserName, user.Name);
                HttpContext.Session.SetString(SessionKeys.UserRole, user.Role.ToString());

                return RedirectToPage("/Index");
            }
        }
        ErrorMessage = "Nesprávný PIN. Zkuste znovu.";
        return Page();
     }
}