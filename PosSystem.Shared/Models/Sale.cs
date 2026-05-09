using System;
using System.Collections.Generic;

namespace PosSystem.Shared.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public List<SaleItem> SaleItems { get; set; } = new();
        
        // Flag for sync status
        public bool IsSynced { get; set; } = false;
    }
}
