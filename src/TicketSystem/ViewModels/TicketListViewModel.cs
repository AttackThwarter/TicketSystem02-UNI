namespace TicketSystem.ViewModels
{
    public class TicketListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string StatusBadgeClass => Status switch
        {
            "Open" => "bg-success",
            "InProgress" => "bg-warning",
            "Closed" => "bg-secondary",
            _ => "bg-primary"
        };
        public string StatusDisplay => Status switch
        {
            "Open" => "باز",
            "InProgress" => "در حال بررسی",
            "Closed" => "بسته شده",
            _ => Status
        };
    }
}
