using MeetingSummarize.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetingSummarize.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Meeting> Meetings => Set<Meeting>();
        public DbSet<Participant> Participants => Set<Participant>();
        public DbSet<ActionItem> ActionItems => Set<ActionItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Meeting>()
                .HasMany(m => m.Participants)
                .WithOne(p => p.Meeting!)
                .HasForeignKey(p => p.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Meeting>()
                .HasMany(m => m.ActionItems)
                .WithOne(a => a.Meeting!)
                .HasForeignKey(a => a.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}


