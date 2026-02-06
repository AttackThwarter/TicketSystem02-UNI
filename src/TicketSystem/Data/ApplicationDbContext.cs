using Microsoft.EntityFrameworkCore;
using TicketSystem.Models;

namespace TicketSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketReply> TicketReplies { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });
        
        // Ticket Configuration
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(t => t.User)
                  .WithMany(u => u.Tickets)
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // TicketReply Configuration
        modelBuilder.Entity<TicketReply>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(r => r.Ticket)
                  .WithMany(t => t.Replies)
                  .HasForeignKey(r => r.TicketId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Seed Admin User
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            FullName = "System Admin",
            Email = "admin@ticket.com",
            PasswordHash = "admin123", // در پروژه واقعی باید Hash شود
            Role = "Admin"
        });
    }
}
