using Microsoft.EntityFrameworkCore;
using PollSurveyBuilder.API.Models;

namespace PollSurveyBuilder.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Poll> Polls { get; set; }

        public DbSet<PollOption> PollOptions { get; set; }

        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Poll>()
                .HasIndex(poll => poll.Code)
                .IsUnique();

            modelBuilder.Entity<Vote>()
                .HasIndex(vote => new
                {
                    vote.PollId,
                    vote.VoterToken
                })
                .IsUnique();

            modelBuilder.Entity<Poll>()
                .HasMany(poll => poll.Options)
                .WithOne(option => option.Poll)
                .HasForeignKey(option => option.PollId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Poll>()
                .HasMany(poll => poll.Votes)
                .WithOne(vote => vote.Poll)
                .HasForeignKey(vote => vote.PollId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PollOption>()
                .HasMany(option => option.Votes)
                .WithOne(vote => vote.PollOption)
                .HasForeignKey(vote => vote.PollOptionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}