using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Users;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _hasher;

    public CreateModel(AppDbContext db, IPasswordHasher<User> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    [BindProperty]
    public User CurrentUser { get; set; } = new();

    [BindProperty]
    public string Pin { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Ověř že PIN má přesně 5 číslic
        if (Pin.Length != 5 || !Pin.All(char.IsDigit))
        {
            ModelState.AddModelError(string.Empty, "PIN musí obsahovat přesně 5 číslic.");
            return Page();
        }

        // Ověř že PIN není již používán
        var existingUsers = await _db.Users.ToListAsync();
        bool pinExists = existingUsers.Any(u =>
            _hasher.VerifyHashedPassword(u, u.PinHash, Pin) == PasswordVerificationResult.Success);
        
        if (pinExists)
        {
            ModelState.AddModelError(string.Empty, "Tento PIN je již používán.");
            return Page();
        }

        CurrentUser.PinHash = _hasher.HashPassword(CurrentUser, Pin);

        _db.Users.Add(CurrentUser);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Users/Index");
    }
}