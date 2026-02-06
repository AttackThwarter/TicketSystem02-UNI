using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Data;
using TicketSystem.Models;
using TicketSystem.ViewModels;

namespace TicketSystem.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // چک کردن لاگین بودن کاربر
        private int? GetCurrentUserId()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return userId;
        }

        // داشبورد کاربر
        public async Task<IActionResult> Dashboard()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var tickets = await _context.Tickets
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .Select(t => new TicketListViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            var stats = new DashboardViewModel
            {
                TotalTickets = await _context.Tickets.CountAsync(t => t.UserId == userId),
                OpenTickets = await _context.Tickets.CountAsync(t => t.UserId == userId && t.Status == "Open"),
                InProgressTickets = await _context.Tickets.CountAsync(t => t.UserId == userId && t.Status == "InProgress"),
                ClosedTickets = await _context.Tickets.CountAsync(t => t.UserId == userId && t.Status == "Closed"),
                RecentTickets = tickets
            };

            return View(stats);
        }

        // فرم ایجاد تیکت جدید
        public IActionResult Create()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // ذخیره تیکت جدید
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTicketViewModel model)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var ticket = new Ticket
                {
                    Title = model.Title,
                    Description = model.Description,
                    Status = "Open",
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId.Value
                };

                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "درخواست شما با موفقیت ثبت شد";
                return RedirectToAction(nameof(MyTickets));
            }

            return View(model);
        }

        // لیست تیکت‌های کاربر
        public async Task<IActionResult> MyTickets()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var tickets = await _context.Tickets
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TicketListViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return View(tickets);
        }

        // جزئیات تیکت
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var ticket = await _context.Tickets
                .Include(t => t.Replies)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (ticket == null)
                return NotFound();

            var viewModel = new TicketDetailsViewModel
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status,
                CreatedAt = ticket.CreatedAt,
                Replies = ticket.Replies.Select(r => new ReplyViewModel
                {
                    Id = r.Id,
                    Message = r.Message,
                    CreatedAt = r.CreatedAt,
                    IsAdminReply = r.IsAdminReply
                }).ToList()
            };

            return View(viewModel);
        }

        // ارسال پاسخ کاربر
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReply(int id, string message)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // چک کردن اینکه تیکت متعلق به کاربر هست
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (ticket == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["ErrorMessage"] = "متن پاسخ نمی‌تواند خالی باشد";
                return RedirectToAction(nameof(Details), new { id = id });
            }

            var reply = new TicketReply
            {
                TicketId = id,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsAdminReply = false
            };

            _context.TicketReplies.Add(reply);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "پاسخ شما با موفقیت ثبت شد";
            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}
