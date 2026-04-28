using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Services;

namespace WarehouseSystem.Controllers;

[ApiController]
[Route("api/export")]
public class ExportController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ExportService _exportService;

    public ExportController(AppDbContext db, ExportService exportService)
    {
        _db = db;
        _exportService = exportService;
    }

    [HttpGet("products/excel")]
    public async Task<IActionResult> ProductsExcel()
    {
        var products = await _db.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();

        var bytes = _exportService.ExportProductsToExcel(products);
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"produkty_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    [HttpGet("products/pdf")]
    public async Task<IActionResult> ProductsPdf()
    {
        var products = await _db.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();

        var bytes = _exportService.ExportProductsToPdf(products);
        return File(bytes, "application/pdf",
            $"produkty_{DateTime.Now:yyyyMMdd}.pdf");
    }

    [HttpGet("movements/excel")]
    public async Task<IActionResult> MovementsExcel()
    {
        var movements = await _db.WarehouseMovements
            .Include(m => m.Product)
            .Include(m => m.Supplier)
            .Include(m => m.CreatedByUser)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        var bytes = _exportService.ExportMovementsToExcel(movements);
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"pohyby_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    [HttpGet("movements/pdf")]
    public async Task<IActionResult> MovementsPdf()
    {
        var movements = await _db.WarehouseMovements
            .Include(m => m.Product)
            .Include(m => m.Supplier)
            .Include(m => m.CreatedByUser)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        var bytes = _exportService.ExportMovementsToPdf(movements);
        return File(bytes, "application/pdf",
            $"pohyby_{DateTime.Now:yyyyMMdd}.pdf");
    }
}