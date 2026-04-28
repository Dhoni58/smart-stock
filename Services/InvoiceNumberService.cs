using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Services;

public class InvoiceNumberService
{
    private readonly AppDbContext _db;

    public InvoiceNumberService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<string> GenerateAsync(InvoiceType type)
    {
        var year = DateTime.Now.Year;
        var prefix = type == InvoiceType.Received ? "PR" : "VY";

        var count = await _db.Invoices
            .CountAsync(i => i.Type == type &&
                        i.IssueDate.Year == year);

        return $"{prefix}-{year}-{(count + 1):D4}";
    }
}