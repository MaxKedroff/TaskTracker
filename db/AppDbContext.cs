using Microsoft.EntityFrameworkCore;
using TaskTracker.Models;

namespace TaskTracker.db
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@chill_team.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("adminPassword"), 
                    Role = "admin" 
                }
            );
        }
    }
}
