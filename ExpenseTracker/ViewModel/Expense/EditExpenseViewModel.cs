using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.ViewModel.Expense
{
    public class EditExpenseViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        // emoji like "🛒"
        public string? IconEmoji { get; set; }
    }
}
