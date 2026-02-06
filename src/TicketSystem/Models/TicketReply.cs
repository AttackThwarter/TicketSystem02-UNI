namespace TicketSystem.Models;

public class TicketReply
{
    public int Id { get; set; }
    
    public string Message { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public bool IsAdminReply { get; set; } = false;
    
    // Foreign Key
    public int TicketId { get; set; }
    
    // Navigation Property
    public Ticket? Ticket { get; set; }
}
