using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Data;
using TicketSystem.Models;
using TicketSystem.ViewModels;

namespace TicketSystem.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Account/Login
    public IActionResult Login()
    {
        // اگر کاربر لاگین کرده، به داشبورد برود
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "Admin")
                return RedirectToAction("Index", "Admin");
            return RedirectToAction("Index", "Tickets");
        }
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email && u.PasswordHash == model.Password);

        if (user == null)
        {
            ModelState.AddModelError("", "ایمیل یا رمز عبور اشتباه است");
            return View(model);
        }

        // ذخیره اطلاعات در Session
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.FullName);
        HttpContext.Session.SetString("UserRole", user.Role);

        // هدایت بر اساس نقش
        if (user.Role == "Admin")
            return RedirectToAction("Index", "Admin");
        
        return RedirectToAction("Index", "Tickets");
    }

    // GET: /Account/Register
    public IActionResult Register()
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToAction("Index", "Tickets");
        }
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // چک کردن تکراری نبودن ایمیل
        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        {
            ModelState.AddModelError("Email", "این ایمیل قبلاً ثبت شده است");
            return View(model);
        }

        // ساخت کاربر جدید
        var user = new User
        {
            FullName = model.FullName,
            Email = model.Email,
            PasswordHash = model.Password, // در پروژه واقعی باید Hash شود
            Role = "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // لاگین خودکار
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.FullName);
        HttpContext.Session.SetString("UserRole", user.Role);

        return RedirectToAction("Index", "Tickets");
    }

    // GET: /Account/Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
