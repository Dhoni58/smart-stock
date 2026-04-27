using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Suppliers;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public List<Supplier> Suppliers { get; set; } = new();

    public async Task OnGetAsync()
    {
        Suppliers = await _db.Suppliers
            .OrderBy(s => s.Name)
            .ToListAsync();
    }
}