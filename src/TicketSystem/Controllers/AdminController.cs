using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Data;
using TicketSystem.Models;
using TicketSystem.ViewModels;

namespace TicketSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // چک کردن ادمین بودن
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        // لیست همه تیکت‌ها
        public async Task<IActionResult> AllTickets()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var tickets = await _context.Tickets
                .Include(t => t.User)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new AdminTicketListViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    UserFullName = t.User.FullName,
                    UserEmail = t.User.Email
                })
                .ToListAsync();

            return View(tickets);
        }

        // جزئیات تیکت برای ادمین
        public async Task<IActionResult> TicketDetails(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var ticket = await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.Replies)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return NotFound();

            var viewModel = new AdminTicketDetailsViewModel
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status,
                CreatedAt = ticket.CreatedAt,
                UserFullName = ticket.User.FullName,
                UserEmail = ticket.User.Email,
                Replies = ticket.Replies.OrderBy(r => r.CreatedAt).Select(r => new ReplyViewModel
                {
                    Id = r.Id,
                    Message = r.Message,
                    CreatedAt = r.CreatedAt,
                    IsAdminReply = r.IsAdminReply
                }).ToList()
            };

            return View(viewModel);
        }

        // تغییر وضعیت تیکت
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, string newStatus)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            // اعتبارسنجی وضعیت
            var validStatuses = new[] { "Open", "InProgress", "Closed" };
            if (!validStatuses.Contains(newStatus))
            {
                TempData["ErrorMessage"] = "وضعیت نامعتبر است";
                return RedirectToAction(nameof(TicketDetails), new { id = id });
            }

            ticket.Status = newStatus;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "وضعیت تیکت با موفقیت تغییر کرد";
            return RedirectToAction(nameof(TicketDetails), new { id = id });
        }

        // ارسال پاسخ ادمین
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminReply(int id, string message)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["ErrorMessage"] = "متن پاسخ نمی‌تواند خالی باشد";
                return RedirectToAction(nameof(TicketDetails), new { id = id });
            }

            var reply = new TicketReply
            {
                TicketId = id,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsAdminReply = true
            };

            _context.TicketReplies.Add(reply);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "پاسخ شما با موفقیت ثبت شد";
            return RedirectToAction(nameof(TicketDetails), new { id = id });
        }
    }
}
