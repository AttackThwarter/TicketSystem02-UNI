namespace TicketSystem.Models;

public class Ticket
{
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Open"; // Open, InProgress, Closed
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    // Foreign Key
    public int UserId { get; set; }
    
    // Navigation Properties
    public User? User { get; set; }
    
    public ICollection<TicketReply> Replies { get; set; } = new List<TicketReply>();
}
