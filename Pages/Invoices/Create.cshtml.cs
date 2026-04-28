using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;
using WarehouseSystem.Services;
using WarehouseSystem.Helpers;

namespace WarehouseSystem.Pages.Invoices;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly InvoiceNumberService _invoiceNumberService;

    public CreateModel(AppDbContext db, InvoiceNumberService invoiceNumberService)
    {
        _db = db;
        _invoiceNumberService = invoiceNumberService;
    }

    [BindProperty]
    public Invoice Invoice { get; set; } = new();

    [BindProperty]
    public List<InvoiceItemInput> Items { get; set; } = new();

    public SelectList SupplierList { get; set; } = null!;
    public List<Product> Products { get; set; } = new();

    public async Task OnGetAsync()
    {
        Invoice.DueDate = DateTime.Now.AddDays(14);
        await LoadListsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!Items.Any())
        {
            ModelState.AddModelError(string.Empty, "Faktura musí obsahovat alespoň jednu položku.");
            await LoadListsAsync();
            return Page();
        }

        Invoice.InvoiceNumber = await _invoiceNumberService.GenerateAsync(Invoice.Type);
        Invoice.CreatedByUserId = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
        Invoice.CreatedAt = DateTime.Now;

        foreach (var item in Items)
        {
            var product = await _db.Products.FindAsync(item.ProductId);
            if (product == null) continue;

            var invoiceItem = new InvoiceItem
            {
                ProductId = item.ProductId,
                ProductName = product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                VatRate = 21m
            };

            Invoice.Items.Add(invoiceItem);

            // Vytvoř skladový pohyb pro každou položku
            var movementType = Invoice.Type == InvoiceType.Received
                ? MovementType.Receipt
                : MovementType.Issue;

            if (movementType == MovementType.Issue && !product.CanIssue(item.Quantity))
            {
                ModelState.AddModelError(string.Empty,
                    $"Nelze vydat {item.Quantity} ks produktu {product.Name}. Na skladě je pouze {product.WarehouseInv} ks.");
                await LoadListsAsync();
                return Page();
            }

            product.ApplyMovement(movementType, item.Quantity);

            var movement = new WarehouseMovement
            {
                ProductId = item.ProductId,
                Type = movementType,
                Quantity = item.Quantity,
                Note = $"Faktura {Invoice.InvoiceNumber}",
                CreatedByUserId = Invoice.CreatedByUserId,
                SupplierId = Invoice.Type == InvoiceType.Received ? Invoice.SupplierId : null,
                CreatedAt = DateTime.Now
            };

            Invoice.Movements.Add(movement);
        }

        _db.Invoices.Add(Invoice);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Invoices/Index");
    }

    private async Task LoadListsAsync()
    {
        var suppliers = await _db.Suppliers
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
        SupplierList = new SelectList(suppliers, "Id", "Name");

        Products = await _db.Products
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}

// Pomocná třída pro binding položek z formuláře
public class InvoiceItemInput
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}