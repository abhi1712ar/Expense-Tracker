using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Models;

namespace ExpenseTracker.ViewModel.Income
{
    // ------------------------------------------------------------
    // This ViewModel is used by Views/Income/Index.cshtml
    // It contains:
    //   • List of all income rows to display
    //   • Data for the bar chart
    //   • Nested ViewModel for the "Add Income" form
    // ------------------------------------------------------------
    public class IncomeIndexViewModel
    {
        // All income records for the current user
        public List<Models.Income> IncomeList { get; set; } = new();

        // Sum of all income amounts
        public decimal TotalIncome { get; set; }

        // Labels for the Income Overview bar chart (e.g. "1st Jan", "4th Jan")
        public List<string> ChartLabels { get; set; } = new();

        // Values for the Income Overview bar chart (sum of income for that day)
        public List<decimal> ChartData { get; set; } = new();

        // Nested ViewModel that represents the "Add Income" form inside the modal
        public CreateIncomeViewModel NewIncome { get; set; } = new();
    }

    // ------------------------------------------------------------
    // ViewModel used only for the Add Income form
    // It has DataAnnotations for validation.
    // ------------------------------------------------------------
    public class CreateIncomeViewModel
    {
        // Emoji stored here, e.g. "💼"
        [Required(ErrorMessage = "Please pick an icon")]
        public string IconEmoji { get; set; } = "💼";

        // Income source text (Salary, Freelancing, etc.)
        [Required(ErrorMessage = "Income source is required")]
        [MaxLength(100)]
        public string Source { get; set; } = string.Empty;

        // Income amount
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, 999999999, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        // Date when this income happened
        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;
    }
}
