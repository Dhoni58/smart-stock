using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Products;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;

    public DeleteModel(AppDbContext db)
    {
        _db = db;
    }
    public Product Product { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);

        if (product == null)
            return RedirectToPage("/Products/Index");
        
        Product = product;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);

        if (product != null)
        {
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage("/Products/Index");
    }
}