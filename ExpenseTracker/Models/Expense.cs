using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }

        // e.g. Shopping, Electricity Bill, Travel
        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        // Optional: detail text
        [MaxLength(200)]
        public string? Note { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [MaxLength(50)]
        public string? Icon { get; set; }

        // ---- LINK TO LOGGED-IN USER (Identity) ----
        [Required]
        public string UserId { get; set; } = string.Empty;

        public User? User { get; set; }   // navigation property
    }
}
