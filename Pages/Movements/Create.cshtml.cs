using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;
using WarehouseSystem.Helpers;


namespace WarehouseSystem.Pages.Movements;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;
    
    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]

    public WarehouseMovement Movement { get; set; } = new();

    public SelectList ProductList { get; set; } = null!;
    public SelectList SupplierList { get; set; } = null!;

    public async Task OnGetAsync()
    {
        await LoadProductListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadProductListAsync();
            return Page();
        }

        var product = await _db.Products.FindAsync(Movement.ProductId);

        if (product == null)
            return RedirectToPage("/Movements/Index");
        // Validate pohybu aby nevzniklo záporné množství
        if (Movement.Type == MovementType.Issue && !product.CanIssue(Movement.Quantity))
        {
            ModelState.AddModelError(string.Empty,
            $"Nelze vydat {Movement.Quantity} ks jelikož na skladě je pouze {product.WarehouseInv} ks.");
            await LoadProductListAsync();
            return Page();
        }

        if (Movement.Type == MovementType.Issue)
            Movement.SupplierId = null;

        //Tvůrce pohybu
        Movement.CreatedByUserId = HttpContext.Session.GetInt32(SessionKeys.UserId);

        product.ApplyMovement(Movement.Type, Movement.Quantity);
        _db.WarehouseMovements.Add(Movement);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Movements/Index");
    }

    private async Task LoadProductListAsync()
    {
        var Products = await _db.Products.ToListAsync();
        ProductList = new SelectList(Products, "Id", "Name");

        var suppliers = await _db.Suppliers
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
        SupplierList = new SelectList(suppliers, "Id", "Name");
    }
}
