using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Users;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public User User { get; set; } = new();

    [BindProperty]
    public string? NewPin { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);

        if (user == null)
            return RedirectToPage("/Users/Index");

        User = user;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _db.Users.FindAsync(User.Id);

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

            var pinExists = await _db.Users
                .AnyAsync(u => u.Pin == NewPin && u.Id != User.Id);

            if (pinExists)
            {
                ModelState.AddModelError(string.Empty, "Tento PIN je již používán.");
                return Page();
            }

            user.Pin = NewPin;
        }

        user.Name = User.Name;
        user.Role = User.Role;
        user.IsActive = User.IsActive;

        await _db.SaveChangesAsync();

        return RedirectToPage("/Users/Index");
    }
}