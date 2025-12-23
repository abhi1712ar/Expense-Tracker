using System.Text;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.ViewModel.Expense;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ExpenseController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Expense
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.Date)
                .ToListAsync();

            var vm = new ExpenseIndexViewModel
            {
                ExpenseList = expenses,
                TotalExpense = expenses.Sum(e => e.Amount)
            };

            BuildChartData(vm, expenses);

            return View(vm);
        }

        // POST: /Expense/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ExpenseIndexViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var form = model.NewExpense;

            if (!ModelState.IsValid)
            {
                // reload list + chart when validation fails
                var expenses = await _context.Expenses
                    .Where(e => e.UserId == userId)
                    .OrderBy(e => e.Date)
                    .ToListAsync();

                model.ExpenseList = expenses;
                model.TotalExpense = expenses.Sum(e => e.Amount);
                BuildChartData(model, expenses);

                return View("Index", model);
            }

            var expense = new Expense
            {
                UserId = userId,
                Category = form.Category,
                Amount = form.Amount,
                Date = form.Date,
                Icon = form.IconEmoji
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Expense added successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Expense/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);

            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (expense == null)
                return NotFound();

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Expense deleted.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Expense/Download
        [HttpGet]
        public async Task<IActionResult> Download()
        {
            var userId = _userManager.GetUserId(User);

            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.Date)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Date,Category,Amount,Icon");

            foreach (var e in expenses)
            {
                var category = e.Category?.Replace("\"", "\"\"") ?? "";
                var icon = e.Icon?.Replace("\"", "\"\"") ?? "";

                sb.AppendLine($"\"{e.Date:yyyy-MM-dd}\",\"{category}\",{e.Amount},\"{icon}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"Expenses_{DateTime.Now:yyyyMMddHHmmss}.csv";

            return File(bytes, "text/csv", fileName);
        }

        // Helper: build last 60 days chart
        private static void BuildChartData(ExpenseIndexViewModel vm, List<Expense> expenses)
        {
            var today = DateTime.Today;
            var start = today.AddDays(-59);

            var last60 = expenses
                .Where(e => e.Date.Date >= start)
                .ToList();

            var grouped = last60
                .GroupBy(e => e.Date.Date)
                .OrderBy(g => g.Key)
                .ToList();

            vm.ChartLabels = grouped
                .Select(g => g.Key.ToString("dd MMM"))
                .ToList();

            vm.ChartData = grouped
                .Select(g => g.Sum(x => x.Amount))
                .ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(EditExpenseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // validation failed -> back to list
                return RedirectToAction(nameof(Index));
            }

            var userId = _userManager.GetUserId(User);

            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == model.Id && e.UserId == userId);

            if (expense == null)
            {
                return NotFound();
            }

            expense.Category = model.Category;
            expense.Amount = model.Amount;
            expense.Date = model.Date;

            if (!string.IsNullOrWhiteSpace(model.IconEmoji))
            {
                expense.Icon = model.IconEmoji;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Expense updated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
