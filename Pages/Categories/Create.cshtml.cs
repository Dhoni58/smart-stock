using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Categories;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public Category Category { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        _db.Categories.Add(Category);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Categories/Index");
    }
}