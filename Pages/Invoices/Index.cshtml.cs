using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Invoices;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public List<Invoice> Invoices { get; set; } = new();

    public async Task OnGetAsync()
    {
        Invoices = await _db.Invoices
            .Include(i => i.Supplier)
            .Include(i => i.CreatedByUser)
            .Include(i => i.Items)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync();
    }
}