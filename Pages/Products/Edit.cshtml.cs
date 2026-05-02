using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Products;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public Product Product { get; set;} = new();

    public SelectList CategoryList { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);

        if (product == null)
            return RedirectToPage("/Products/Index");
        
        Product = product;
        await LoadCategoryListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoryListAsync();
            return Page();
        }

        var product = await _db.Products.FindAsync(Product.Id);

        if (product == null)
            return RedirectToPage("/Products/Index");
        
        product.Name = Product.Name;
        product.Description = Product.Description;
        product.PurchasePrice = Product.PurchasePrice;
        product.SellingPrice = Product.SellingPrice;
        product.MinimumInv = Product.MinimumInv;
        product.CategoryId = Product.CategoryId;
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