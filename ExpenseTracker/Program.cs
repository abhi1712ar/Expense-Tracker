using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Add MVC
// ----------------------
builder.Services.AddControllersWithViews();

// ----------------------
// Add DbContext
// ----------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("constr")));

// ----------------------
// Add FULL ASP.NET Identity
// ----------------------
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Password rules (customize as needed)
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Email settings
    options.User.RequireUniqueEmail = true;

    // Confirmation settings
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();  // Required for forgot password, email verification, reset tokens

var app = builder.Build();

// ----------------------
// Middleware pipeline
// ----------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ----------------------
// Identity authentication & authorization
// ----------------------
app.UseAuthentication(); // MUST come before UseAuthorization
app.UseAuthorization();

// ----------------------
// Default Route
// ----------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=LoginPage}/{id?}");

app.Run();
