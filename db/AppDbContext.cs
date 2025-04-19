using Microsoft.EntityFrameworkCore;
using TaskTracker.Models;

namespace TaskTracker.db
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<SubTask> Subtasks { get; set; }
        public DbSet<Matrix> Matrices { get; set; }
        public DbSet<Defect> Defects { get; set; }
        public DbSet<Backlog> Backlogs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
        .HasOne(ur => ur.user)          
        .WithMany(u => u.UserRoles)    
        .HasForeignKey(ur => ur.UserId)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithOne(r => r.UserRole)
                .HasForeignKey<UserRole>(ur => ur.RoleId)
                .IsRequired()                       
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasData(
        new Role { RoleId = SystemRoles.Admin, Title = "Admin", Permissions = true },
        new Role { RoleId = SystemRoles.Employee, Title = "Employee", Permissions = false }
            );

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Project)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(ur => ur.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
         .HasIndex(ur => new { ur.ProjectId, ur.UserId })      
         .IsUnique();

            modelBuilder.Entity<Board>()
                .HasOne(b => b.Project)
                .WithMany(p => p.Boards)
                .HasForeignKey(b => b.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.Board)
                .WithMany(b => b.Tasks)
                .HasForeignKey(t => t.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Defect>()
                .HasOne(d => d.Board)
                .WithMany(b => b.Defects)
                .HasForeignKey(d => d.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Board>()
                .HasOne(b => b.Matrix)
                .WithOne(m => m.Board)
                .HasForeignKey<Board>(b => b.MatrixId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubTask>()
                .HasOne(st => st.Task)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(st => st.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.StatusRef)
                .WithOne(x => x.Task)                        
                .HasForeignKey<Models.Task>(t => t.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.backlog)              
                .WithMany(b => b.Tasks)           
                .HasForeignKey(b => b.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.UserRole)
                .WithMany(ur => ur.Tasks)           
                .HasForeignKey(ur => ur.TaskId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
