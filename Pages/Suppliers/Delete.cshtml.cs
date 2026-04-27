using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Suppliers;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;

    public DeleteModel(AppDbContext db)
    {
        _db = db;
    }

    public Supplier Supplier { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var supplier = await _db.Suppliers.FindAsync(id);

        if (supplier == null)
            return RedirectToPage("/Suppliers/Index");

        Supplier = supplier;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var supplier = await _db.Suppliers.FindAsync(id);

        if (supplier != null)
        {
            _db.Suppliers.Remove(supplier);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage("/Suppliers/Index");
    }
}