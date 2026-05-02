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
builder.Services.AddScoped<InvoiceNumberService>();
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
    if (!db.Suppliers.Any())
{
    var suppliers = new List<Supplier>
    {
        new Supplier { Ico = "27082440", Name = "Alza.cz a.s.", Address = "Jankovcova 1522/53, 170 00 Praha", ContactPerson = "Jan Novák", Phone = "225340111", Email = "obchod@alza.cz", IsActive = true },
        new Supplier { Ico = "26168685", Name = "Bosh s.r.o.", Address = "Radlická 100, 158 00 Praha", ContactPerson = "Pavel Svoboda", Phone = "251090111", Email = "info@bosch.cz", IsActive = true },
        new Supplier { Ico = "45272891", Name = "SKF Czech a.s.", Address = "Olomoucká 1580, 755 01 Vsetín", ContactPerson = "Martin Horáček", Phone = "571491111", Email = "skf@skf.cz", IsActive = true },
        new Supplier { Ico = "25788787", Name = "Hennlich s.r.o.", Address = "Litoměřická 2536, 412 01 Litoměřice", ContactPerson = "Tomáš Beneš", Phone = "416767111", Email = "info@hennlich.cz", IsActive = true },
        new Supplier { Ico = "27240500", Name = "Haberkorn s.r.o.", Address = "Vídeňská 264, 252 42 Jesenice", ContactPerson = "Lucie Marková", Phone = "244111111", Email = "info@haberkorn.cz", IsActive = true },
        new Supplier { Ico = "26757100", Name = "Rubena a.s.", Address = "Náchodská 1288, 549 01 Nové Město nad Metují", ContactPerson = "Petr Dvořák", Phone = "491111111", Email = "info@rubena.cz", IsActive = true },
        new Supplier { Ico = "25801447", Name = "Linde Gas a.s.", Address = "U Technoplynu 1324, 198 00 Praha", ContactPerson = "Jana Procházková", Phone = "272111111", Email = "info@linde.cz", IsActive = true },
        new Supplier { Ico = "27747087", Name = "Conrad Electronic CZ s.r.o.", Address = "Obchodní 110, 251 70 Čestlice", ContactPerson = "Roman Krejčí", Phone = "281111111", Email = "info@conrad.cz", IsActive = true },
    };

    db.Suppliers.AddRange(suppliers);
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

        var suppliers = db.Suppliers.ToList();

        var products = new List<Product>
        {
        // Spojovací materiál
        new Product { Name = "Šroub M8x20", Description = "Nerezový šroub", PurchasePrice = 2.50m, SellingPrice = 3.00m, WarehouseInv = 150, MinimumInv = 50, CategoryId = categories[0].Id },
        new Product { Name = "Šroub M10x30", Description = "Pozinkovaný šroub", PurchasePrice = 3.80m, SellingPrice = 4.00m, WarehouseInv = 8, MinimumInv = 50, CategoryId = categories[0].Id },
        new Product { Name = "Matice M8", Description = "Nerezová matice", PurchasePrice = 1.20m, SellingPrice = 2.00m, WarehouseInv = 200, MinimumInv = 50, CategoryId = categories[0].Id },
        new Product { Name = "Matice M10", Description = "Pozinkovaná matice", PurchasePrice = 1.80m, SellingPrice = 3.00m, WarehouseInv = 12, MinimumInv = 50, CategoryId = categories[0].Id },
        new Product { Name = "Podložka M8", Description = "Nerezová podložka", PurchasePrice = 0.50m, SellingPrice = 1.00m, WarehouseInv = 500, MinimumInv = 100, CategoryId = categories[0].Id },

        // Ložiska
        new Product { Name = "Ložisko 6205", Description = "Kuličkové ložisko 25x52x15", PurchasePrice = 85m, SellingPrice = 100m, WarehouseInv = 3, MinimumInv = 10, CategoryId = categories[1].Id },
        new Product { Name = "Ložisko 6206", Description = "Kuličkové ložisko 30x62x16", PurchasePrice = 110m, SellingPrice = 120m, WarehouseInv = 7, MinimumInv = 10, CategoryId = categories[1].Id },
        new Product { Name = "Ložisko 6305", Description = "Kuličkové ložisko 25x62x17", PurchasePrice = 145m, SellingPrice = 160m, WarehouseInv = 15, MinimumInv = 5, CategoryId = categories[1].Id },
        new Product { Name = "Ložisko NJ205", Description = "Válečkové ložisko", PurchasePrice = 220m, SellingPrice = 300m, WarehouseInv = 4, MinimumInv = 5, CategoryId = categories[1].Id },

        // Hydraulika
        new Product { Name = "Hydraulický olej 5L", Description = "HM46 olej pro hydraulické systémy", PurchasePrice = 450m, SellingPrice = 500m, WarehouseInv = 12, MinimumInv = 5, CategoryId = categories[2].Id },
        new Product { Name = "Hydraulický válec 50mm", Description = "Dvojčinný hydraulický válec", PurchasePrice = 2800m, SellingPrice = 3000m, WarehouseInv = 2, MinimumInv = 2, CategoryId = categories[2].Id },
        new Product { Name = "Hydraulické hadice 1m", Description = "Vysokotlaká hadice DN10", PurchasePrice = 380m, SellingPrice = 400m, WarehouseInv = 20, MinimumInv = 5, CategoryId = categories[2].Id },

        // Pohony
        new Product { Name = "Řemen 1500mm", Description = "Klínový řemen A58", PurchasePrice = 320m, SellingPrice = 350m, WarehouseInv = 25, MinimumInv = 5, CategoryId = categories[3].Id },
        new Product { Name = "Řemen 2000mm", Description = "Klínový řemen A79", PurchasePrice = 420m, SellingPrice = 450m, WarehouseInv = 3, MinimumInv = 5, CategoryId = categories[3].Id },
        new Product { Name = "Řetěz 08B 1m", Description = "Jednořadý válečkový řetěz", PurchasePrice = 280m, SellingPrice = 300m, WarehouseInv = 30, MinimumInv = 10, CategoryId = categories[3].Id },
        new Product { Name = "Řetězové kolo Z25", Description = "Řetězové kolo 25 zubů", PurchasePrice = 650m, SellingPrice = 700m, WarehouseInv = 8, MinimumInv = 3, CategoryId = categories[3].Id },

        // Elektro
        new Product { Name = "Elektromotor 1.5kW", Description = "Třífázový asynchronní motor", PurchasePrice = 4500m, SellingPrice = 5000m, WarehouseInv = 2, MinimumInv = 2, CategoryId = categories[4].Id },
        new Product { Name = "Kabel CYKY 3x2.5", Description = "Silový kabel 3x2.5mm²", PurchasePrice = 45m, SellingPrice = 60m, WarehouseInv = 150, MinimumInv = 50, CategoryId = categories[4].Id },
        new Product { Name = "Stykač 16A", Description = "Třípólový stykač 230V", PurchasePrice = 890m, SellingPrice = 900m, WarehouseInv = 5, MinimumInv = 3, CategoryId = categories[4].Id },

        // Maziva
        new Product { Name = "Vazelína LV2 400g", Description = "Lithiová vazelína pro ložiska", PurchasePrice = 180m, SellingPrice = 200m, WarehouseInv = 1, MinimumInv = 5, CategoryId = categories[5].Id },
        new Product { Name = "Motorový olej 5W40 4L", Description = "Syntetický motorový olej", PurchasePrice = 520m, SellingPrice = 520m, WarehouseInv = 18, MinimumInv = 5, CategoryId = categories[5].Id },
        };

        db.Products.AddRange(products);
        db.SaveChanges();

        var movements = new List<WarehouseMovement>
    {
        new WarehouseMovement { ProductId = products[0].Id, Type = MovementType.Receipt, Quantity = 300, Note = "Nová dodávka", SupplierId = suppliers.First(s => s.Name.Contains("Haberkorn")).Id, CreatedAt = DateTime.Now.AddDays(-30) },
        new WarehouseMovement { ProductId = products[0].Id, Type = MovementType.Issue, Quantity = 100, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-25) },
        new WarehouseMovement { ProductId = products[0].Id, Type = MovementType.Issue, Quantity = 50, Note = "Výdej do opravny", CreatedAt = DateTime.Now.AddDays(-10) },

        new WarehouseMovement { ProductId = products[1].Id, Type = MovementType.Receipt, Quantity = 100, Note = "Nová dodávka", SupplierId = suppliers.First(s => s.Name.Contains("Haberkorn")).Id, CreatedAt = DateTime.Now.AddDays(-28) },
        new WarehouseMovement { ProductId = products[1].Id, Type = MovementType.Issue, Quantity = 92, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-20) },

        new WarehouseMovement { ProductId = products[2].Id, Type = MovementType.Receipt, Quantity = 500, Note = "Nová dodávka", SupplierId = suppliers.First(s => s.Name.Contains("Haberkorn")).Id, CreatedAt = DateTime.Now.AddDays(-28) },
        new WarehouseMovement { ProductId = products[2].Id, Type = MovementType.Issue, Quantity = 150, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-15) },
        new WarehouseMovement { ProductId = products[2].Id, Type = MovementType.Issue, Quantity = 150, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-5) },

        new WarehouseMovement { ProductId = products[3].Id, Type = MovementType.Receipt, Quantity = 200, Note = "Nová dodávka", SupplierId = suppliers.First(s => s.Name.Contains("SKF")).Id, CreatedAt = DateTime.Now.AddDays(-27) },
        new WarehouseMovement { ProductId = products[3].Id, Type = MovementType.Issue, Quantity = 188, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-10) },

        new WarehouseMovement { ProductId = products[5].Id, Type = MovementType.Receipt, Quantity = 20, Note = "Nová dodávka", SupplierId = suppliers.First(s => s.Name.Contains("SKF")).Id, CreatedAt = DateTime.Now.AddDays(-26) },
        new WarehouseMovement { ProductId = products[5].Id, Type = MovementType.Issue, Quantity = 17, Note = "Výdej do opravny", CreatedAt = DateTime.Now.AddDays(-8) },

        new WarehouseMovement { ProductId = products[6].Id, Type = MovementType.Receipt, Quantity = 15, Note = "Nová dodávka", SupplierId = suppliers.First(s => s.Name.Contains("Hennlich")).Id, CreatedAt = DateTime.Now.AddDays(-25) },
        new WarehouseMovement { ProductId = products[6].Id, Type = MovementType.Issue, Quantity = 8, Note = "Výdej do opravny", CreatedAt = DateTime.Now.AddDays(-12) },

        new WarehouseMovement { ProductId = products[7].Id, Type = MovementType.Receipt, Quantity = 20, Note = "Nová dodávka", SupplierId = suppliers.First(s => s.Name.Contains("Hennlich")).Id, CreatedAt = DateTime.Now.AddDays(-24) },
        new WarehouseMovement { ProductId = products[7].Id, Type = MovementType.Issue, Quantity = 5, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-6) },

        new WarehouseMovement { ProductId = products[9].Id, Type = MovementType.Receipt, Quantity = 20, Note = "Nová dodávka", SupplierId = suppliers.First(s => s.Name.Contains("Rubena")).Id, CreatedAt = DateTime.Now.AddDays(-22) },
        new WarehouseMovement { ProductId = products[9].Id, Type = MovementType.Issue, Quantity = 8, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-14) },

        new WarehouseMovement { ProductId = products[10].Id, Type = MovementType.Receipt, Quantity = 5, Note = "Nová dodávka", SupplierId = suppliers.First(s => s.Name.Contains("Rubena")).Id, CreatedAt = DateTime.Now.AddDays(-20) },
        new WarehouseMovement { ProductId = products[10].Id, Type = MovementType.Issue, Quantity = 3, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-7) },

        new WarehouseMovement { ProductId = products[12].Id, Type = MovementType.Receipt, Quantity = 30, Note = "Nová dodávka", SupplierId = suppliers.First(s => s.Name.Contains("Conrad")).Id, CreatedAt = DateTime.Now.AddDays(-18) },
        new WarehouseMovement { ProductId = products[12].Id, Type = MovementType.Issue, Quantity = 5, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-9) },

        new WarehouseMovement { ProductId = products[13].Id, Type = MovementType.Receipt, Quantity = 10, Note = "Nová dodávka", SupplierId = suppliers.First(s => s.Name.Contains("Conrad")).Id, CreatedAt = DateTime.Now.AddDays(-17) },
        new WarehouseMovement { ProductId = products[13].Id, Type = MovementType.Issue, Quantity = 7, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-4) },

        new WarehouseMovement { ProductId = products[16].Id, Type = MovementType.Receipt, Quantity = 5, Note = "Nová dodávka", SupplierId = suppliers.First(s => s.Name.Contains("Linde")).Id, CreatedAt = DateTime.Now.AddDays(-15) },
        new WarehouseMovement { ProductId = products[16].Id, Type = MovementType.Issue, Quantity = 3, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-3) },

        new WarehouseMovement { ProductId = products[17].Id, Type = MovementType.Receipt, Quantity = 300, Note = "Nová dodávka", SupplierId = suppliers.First(s => s.Name.Contains("Linde")).Id, CreatedAt = DateTime.Now.AddDays(-14) },
        new WarehouseMovement { ProductId = products[17].Id, Type = MovementType.Issue, Quantity = 100, Note = "Výdej do výroby", CreatedAt = DateTime.Now.AddDays(-2) }
        };

        db.WarehouseMovements.AddRange(movements);
        db.SaveChanges();
    }
}

app.Run();
