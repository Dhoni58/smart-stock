using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

namespace WarehouseSystem.Pages.Suppliers;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public Supplier Supplier { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var supplier = await _db.Suppliers.FindAsync(id);

        if (supplier == null)
            return RedirectToPage("/Suppliers/Index");

        Supplier = supplier;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var supplier = await _db.Suppliers.FindAsync(Supplier.Id);

        if (supplier == null)
            return RedirectToPage("/Suppliers/Index");

        supplier.Name = Supplier.Name;
        supplier.Address = Supplier.Address;
        supplier.ContactPerson = Supplier.ContactPerson;
        supplier.Phone = Supplier.Phone;
        supplier.Email = Supplier.Email;
        supplier.Note = Supplier.Note;
        supplier.IsActive = Supplier.IsActive;
        // IČO záměrně neměníme

        await _db.SaveChangesAsync();

        return RedirectToPage("/Suppliers/Index");
    }
}