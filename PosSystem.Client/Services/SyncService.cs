using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using PosSystem.Client.Data;
using PosSystem.Shared.Models;

namespace PosSystem.Client.Services
{
    public class SyncService
    {
        private readonly IDbContextFactory<LocalDbContext> _dbFactory;
        private readonly HttpClient _httpClient;

        public SyncService(IDbContextFactory<LocalDbContext> dbFactory, HttpClient httpClient)
        {
            _dbFactory = dbFactory;
            _httpClient = httpClient;
        }

        public async Task SyncDataAsync()
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            
            // 1. Sync local sales to cloud
            var pendingSales = await db.Sales
                .Include(s => s.SaleItems)
                .Where(s => !s.IsSynced)
                .ToListAsync();

            if (pendingSales.Any())
            {
                var response = await _httpClient.PostAsJsonAsync("api/sync/sync-sales", pendingSales);
                if (response.IsSuccessStatusCode)
                {
                    // Mark as synced
                    foreach (var sale in pendingSales)
                    {
                        sale.IsSynced = true;
                    }
                    await db.SaveChangesAsync();
                }
            }

            // 2. Sync master data from cloud (Products, Employees)
            try 
            {
                var products = await _httpClient.GetFromJsonAsync<List<Product>>("api/sync/products");
                if (products != null && products.Any())
                {
                    // For simplicity, we just clear and re-insert, or update existing.
                    // A real app would use a more robust sync mechanism (like sync tokens).
                    db.Products.RemoveRange(db.Products);
                    await db.Products.AddRangeAsync(products);
                }

                var employees = await _httpClient.GetFromJsonAsync<List<Employee>>("api/sync/employees");
                if (employees != null && employees.Any())
                {
                    db.Employees.RemoveRange(db.Employees);
                    await db.Employees.AddRangeAsync(employees);
                }

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching master data: {ex.Message}");
            }
        }
    }
}
