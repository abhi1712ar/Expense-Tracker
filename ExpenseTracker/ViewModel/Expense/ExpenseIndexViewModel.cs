using ExpenseTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.ViewModel.Expense
{
    public class ExpenseIndexViewModel
    {
        // All expenses for the logged-in user
        public List<ExpenseTracker.Models.Expense> ExpenseList { get; set; } = new();

        // Sum of all expenses
        public decimal TotalExpense { get; set; }

        // Chart.js labels (e.g. "02 Jan", "03 Jan")
        public List<string> ChartLabels { get; set; } = new();

        // Chart.js data points (amounts for each label)
        public List<decimal> ChartData { get; set; } = new();

        // Form object used by the "Add Expense" modal
        public NewExpenseForm NewExpense { get; set; } = new();
    }

    public class NewExpenseForm
    {
        [Required(ErrorMessage = "Please pick an icon")]
        public string IconEmoji { get; set; } = "💳";

        [Required(ErrorMessage = "Category is required")]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, 999999999, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;
    }
}
