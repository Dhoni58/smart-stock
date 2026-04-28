using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;
using WarehouseSystem.Filters;
using WarehouseSystem.Services;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<AresService>();
builder.Services.AddControllers();
builder.Services.AddScoped<ExportService>();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddFolderApplicationModelConvention(
        "/",
        model => model.Filters.Add(new AuthFilter()));
});

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration
                .GetConnectionString("Default")
    ));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
});

var app = builder.Build();

    app.MapControllers();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();

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

    if (!db.Users.Any())
    {
        var users = new List<User>
        {
            new User { Name = "Jan Novák", Pin = "12345", Role = UserRole.Vedouci },
            new User { Name = "Pavel Dvořák", Pin = "11111", Role = UserRole.Skladnik },
            new User { Name = "Martin Král", Pin = "22222", Role = UserRole.Skladnik },
        };

        db.Users.AddRange(users);
        db.SaveChanges();
    }

    if (!db.Products.Any())
    {
        var categories = new List<Category>
        {
        new Category { Name = "Spojovací materiál", Description = "Šrouby, matice, podložky" },
        new Category { Name = "Ložiska", Description = "Kuličková a válečková ložiska" },
        new Category { Name = "Hydraulika", Description = "Hydraulické prvky a kapaliny" },
        new Category { Name = "Pohony", Description = "Řemeny, řetězy, převodovky" },
        new Category { Name = "Elektro", Description = "Elektromotory, kabely, spínače" },
        new Category { Name = "Maziva", Description = "Oleje, tuky, čistidla" },
       };

        db.Categories.AddRange(categories);
        db.SaveChanges();
        var products = new List<Product>
        {
        // Spojovací materiál
        new Product { Name = "Šroub M8x20", Description = "Nerezový šroub", Price = 2.50m, WarehouseInv = 150, MinimumInv = 50, CategoryId = categories[0].Id },
        new Product { Name = "Šroub M10x30", Description = "Pozinkovaný šroub", Price = 3.80m, WarehouseInv = 8, MinimumInv = 50, CategoryId = categories[0].Id },
        new Product { Name = "Matice M8", Description = "Nerezová matice", Price = 1.20m, WarehouseInv = 200, MinimumInv = 50, CategoryId = categories[0].Id },
        new Product { Name = "Matice M10", Description = "Pozinkovaná matice", Price = 1.80m, WarehouseInv = 12, MinimumInv = 50, CategoryId = categories[0].Id },
        new Product { Name = "Podložka M8", Description = "Nerezová podložka", Price = 0.50m, WarehouseInv = 500, MinimumInv = 100, CategoryId = categories[0].Id },

        // Ložiska
        new Product { Name = "Ložisko 6205", Description = "Kuličkové ložisko 25x52x15", Price = 85m, WarehouseInv = 3, MinimumInv = 10, CategoryId = categories[1].Id },
        new Product { Name = "Ložisko 6206", Description = "Kuličkové ložisko 30x62x16", Price = 110m, WarehouseInv = 7, MinimumInv = 10, CategoryId = categories[1].Id },
        new Product { Name = "Ložisko 6305", Description = "Kuličkové ložisko 25x62x17", Price = 145m, WarehouseInv = 15, MinimumInv = 5, CategoryId = categories[1].Id },
        new Product { Name = "Ložisko NJ205", Description = "Válečkové ložisko", Price = 220m, WarehouseInv = 4, MinimumInv = 5, CategoryId = categories[1].Id },

        // Hydraulika
        new Product { Name = "Hydraulický olej 5L", Description = "HM46 olej pro hydraulické systémy", Price = 450m, WarehouseInv = 12, MinimumInv = 5, CategoryId = categories[2].Id },
        new Product { Name = "Hydraulický válec 50mm", Description = "Dvojčinný hydraulický válec", Price = 2800m, WarehouseInv = 2, MinimumInv = 2, CategoryId = categories[2].Id },
        new Product { Name = "Hydraulické hadice 1m", Description = "Vysokotlaká hadice DN10", Price = 380m, WarehouseInv = 20, MinimumInv = 5, CategoryId = categories[2].Id },

        // Pohony
        new Product { Name = "Řemen 1500mm", Description = "Klínový řemen A58", Price = 320m, WarehouseInv = 25, MinimumInv = 5, CategoryId = categories[3].Id },
        new Product { Name = "Řemen 2000mm", Description = "Klínový řemen A79", Price = 420m, WarehouseInv = 3, MinimumInv = 5, CategoryId = categories[3].Id },
        new Product { Name = "Řetěz 08B 1m", Description = "Jednořadý válečkový řetěz", Price = 280m, WarehouseInv = 30, MinimumInv = 10, CategoryId = categories[3].Id },
        new Product { Name = "Řetězové kolo Z25", Description = "Řetězové kolo 25 zubů", Price = 650m, WarehouseInv = 8, MinimumInv = 3, CategoryId = categories[3].Id },

        // Elektro
        new Product { Name = "Elektromotor 1.5kW", Description = "Třífázový asynchronní motor", Price = 4500m, WarehouseInv = 2, MinimumInv = 2, CategoryId = categories[4].Id },
        new Product { Name = "Kabel CYKY 3x2.5", Description = "Silový kabel 3x2.5mm²", Price = 45m, WarehouseInv = 150, MinimumInv = 50, CategoryId = categories[4].Id },
        new Product { Name = "Stykač 16A", Description = "Třípólový stykač 230V", Price = 890m, WarehouseInv = 5, MinimumInv = 3, CategoryId = categories[4].Id },

        // Maziva
        new Product { Name = "Vazelína LV2 400g", Description = "Lithiová vazelína pro ložiska", Price = 180m, WarehouseInv = 1, MinimumInv = 5, CategoryId = categories[5].Id },
        new Product { Name = "Motorový olej 5W40 4L", Description = "Syntetický motorový olej", Price = 520m, WarehouseInv = 18, MinimumInv = 5, CategoryId = categories[5].Id },
        };

        db.Products.AddRange(products);
        db.SaveChanges();

        var movements = new List<WarehouseMovement>
    {
        new WarehouseMovement { ProductId = products[0].Id, Type = MovementType.Receipt, Quantity = 300, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-30) },
        new WarehouseMovement { ProductId = products[0].Id, Type = MovementType.Issue, Quantity = 100, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-25) },
        new WarehouseMovement { ProductId = products[0].Id, Type = MovementType.Issue, Quantity = 50, Note = "Výdej do opravny", CreatedAt = DateTime.Now.AddDays(-10) },

        new WarehouseMovement { ProductId = products[1].Id, Type = MovementType.Receipt, Quantity = 100, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-28) },
        new WarehouseMovement { ProductId = products[1].Id, Type = MovementType.Issue, Quantity = 92, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-20) },

        new WarehouseMovement { ProductId = products[2].Id, Type = MovementType.Receipt, Quantity = 500, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-28) },
        new WarehouseMovement { ProductId = products[2].Id, Type = MovementType.Issue, Quantity = 150, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-15) },
        new WarehouseMovement { ProductId = products[2].Id, Type = MovementType.Issue, Quantity = 150, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-5) },

        new WarehouseMovement { ProductId = products[3].Id, Type = MovementType.Receipt, Quantity = 200, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-27) },
        new WarehouseMovement { ProductId = products[3].Id, Type = MovementType.Issue, Quantity = 188, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-10) },

        new WarehouseMovement { ProductId = products[5].Id, Type = MovementType.Receipt, Quantity = 20, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-26) },
        new WarehouseMovement { ProductId = products[5].Id, Type = MovementType.Issue, Quantity = 17, Note = "Výdej do opravny", CreatedAt = DateTime.Now.AddDays(-8) },

        new WarehouseMovement { ProductId = products[6].Id, Type = MovementType.Receipt, Quantity = 15, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-25) },
        new WarehouseMovement { ProductId = products[6].Id, Type = MovementType.Issue, Quantity = 8, Note = "Výdej do opravny", CreatedAt = DateTime.Now.AddDays(-12) },

        new WarehouseMovement { ProductId = products[7].Id, Type = MovementType.Receipt, Quantity = 20, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-24) },
        new WarehouseMovement { ProductId = products[7].Id, Type = MovementType.Issue, Quantity = 5, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-6) },

        new WarehouseMovement { ProductId = products[9].Id, Type = MovementType.Receipt, Quantity = 20, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-22) },
        new WarehouseMovement { ProductId = products[9].Id, Type = MovementType.Issue, Quantity = 8, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-14) },

        new WarehouseMovement { ProductId = products[10].Id, Type = MovementType.Receipt, Quantity = 5, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-20) },
        new WarehouseMovement { ProductId = products[10].Id, Type = MovementType.Issue, Quantity = 3, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-7) },

        new WarehouseMovement { ProductId = products[12].Id, Type = MovementType.Receipt, Quantity = 30, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-18) },
        new WarehouseMovement { ProductId = products[12].Id, Type = MovementType.Issue, Quantity = 5, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-9) },

        new WarehouseMovement { ProductId = products[13].Id, Type = MovementType.Receipt, Quantity = 10, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-17) },
        new WarehouseMovement { ProductId = products[13].Id, Type = MovementType.Issue, Quantity = 7, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-4) },

        new WarehouseMovement { ProductId = products[16].Id, Type = MovementType.Receipt, Quantity = 5, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-15) },
        new WarehouseMovement { ProductId = products[16].Id, Type = MovementType.Issue, Quantity = 3, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-3) },

        new WarehouseMovement { ProductId = products[17].Id, Type = MovementType.Receipt, Quantity = 300, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-14) },
        new WarehouseMovement { ProductId = products[17].Id, Type = MovementType.Issue, Quantity = 100, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-2) },

        new WarehouseMovement { ProductId = products[19].Id, Type = MovementType.Receipt, Quantity = 10, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-12) },
        new WarehouseMovement { ProductId = products[19].Id, Type = MovementType.Issue, Quantity = 9, Note = "Spotřeba na údržbu", CreatedAt = DateTime.Now.AddDays(-1) },

        new WarehouseMovement { ProductId = products[20].Id, Type = MovementType.Receipt, Quantity = 25, Note = "Nová dodávka", CreatedAt = DateTime.Now.AddDays(-10) },
        new WarehouseMovement { ProductId = products[20].Id, Type = MovementType.Issue, Quantity = 7, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-1) },
        };

        db.WarehouseMovements.AddRange(movements);
        db.SaveChanges();
    }
}

app.Run();
