using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Users;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public User CurrentUser { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Ověř že PIN má přesně 5 číslic
        if (CurrentUser.Pin.Length != 5 || !CurrentUser.Pin.All(char.IsDigit))
        {
            ModelState.AddModelError(string.Empty, "PIN musí obsahovat přesně 5 číslic.");
            return Page();
        }

        // Ověř že PIN není již používán
        var pinExists = await _db.Users.AnyAsync(u => u.Pin == CurrentUser.Pin);
        if (pinExists)
        {
            ModelState.AddModelError(string.Empty, "Tento PIN je již používán.");
            return Page();
        }

        _db.Users.Add(CurrentUser);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Users/Index");
    }
}