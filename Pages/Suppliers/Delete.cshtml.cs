using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var supplier = await _db.Suppliers
            .Include(s => s.WarehouseMovements)
            .FirstOrDefaultAsync(s => s.Id == id);
        
        if (supplier == null)
            return RedirectToPage("/Suppliers/Index");
        
        if (supplier.WarehouseMovements.Any())
        {
            TempData["Error"] = $"Nelze smazal dodavatele {supplier.Name} - {supplier.WarehouseMovements.Count} pohybů v historii. Místo smazání pouze deaktivuj.";
        }

        _db.Suppliers.Remove(supplier);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Suppliers/Index");
    }
}