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

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var totalTickets = await _context.Tickets.CountAsync();
            var openTickets = await _context.Tickets.CountAsync(t => t.Status == "Open");
            var inProgressTickets = await _context.Tickets.CountAsync(t => t.Status == "InProgress");
            var closedTickets = await _context.Tickets.CountAsync(t => t.Status == "Closed");
            var totalUsers = await _context.Users.CountAsync(u => u.Role == "User");

            var today = DateTime.Today;
            var weekAgo = today.AddDays(-7);

            var todayTickets = await _context.Tickets.CountAsync(t => t.CreatedAt.Date == today);
            var thisWeekTickets = await _context.Tickets.CountAsync(t => t.CreatedAt >= weekAgo);

            var completionRate = totalTickets > 0 ? (double)closedTickets / totalTickets * 100 : 0;

            // Weekly stats
            var weeklyStats = new List<DailyStats>();
            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var count = await _context.Tickets.CountAsync(t => t.CreatedAt.Date == date);
                weeklyStats.Add(new DailyStats
                {
                    Day = date.ToString("ddd"),
                    Count = count
                });
            }

            // Recent tickets
            var recentTickets = await _context.Tickets
                .Include(t => t.User)
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .Select(t => new RecentTicketViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    UserName = t.User != null ? t.User.FullName : "Unknown"
                })
                .ToListAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalTickets = totalTickets,
                OpenTickets = openTickets,
                InProgressTickets = inProgressTickets,
                ClosedTickets = closedTickets,
                TotalUsers = totalUsers,
                TodayTickets = todayTickets,
                ThisWeekTickets = thisWeekTickets,
                CompletionRate = Math.Round(completionRate, 1),
                WeeklyStats = weeklyStats,
                RecentTickets = recentTickets
            };

            return View(viewModel);
        }

        public async Task<IActionResult> AllTickets(string status = "")
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var query = _context.Tickets
                .Include(t => t.User)
                .Include(t => t.Replies)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            var tickets = await query
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new AdminTicketViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    UserName = t.User != null ? t.User.FullName : "Unknown",
                    UserEmail = t.User != null ? t.User.Email : "",
                    ReplyCount = t.Replies != null ? t.Replies.Count : 0
                })
                .ToListAsync();

            ViewBag.CurrentStatus = status;
            return View(tickets);
        }

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
                UserName = ticket.User?.FullName ?? "Unknown",
                UserEmail = ticket.User?.Email ?? "",
                Replies = ticket.Replies?.Select(r => new AdminReplyViewModel
                {
                    Id = r.Id,
                    Message = r.Message,
                    CreatedAt = r.CreatedAt,
                    IsAdminReply = r.IsAdminReply
                }).OrderBy(r => r.CreatedAt).ToList() ?? new List<AdminReplyViewModel>()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, string status)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            ticket.Status = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = "وضعیت تیکت با موفقیت تغییر کرد";
            return RedirectToAction("TicketDetails", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> AddReply(int ticketId, string message)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return NotFound();

            var reply = new TicketReply
            {
                TicketId = ticketId,
                Message = message,
                CreatedAt = DateTime.Now,
                IsAdminReply = true
            };

            _context.TicketReplies.Add(reply);
            await _context.SaveChangesAsync();

            TempData["Success"] = "پاسخ با موفقیت ثبت شد";
            return RedirectToAction("TicketDetails", new { id = ticketId });
        }
    }
}
