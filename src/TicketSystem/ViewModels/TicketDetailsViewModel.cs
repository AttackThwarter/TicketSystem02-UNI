namespace TicketSystem.ViewModels
{
    public class TicketDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        // اطلاعات کاربر (برای پنل ادمین)
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        
        public List<ReplyViewModel> Replies { get; set; } = new();
    }

    public class ReplyViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsAdminReply { get; set; }
    }
}
