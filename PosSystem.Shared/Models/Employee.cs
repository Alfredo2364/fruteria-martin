using System;
using System.ComponentModel.DataAnnotations;

namespace PosSystem.Shared.Models
{
    public class Employee
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Role { get; set; } = string.Empty; // e.g., Admin, Cashier
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Contraseña hasheada con BCrypt. Nunca se almacena en texto plano.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;
    }
}
