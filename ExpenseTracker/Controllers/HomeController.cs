using ExpenseTracker.Models;
using ExpenseTracker.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ExpenseTracker.Controllers
{
    public class HomeController : Controller
    {
        // ---------------------------------------------------------------
        // ✔ UserManager : used to Create / Find / Update users
        // ✔ SignInManager : used to Sign-In / Sign-Out users
        // ✔ ILogger : for debugging/logging (optional but good practice)
        // ---------------------------------------------------------------
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<HomeController> _logger;

        // ---------------------------------------------------------------
        // 🔧 Constructor
        // ASP.NET injects UserManager + SignInManager automatically.
        // Used for Registration + Login functionality.
        // ---------------------------------------------------------------
        public HomeController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }


        // ---------------------------------------------------------------
        // 📌 HOME PAGE (Optional)
        // This is NOT the dashboard. This is a simple welcome view.
        // You can choose to use or delete Views/Home/Index.cshtml.
        // ---------------------------------------------------------------
        public IActionResult Index()
        {
            return View();
        }


        // ---------------------------------------------------------------
        // 📌 REGISTER (GET)
        // Shows the registration form to the user.
        // ---------------------------------------------------------------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        // ---------------------------------------------------------------
        // 📌 REGISTER (POST)
        // Handles form submission for user registration.
        // Uses Identity to create the user in ASP.NET Identity tables.
        // ---------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Validate form fields using DataAnnotations
            if (!ModelState.IsValid)
                return View(model);

            // Create a new user object for Identity
            var user = new User
            {
                UserName = model.UserName,
                Email = model.EmailId
            };

            // Try to register user in database
            var result = await _userManager.CreateAsync(user, model.Password);

            // If successfully registered
            if (result.Succeeded)
            {
                // Toast message for success (TempData is used on next request)
                TempData["Success"] = "Account created successfully!";

                // Redirect user to Login page
                return RedirectToAction("LoginPage");
            }

            // If Identity returns errors (username/email/password issues)
            foreach (var error in result.Errors)
            {
                // Attach error message to ModelState so UI can display tooltip
                ModelState.AddModelError("", error.Description);
            }

            // Show same registration view with validation messages
            return View(model);
        }


        // ---------------------------------------------------------------
        // 📌 LOGIN (GET)
        // Shows the login page.
        // ---------------------------------------------------------------
        [HttpGet]
        public IActionResult LoginPage()
        {
            return View();
        }


        // ---------------------------------------------------------------
        // 📌 LOGIN (POST)
        // Validates username + password.
        // If correct → SignIn and redirect to Dashboard.
        // ---------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> LoginPage(LoginViewModel model)
        {
            // Step 1: Validate input (fields cannot be empty)
            if (!ModelState.IsValid)
                return View(model);

            // Step 2: Check if user exists
            var user = await _userManager.FindByNameAsync(model.UserName!);

            if (user == null)
            {
                // If username doesn't exist
                ModelState.AddModelError(nameof(model.UserName), "Invalid username");
                return View(model);
            }

            // Step 3: Validate password
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                model.Password!,
                isPersistent: false,      // do not remember login for long term
                lockoutOnFailure: false); // do not lock user on failures

            if (!result.Succeeded)
            {
                // Wrong password
                ModelState.AddModelError(nameof(model.Password), "Incorrect password");
                return View(model);
            }

            // Step 4: Redirect to the Dashboard after successful login
            return RedirectToAction("Index", "Dashboard");
        }


        // ---------------------------------------------------------------
        // 📌 LOGOUT
        // Clears authentication cookie and logs user out.
        // ---------------------------------------------------------------
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            // Redirect user to login page after logout
            return RedirectToAction("LoginPage");
        }


        // ---------------------------------------------------------------
        // 📌 ERROR PAGE (Auto generated)
        // Handles generic errors in the application.
        // ---------------------------------------------------------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
