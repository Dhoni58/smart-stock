using Microsoft.EntityFrameworkCore;
using WarehouseSystem.Data;
using WarehouseSystem.Models;
using WarehouseSystem.Filters;
using WarehouseSystem.Services;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<AresService>();
builder.Services.AddControllers();
builder.Services.AddScoped<InvoiceNumberService>();
builder.Services.AddScoped<ExportService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<DemoResetService>();
}
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Smart Stock API",
        Version = "v1",
        Description = "REST API pro systém Smart Stock"
    });
});
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddFolderApplicationModelConvention(
        "/",
        model => model.Filters.Add(new AuthFilter()));
});

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration
                .GetConnectionString("Default")
    ));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "text/html; charset=utf-8";
        await context.HttpContext.Response.WriteAsync(
            "<h2>Příliš mnoho pokusů o přihlášení</h2>" +
            "<p>Zkuste to prosím znovu za pár minut.</p>" +
            "<a href=\"/Login\">Zpět na přihlášení</a>",
            cancellationToken);
    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        bool isLoginPost = context.Request.Path == "/Login"
            && HttpMethods.IsPost(context.Request.Method);

        if (!isLoginPost)
            return RateLimitPartition.GetNoLimiter("bypass");

        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            Window = TimeSpan.FromMinutes(5),
            PermitLimit = 5,
            QueueLimit = 0
        });
    });
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

app.UseRateLimiter();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Stock API v1");
    c.RoutePrefix = "swagger";
});

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Users.Any())
    {
        var hasher = new PasswordHasher<User>();
        var users = new List<User>
        {
            new User { Name = "Jan Novák", Role = UserRole.Vedouci },
            new User { Name = "Pavel Dvořák",  Role = UserRole.Skladnik },
            new User { Name = "Martin Král", Role = UserRole.Skladnik },
        };

        var managerPin = builder.Configuration["SeedUsers:ManagerPin"]
            ?? throw new InvalidOperationException("SeedUsers:ManagerPin is not configured.");
        var worker1Pin = builder.Configuration["SeedUsers:Worker1Pin"]
            ?? throw new InvalidOperationException("SeedUsers:Worker1Pin is not configured.");
        var worker2Pin = builder.Configuration["SeedUsers:Worker2Pin"]
            ?? throw new InvalidOperationException("SeedUsers:Worker2Pin is not configured.");
        users[0].PinHash = hasher.HashPassword(users[0], managerPin);
        users[1].PinHash = hasher.HashPassword(users[1], worker1Pin);
        users[2].PinHash = hasher.HashPassword(users[2], worker2Pin);

        db.Users.AddRange(users);
        db.SaveChanges();
    }

    if (!db.Suppliers.Any())
    {
        await DemoDataSeeder.SeedBusinessDataAsync(db);
    }

    if (!db.Categories.Any())
    {
       await DemoDataSeeder.SeedBusinessDataAsync(db);
    }

    if (!db.Products.Any())
    {
        await DemoDataSeeder.SeedBusinessDataAsync(db);
    }

    if (!db.WarehouseMovements.Any())
    {
        await DemoDataSeeder.SeedBusinessDataAsync(db);
    }
}

app.Run();
