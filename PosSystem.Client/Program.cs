using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using PosSystem.Client;
using PosSystem.Client.Data;
using PosSystem.Shared.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Replace with your API URL if different
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7169/") });

// Add EF Core SQLite DbContext
builder.Services.AddDbContextFactory<LocalDbContext>(options =>
    options.UseSqlite("Data Source=localpos.db"));

// Register SyncService
builder.Services.AddScoped<PosSystem.Client.Services.SyncService>();

// Execute migrations to ensure database is created in the browser storage
var host = builder.Build();


var dbFactory = host.Services.GetRequiredService<IDbContextFactory<LocalDbContext>>();
using var db = dbFactory.CreateDbContext();
await db.Database.EnsureCreatedAsync();

// Seed: Crea el usuario Admin por defecto si no existe ninguno
if (!db.Employees.Any())
{
    db.Employees.Add(new Employee
    {
        Name = "Administrador",
        Role = "Admin",
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        // Contraseña por defecto: admin1234  (cámbiala desde el módulo de empleados)
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin1234")
    });
    await db.SaveChangesAsync();
}

await host.RunAsync();
