using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

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

    public async Task OnGetAsync()
    {
        await LoadProductListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values
            .SelectMany(v => v.Errors))
        {
            Console.WriteLine("CHYBA: " + error.ErrorMessage);
        }
            await LoadProductListAsync();
            return Page();
        }

        Console.WriteLine($"ProductId: {Movement.ProductId}");
        Console.WriteLine($"Type: {Movement.Type}");
        Console.WriteLine($"Quantity: {Movement.Quantity}");
        var product = await _db.Products.FindAsync(Movement.ProductId);

        Console.WriteLine($"Product nalezen: {product != null}");
        if (product == null)
            return RedirectToPage("/Movements/Index");
        if (Movement.Type == MovementType.Receipt)
            product.WarehouseInv += Movement.Quantity;
        else
            product.WarehouseInv -= Movement.Quantity;
        
        _db.WarehouseMovements.Add(Movement);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Movements/Index");
    }

    private async Task LoadProductListAsync()
    {
        var Products = await _db.Products.ToListAsync();
        ProductList = new SelectList(Products, "Id", "Name");
    }
}
