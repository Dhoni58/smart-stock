using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;

namespace WarehouseSystem.Pages;

public class LoginModel : PageModel
{
    private readonly AppDbContext _db;

    public LoginModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public string Pin { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Pin == Pin && u.IsActive);

        if (user == null)
        {
            ErrorMessage = "Nesprávný PIN. Zkuste to znovu.";
            return Page();
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.Name);
        HttpContext.Session.SetString("UserRole", user.Role.ToString());

        return RedirectToPage("/Index");
    }
}