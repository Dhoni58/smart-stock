using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Movements;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    
    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public List<WarehouseMovement> Movements { get; set; } = new();

    public async Task OnGetAsync()
    {
        Movements = await _db.WarehouseMovements
            .Include(m => m.Product)
            .Include(m => m.CreatedByUser)
            .Include(m => m.Supplier)
            .Include(m => m.Invoice)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }
}