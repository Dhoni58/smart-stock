using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Suppliers;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public Supplier Supplier { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var icoExists = await _db.Suppliers.AnyAsync(s => s.Ico == Supplier.Ico);
        if (icoExists)
        {
            ModelState.AddModelError(string.Empty, "Dodavatel s tímto IČO již existuje.");
            return Page();
        }

        _db.Suppliers.Add(Supplier);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Suppliers/Index");
    }
}