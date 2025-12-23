namespace ExpenseTracker.ViewModel.Dashboard
{
    public class RecentTransactionViewModel
    {
        public DateTime Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsIncome { get; set; }   //true=Income , false=Expense
        public string? Icon { get; set; }    // optional icon key
    }
}
