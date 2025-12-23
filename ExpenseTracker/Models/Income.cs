using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Income
    {
        public int Id { get; set; }

        // e.g. Salary, Freelancing, Interest from Savings
        [Required]
        [MaxLength(100)]
        public string Source { get; set; } = string.Empty;

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
