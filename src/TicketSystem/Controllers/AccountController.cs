using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Data;
using TicketSystem.Models;
using TicketSystem.ViewModels;

namespace TicketSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // صفحه ورود
        public IActionResult Login()
        {
            // اگر قبلاً لاگین کرده، به داشبورد برو
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                var role = HttpContext.Session.GetString("UserRole");
                if (role == "Admin")
                    return RedirectToAction("AllTickets", "Admin");
                return RedirectToAction("Dashboard", "Tickets");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.PasswordHash == model.Password);

                if (user != null)
                {
                    // ذخیره اطلاعات کاربر در Session
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("UserName", user.FullName);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    // هدایت بر اساس نقش
                    if (user.Role == "Admin")
                        return RedirectToAction("AllTickets", "Admin");
                    
                    return RedirectToAction("Dashboard", "Tickets");
                }

                ModelState.AddModelError("", "ایمیل یا رمز عبور اشتباه است");
            }
            return View(model);
        }

        // صفحه ثبت‌نام
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
                return RedirectToAction("Dashboard", "Tickets");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // چک کردن تکراری نبودن ایمیل
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "این ایمیل قبلاً ثبت شده است");
                    return View(model);
                }

                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = model.Password,
                    Role = "User"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // ورود خودکار بعد از ثبت‌نام
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserRole", user.Role);

                return RedirectToAction("Dashboard", "Tickets");
            }
            return View(model);
        }

        // خروج
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
