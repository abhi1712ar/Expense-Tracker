using System.Text;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.ViewModel.Income;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    // ------------------------------------------------------------
    // This controller handles everything related to Income:
    //   • Income overview page
    //   • Add income
    //   • Delete income
    //   • Download CSV (Excel readable)
    // Only logged-in users can access it.
    // ------------------------------------------------------------
    [Authorize]
    public class IncomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        // DI: we need DbContext to talk to DB and UserManager to know current user
        public IncomeController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --------------------------------------------------------
        // GET: /Income
        //
        // This builds the Income page:
        //   • loads all income for current user
        //   • prepares totals
        //   • builds bar chart data (last 60 days)
        //   • passes everything to IncomeIndexViewModel
        // --------------------------------------------------------
        public async Task<IActionResult> Index()
        {
            // Who is the current user?
            var userId = _userManager.GetUserId(User);

            // Load all income rows for this user, oldest -> newest
            var incomes = await _context.Incomes
                .Where(i => i.UserId == userId)
                .OrderBy(i => i.Date)
                .ToListAsync();

            // Build ViewModel
            var vm = new IncomeIndexViewModel
            {
                IncomeList = incomes,
                TotalIncome = incomes.Sum(i => i.Amount)
            };

            // Prepare chart data: last 60 days
            var today = DateTime.Today;
            var start = today.AddDays(-59); // includes today → 60 days

            var last60 = incomes
                .Where(i => i.Date.Date >= start)
                .ToList();

            // Group by date and sum per day
            var grouped = last60
                .GroupBy(i => i.Date.Date)
                .OrderBy(g => g.Key)
                .ToList();

            vm.ChartLabels = grouped
                .Select(g => g.Key.ToString("dd MMM"))
                .ToList();

            vm.ChartData = grouped
                .Select(g => g.Sum(x => x.Amount))
                .ToList();

            return View(vm);
        }

        // --------------------------------------------------------
        // POST: /Income/Add
        //
        // This handles the "Add Income" modal form submit.
        // The form is bound to IncomeIndexViewModel.NewIncome.
        // If validation fails, we rebuild the page (like Index)
        // but keep the entered values so user sees errors.
        // --------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(IncomeIndexViewModel model)
        {
            // Extract the nested "NewIncome" form model
            var form = model.NewIncome;

            // Current user id
            var userId = _userManager.GetUserId(User);

            if (!ModelState.IsValid)
            {
                // Validation failed → we must rebuild the same data as Index()

                var incomes = await _context.Incomes
                    .Where(i => i.UserId == userId)
                    .OrderBy(i => i.Date)
                    .ToListAsync();

                model.IncomeList = incomes;
                model.TotalIncome = incomes.Sum(i => i.Amount);

                var today = DateTime.Today;
                var start = today.AddDays(-59);

                var last60 = incomes
                    .Where(i => i.Date.Date >= start)
                    .ToList();

                var grouped = last60
                    .GroupBy(i => i.Date.Date)
                    .OrderBy(g => g.Key)
                    .ToList();

                model.ChartLabels = grouped
                    .Select(g => g.Key.ToString("dd MMM"))
                    .ToList();

                model.ChartData = grouped
                    .Select(g => g.Sum(x => x.Amount))
                    .ToList();

                // Return the same Index view, with ModelState errors
                return View("Index", model);
            }

            // If validation ok → create new Income entity
            var income = new Income
            {
                UserId = userId,
                Source = form.Source,
                Amount = form.Amount,
                Date = form.Date,
                Icon = form.IconEmoji   // emoji string saved here
            };

            _context.Incomes.Add(income);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Income added successfully.";
            return RedirectToAction(nameof(Index));
        }

        // --------------------------------------------------------
        // POST: /Income/Delete
        //
        // Called from delete confirmation modal.
        // We ensure:
        //   • Income exists
        //   • It belongs to current user
        // Then delete and redirect.
        // --------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);

            var income = await _context.Incomes
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            if (income == null)
            {
                return NotFound();
            }

            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Income deleted.";
            return RedirectToAction(nameof(Index));
        }

        // --------------------------------------------------------
        // GET: /Income/Download
        //
        // Generates a CSV file (Excel can open it easily).
        // We write:
        //   Date, Source, Amount, Icon
        // --------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Download()
        {
            var userId = _userManager.GetUserId(User);

            var incomes = await _context.Incomes
                .Where(i => i.UserId == userId)
                .OrderBy(i => i.Date)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Date,Source,Amount,Icon");

            foreach (var i in incomes)
            {
                // Escape commas in text by wrapping with quotes
                var source = i.Source?.Replace("\"", "\"\"") ?? "";
                var icon = i.Icon?.Replace("\"", "\"\"") ?? "";

                sb.AppendLine($"\"{i.Date:yyyy-MM-dd}\",\"{source}\",{i.Amount},\"{icon}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"Income_{DateTime.Now:yyyyMMddHHmmss}.csv";

            // "text/csv" – Excel opens this automatically
            return File(bytes, "text/csv", fileName);
        }

        // --------------------------------------------------------
        // POST: /Income/Update
        //
        // This updates an existing income entry.
        // Steps:
        //   • Validate model
        //   • Ensure the row exists
        //   • Ensure it belongs to current user
        //   • Update fields safely
        // --------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(EditIncomeViewModel model)
        {
            var userId = _userManager.GetUserId(User);

            // Validate only the fields that exist in EditIncomeViewModel
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please enter valid details.";
                return RedirectToAction(nameof(Index));
            }

            // Find income belonging to this user
            var existing = await _context.Incomes
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.UserId == userId);

            if (existing == null)
            {
                TempData["Error"] = "Income not found.";
                return RedirectToAction(nameof(Index));
            }

            // Update fields
            existing.Source = model.Source;
            existing.Amount = model.Amount;
            existing.Date = model.Date;

            // update Icon
            existing.Icon = model.IconEmoji;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Income updated successfully.";

            return RedirectToAction(nameof(Index));
        }



    }
}
