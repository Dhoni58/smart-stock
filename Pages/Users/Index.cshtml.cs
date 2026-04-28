using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Users;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public List<User> Users { get; set; } = new();

    public async Task OnGetAsync()
    {
        Users = await _db.Users
            .Include(u => u.WarehouseMovements)
            .OrderBy(u => u.Name)
            .ToListAsync();
    }
}