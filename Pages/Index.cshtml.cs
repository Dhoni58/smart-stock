using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public List<Product> LowStockProducts { get; set; } = new();

    public async Task OnGetAsync()
    {
        LowStockProducts = await _db.Products
            .Where(p => p.WarehouseInv <= p.MinimumInv)
            .OrderBy(p => p.WarehouseInv)
            .ToListAsync();
    }
}