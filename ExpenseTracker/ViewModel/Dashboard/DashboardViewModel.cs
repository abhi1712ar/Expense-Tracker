using ExpenseTracker.Models;

namespace ExpenseTracker.ViewModel.Dashboard
{
    // ViewModel used by Dashboard/Index view
    public class DashboardViewModel
    {
        // --------------------------------------------------
        // RAW DATA FOR CURRENT USER
        // --------------------------------------------------
        // All income rows for this user
        public List<ExpenseTracker.Models.Income> AllIncome { get; set; } = new();

        // All expense rows for this user
        public List<ExpenseTracker.Models.Expense> AllExpenses { get; set; } = new();

        // --------------------------------------------------
        // TOP SUMMARY + RECENT TRANSACTIONS
        // --------------------------------------------------
        public List<TransactionItem> RecentTransactions { get; set; } = new();

        // Sum of AllIncome.Amount
        public decimal TotalIncome { get; set; }

        // Sum of AllExpenses.Amount
        public decimal TotalExpense { get; set; }

        // Calculated balance (no setter needed)
        public decimal Balance => TotalIncome - TotalExpense;

        // Donut chart at top-right (Balance / Income / Expense)
        public List<string> DonutLabels { get; set; } = new();
        public List<decimal> DonutData { get; set; } = new();

        // --------------------------------------------------
        // MIDDLE ROW: EXPENSES LIST + LAST 30 DAYS BAR CHART
        // --------------------------------------------------

        // Short expenses list for “Expenses” card (left middle)
        public List<ExpenseTracker.Models.Expense> RecentExpenses { get; set; } = new();

        // Bar chart labels for last 30 days expenses (dates)
        public List<string> ExpenseBarLabels { get; set; } = new();

        // Bar chart values for last 30 days expenses (sum per day)
        public List<decimal> ExpenseBarData { get; set; } = new();

        // --------------------------------------------------
        // BOTTOM ROW: LAST 60 DAYS INCOME DONUT + INCOME LIST
        // --------------------------------------------------

        // Short incomes list for “Income” card (right bottom)
        public List<ExpenseTracker.Models.Income> RecentIncome { get; set; } = new();

        // Donut chart labels by income source (Salary, Freelance...)
        public List<string> IncomeSourceLabels { get; set; } = new();

        // Donut chart values – amount per income source
        public List<decimal> IncomeSourceData { get; set; } = new();
    }

    // Helper model used to merge Income + Expense into one list
    public class TransactionItem
    {
        public string Title { get; set; } = string.Empty; // Income.Source or Expense.Category
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public bool IsIncome { get; set; }
    }
}
