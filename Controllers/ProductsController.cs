using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Filters;
using WarehouseSystem.Models;

namespace WarehouseSystem.Controllers;

public record ProductResponse(
    int Id,
    string Name,
    string? Description,
    decimal PurchasePrice,
    decimal SellingPrice,
    decimal DphRate,
    decimal PurchasePriceWithDph,
    decimal SellingPriceWithDph,
    int WarehouseInv,
    int MinimumInv,
    int? CategoryId,
    string? CategoryName);

public class ProductUpsertRequest
{
    [Required(ErrorMessage = "Název je povinný.")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Nákupní cena nemůže být záporná.")]
    public decimal PurchasePrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Prodejní cena nemůže být záporná.")]
    public decimal SellingPrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Minimální zásoba nemůže být záporná.")]
    public int MinimumInv { get; set; }

    public int? CategoryId { get; set; }
}

[ApiController]
[Route("api/products")]
[RequireAuth]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductResponse>>> GetAll()
    {
        var products = await _db.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return Ok(products.Select(MapToResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> GetById(int id)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        return Ok(MapToResponse(product));
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(ProductUpsertRequest request)
    {
        if (request.CategoryId is int catId && !await _db.Categories.AnyAsync(c => c.Id == catId))
            return BadRequest(new { message = "Zvolená kategorie neexistuje." });

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            PurchasePrice = request.PurchasePrice,
            SellingPrice = request.SellingPrice,
            MinimumInv = request.MinimumInv,
            CategoryId = request.CategoryId
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        await _db.Entry(product).Reference(p => p.Category).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, MapToResponse(product));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProductUpsertRequest request)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        if (request.CategoryId is int catId && !await _db.Categories.AnyAsync(c => c.Id == catId))
            return BadRequest(new { message = "Zvolená kategorie neexistuje." });

        product.Name = request.Name;
        product.Description = request.Description;
        product.PurchasePrice = request.PurchasePrice;
        product.SellingPrice = request.SellingPrice;
        product.MinimumInv = request.MinimumInv;
        product.CategoryId = request.CategoryId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        _db.Products.Remove(product);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "Produkt nelze smazat, protože má vázané pohyby na skladě." });
        }

        return NoContent();
    }

    private static ProductResponse MapToResponse(Product p) => new(
        p.Id, p.Name, p.Description, p.PurchasePrice, p.SellingPrice, p.DphRate,
        p.PurchasePriceWithDph, p.SellingPriceWithDph, p.WarehouseInv, p.MinimumInv,
        p.CategoryId, p.Category?.Name);
}