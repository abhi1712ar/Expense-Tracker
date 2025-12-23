using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.ViewModel.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTracker.Controllers
{
    // Only logged-in users can see dashboard
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // MAIN DASHBOARD PAGE
        public async Task<IActionResult> Index()
        {
            string userId = "";
            // 1️⃣ Get current logged-in user id
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                userId = user.Id;
            }
           

            // 2️⃣ Prepare ViewModel
            var vm = new DashboardViewModel();

            // 3️⃣ Load all income for this user
            vm.AllIncome = await _context.Incomes
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.Date)
                .ToListAsync();

            // 4️⃣ Load all expenses for this user
            vm.AllExpenses = await _context.Expenses
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            // 5️⃣ Calculate totals
            vm.TotalIncome = vm.AllIncome.Sum(i => i.Amount);
            vm.TotalExpense = vm.AllExpenses.Sum(e => e.Amount);

            // 6️⃣ BUILD RECENT TRANSACTIONS (top middle card)
            var incomeTx = vm.AllIncome.Select(i => new TransactionItem
            {
                Title = i.Source,   // your Income model uses Source
                Amount = i.Amount,
                Date = i.Date,
                IsIncome = true
            });

            var expenseTx = vm.AllExpenses.Select(e => new TransactionItem
            {
                Title = e.Category, // your Expense model uses Category
                Amount = e.Amount,
                Date = e.Date,
                IsIncome = false
            });

            vm.RecentTransactions = incomeTx
                .Concat(expenseTx)
                .OrderByDescending(t => t.Date)
                .Take(6)
                .ToList();

            // 7️⃣ TOP DONUT CHART (Balance / Income / Expense)
            vm.DonutLabels = new List<string>
            {
                "Total Balance",
                "Total Income",
                "Total Expenses"
            };

            vm.DonutData = new List<decimal>
            {
                vm.Balance,
                vm.TotalIncome,
                vm.TotalExpense
            };

            // --------------------------------------------------
            // MIDDLE ROW: EXPENSES LIST + LAST 30 DAYS BAR CHART
            // --------------------------------------------------

            // List: take latest 5 expenses
            vm.RecentExpenses = vm.AllExpenses
                .OrderByDescending(e => e.Date)
                .Take(5)
                .ToList();

            // Bar chart: last 30 days expenses grouped by date
            var today = DateTime.Today;
            var start30 = today.AddDays(-29); // include today → 30 days

            var last30Expenses = vm.AllExpenses
                .Where(e => e.Date.Date >= start30)
                .ToList();

            var groupedExpenses = last30Expenses
                .GroupBy(e => e.Date.Date)
                .OrderBy(g => g.Key)
                .ToList();

            vm.ExpenseBarLabels = groupedExpenses
                .Select(g => g.Key.ToString("dd MMM"))
                .ToList();

            vm.ExpenseBarData = groupedExpenses
                .Select(g => g.Sum(x => x.Amount))
                .ToList();

            // --------------------------------------------------
            // BOTTOM ROW: LAST 60 DAYS INCOME DONUT + INCOME LIST
            // --------------------------------------------------

            // Income list: latest 6 income rows
            vm.RecentIncome = vm.AllIncome
                .OrderByDescending(i => i.Date)
                .Take(6)
                .ToList();

            // Donut chart: last 60 days income grouped by Source
            var start60 = today.AddDays(-59);

            var last60Income = vm.AllIncome
                .Where(i => i.Date.Date >= start60)
                .ToList();

            var groupedIncome = last60Income
                .GroupBy(i => i.Source)
                .OrderByDescending(g => g.Sum(x => x.Amount))
                .Take(6) // show top 6 sources
                .ToList();

            vm.IncomeSourceLabels = groupedIncome
                .Select(g => g.Key)
                .ToList();

            vm.IncomeSourceData = groupedIncome
                .Select(g => g.Sum(x => x.Amount))
                .ToList();

            // FINALLY, return the view with the filled model
            return View(vm);
        }
    }
}
