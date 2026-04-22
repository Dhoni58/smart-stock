using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Products;

public class AddModel : PageModel
{
    private readonly AppDbContext _db;

    public AddModel(AppDbContext db)
    {
        _db = db;
    }
    [BindProperty]
    public Product Product { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        _db.Products.Add(Product);
        await _db.SaveChangesAsync();
        return RedirectToPage("/Products/Index");
    }
}