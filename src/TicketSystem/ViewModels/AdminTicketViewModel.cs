namespace TicketSystem.ViewModels
{
    public class AdminTicketListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }

    public class AdminTicketDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public List<ReplyViewModel> Replies { get; set; } = new List<ReplyViewModel>();
    }

    public class ChangeStatusViewModel
    {
        public int TicketId { get; set; }
        public string CurrentStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
    }
}
