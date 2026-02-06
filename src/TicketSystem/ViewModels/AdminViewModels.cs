namespace TicketSystem.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int TotalUsers { get; set; }
        public int TodayTickets { get; set; }
        public int ThisWeekTickets { get; set; }
        public double CompletionRate { get; set; }
        public List<DailyStats> WeeklyStats { get; set; } = new List<DailyStats>();
        public List<RecentTicketViewModel> RecentTickets { get; set; } = new List<RecentTicketViewModel>();
    }

    public class DailyStats
    {
        public string Day { get; set; } = "";
        public int Count { get; set; }
    }

    public class RecentTicketViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; } = "";
    }

    public class AdminTicketViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; } = "";
        public string UserEmail { get; set; } = "";
        public int ReplyCount { get; set; }
    }

    public class AdminTicketDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; } = "";
        public string UserEmail { get; set; } = "";
        public List<AdminReplyViewModel> Replies { get; set; } = new List<AdminReplyViewModel>();
    }

    public class AdminReplyViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public bool IsAdminReply { get; set; }
    }
}
