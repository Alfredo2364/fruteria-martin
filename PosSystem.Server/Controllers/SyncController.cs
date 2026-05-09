using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PosSystem.Shared.Models;

namespace PosSystem.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SyncController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("sync-sales")]
        public async Task<IActionResult> SyncSales([FromBody] List<Sale> sales)
        {
            if (sales == null || !sales.Any())
                return BadRequest("No sales to sync.");

            foreach (var sale in sales)
            {
                // Check if sale already exists based on some unique identifier or just add it
                // We'll reset the IDs to let SQL Server generate new ones, or use the client's IDs if allowed.
                // Assuming client IDs are temporary, but for simplicity we'll just add them.
                sale.Id = 0; // Let SQL Server assign identity
                if (sale.SaleItems != null)
                {
                    foreach (var item in sale.SaleItems)
                    {
                        item.Id = 0;
                        item.SaleId = 0; // Will be set by EF Core
                    }
                }
                
                sale.IsSynced = true;
                _context.Sales.Add(sale);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"{sales.Count} sales synced successfully." });
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }
        
        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return Ok(employees);
        }
    }
}
