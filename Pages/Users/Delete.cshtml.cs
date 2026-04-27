using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Users;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;

    public DeleteModel(AppDbContext db)
    {
        _db = db;
    }

    public User User { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);

        if (user == null)
            return RedirectToPage("/Users/Index");

        User = user;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);

        if (user != null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage("/Users/Index");
    }
}