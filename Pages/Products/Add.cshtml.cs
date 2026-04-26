using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

    public SelectList CategoryList { get; set; } = null!;

    public async Task OnGetAsync()
    {
        await LoadCategoryListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        _db.Products.Add(Product);
        await _db.SaveChangesAsync();
        return RedirectToPage("/Products/Index");
    }

     private async Task LoadCategoryListAsync()
    {
        var categories = await _db.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();
        CategoryList = new SelectList(categories, "Id", "Name");
    }
}