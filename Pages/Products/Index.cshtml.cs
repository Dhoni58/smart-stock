using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Products;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public List<Product> Products { get; set; } = new();
    public SelectList CategoryList { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }

    public async Task OnGetAsync()
    {
        var query = _db.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var searchLower = Search.ToLower();
            query = query.Where(p => 
                 p.Name.ToLower().Contains(searchLower) ||
                (p.Description != null && p.Description.ToLower().Contains(searchLower)) ||
                (p.Category != null && p.Category.Name.ToLower().Contains(searchLower)));
        }
        if (CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == CategoryId);

        Products = await query
            .OrderBy(p => p.Name)
            .ToListAsync();

        var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        CategoryList = new SelectList(categories, "Id", "Name");
    }
}