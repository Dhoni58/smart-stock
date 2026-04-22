using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration
                .GetConnectionString("Default")
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Products.Any())
    {
        var products = new List<Product>
        {
            new Product { Name = "Šroub M8x20", Description = "Nerezový šroub", Price = 2.50m, WarehouseInv = 150, MinimumInv = 20 },
            new Product { Name = "Matice M8", Description = "Nerezová matice", Price = 1.20m, WarehouseInv = 8, MinimumInv = 20 },
            new Product { Name = "Hydraulický olej 5L", Description = "Olej pro hydraulické systémy", Price = 450m, WarehouseInv = 12, MinimumInv = 5 },
            new Product { Name = "Ložisko 6205", Description = "Kuličkové ložisko", Price = 85m, WarehouseInv = 3, MinimumInv = 10 },
            new Product { Name = "Řemen 1500mm", Description = "Klínový řemen", Price = 320m, WarehouseInv = 25, MinimumInv = 5 },
        };

        db.Products.AddRange(products);
        db.SaveChanges();

        var movements = new List<WarehouseMovement>
        {
            new WarehouseMovement { ProductId = 1, Type = MovementType.Receipt, Quantity = 200, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-10) },
            new WarehouseMovement { ProductId = 1, Type = MovementType.Issue, Quantity = 50, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-7) },
            new WarehouseMovement { ProductId = 2, Type = MovementType.Receipt, Quantity = 100, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-8) },
            new WarehouseMovement { ProductId = 2, Type = MovementType.Issue, Quantity = 92, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-3) },
            new WarehouseMovement { ProductId = 3, Type = MovementType.Receipt, Quantity = 20, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-5) },
            new WarehouseMovement { ProductId = 4, Type = MovementType.Receipt, Quantity = 15, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-6) },
            new WarehouseMovement { ProductId = 4, Type = MovementType.Issue, Quantity = 12, Note = "Výdej do opravny", CreatedAt = DateTime.Now.AddDays(-1) },
            new WarehouseMovement { ProductId = 5, Type = MovementType.Receipt, Quantity = 30, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-4) },
        };

        db.WarehouseMovements.AddRange(movements);
        db.SaveChanges();
    }
}

app.Run();
