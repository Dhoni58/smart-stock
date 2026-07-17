using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Users;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _hasher;

    public EditModel(AppDbContext db, IPasswordHasher<User> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    [BindProperty]
    public User CurrentUser { get; set; } = new();

    [BindProperty]
    public string? NewPin { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);

        if (user == null)
            return RedirectToPage("/Users/Index");

        CurrentUser = user;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _db.Users.FindAsync(CurrentUser.Id);

        if (user == null)
            return RedirectToPage("/Users/Index");

        // Změn PIN pouze pokud byl zadán nový
        if (!string.IsNullOrEmpty(NewPin))
        {
            if (NewPin.Length != 5 || !NewPin.All(char.IsDigit))
            {
                ModelState.AddModelError(string.Empty, "PIN musí obsahovat přesně 5 číslic.");
                return Page();
            }

             var otherUsers = await _db.Users
                .Where(u => u.Id != CurrentUser.Id)
                .ToListAsync();

            bool pinExists = otherUsers.Any(u =>
                _hasher.VerifyHashedPassword(u, u.PinHash, NewPin) == PasswordVerificationResult.Success);

            if (pinExists)
            {
                ModelState.AddModelError(string.Empty, "Tento PIN je již používán.");
                return Page();
            }

            user.PinHash = _hasher.HashPassword(user, NewPin);
        }

        user.Name = CurrentUser.Name;
        user.Role = CurrentUser.Role;
        user.IsActive = CurrentUser.IsActive;

        await _db.SaveChangesAsync();

        return RedirectToPage("/Users/Index");
    }
}