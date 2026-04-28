using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;

namespace WarehouseSystem.Pages.Invoices;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;

    public DeleteModel(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var invoice = await _db.Invoices
            .Include(i => i.Items)
            .Include(i => i.Movements)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
            return RedirectToPage("/Invoices/Index");

        // Odpoj pohyby od faktury — ponech je v historii
        foreach (var movement in invoice.Movements)
        {
            movement.InvoiceId = null;
            movement.Invoice = null;
        }

        _db.InvoiceItems.RemoveRange(invoice.Items);
        _db.Invoices.Remove(invoice);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Invoices/Index");
    }
}