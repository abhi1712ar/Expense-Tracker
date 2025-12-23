using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.ViewModel.Income
{
    public class EditIncomeViewModel
    {
        public int Id { get; set; }

        [Required]
        public string? Source { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string? IconEmoji { get; set; }
    }
}
