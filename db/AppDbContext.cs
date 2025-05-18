using Microsoft.EntityFrameworkCore;
using TaskTracker.Models;
using TaskTracker.Models.DTO;

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

        public DbSet<Column> Columns { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }
        public DbSet<ColumnHistory> ColumnHistories { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            TrackStatusChanges();
            TrackColumnChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void TrackColumnChanges()
        {
            var now = DateTime.UtcNow;

            var moved = ChangeTracker
                .Entries<Models.Task>()
                .Where(e => e.State == EntityState.Modified &&
                            e.OriginalValues.GetValue<int>("ColumnId")
                          != e.CurrentValues.GetValue<int>("ColumnId"))
                .ToList();

            foreach (var e in moved)
            {
                var oldId = e.OriginalValues.GetValue<int>("ColumnId");
                var newId = e.CurrentValues.GetValue<int>("ColumnId");

                ColumnHistories.Add(new ColumnHistory
                {
                    TaskId = e.Entity.TaskId,
                    OldColumnId = oldId,
                    NewColumnId = newId,
                    ChangeDate = now
                });
            }
        }


        private void TrackStatusChanges()
        {
            var now = DateTime.UtcNow;

            var modifiedTasks = ChangeTracker
                .Entries<Models.Task>()
                .Where(e =>
                    e.State == EntityState.Modified
                    && e.OriginalValues.GetValue<int?>("StatusId")
                       != e.CurrentValues.GetValue<int?>("StatusId"))
                .ToList();

            if (!modifiedTasks.Any())
                return;

            foreach (var entry in modifiedTasks)
            {
                var taskId = entry.Entity.TaskId;
                var oldStatusId = entry.OriginalValues.GetValue<int?>("StatusId");
                var newStatusId = entry.CurrentValues.GetValue<int?>("StatusId");

                               var oldTitle = oldStatusId.HasValue
                    ? Statuses.Find(oldStatusId.Value)?.Title
                    : null;
                var newTitle = newStatusId.HasValue
                    ? Statuses.Find(newStatusId.Value)?.Title
                    : null;

                TaskHistories.Add(new TaskHistory
                {
                    TaskId = taskId,
                    OldStatus = oldTitle ?? "Unknown",
                    NewStatus = newTitle ?? "Unknown",
                    ChangeDate = now
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
            .Property(r => r.Permissions)
            .HasConversion<long>();

            modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)          
            .WithMany(u => u.UserRoles)    
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired()                       
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 1,
                Title = "Administrator",
                Permissions = Permission.CreateTask
                          | Permission.EditTask
                          | Permission.DeleteTask
                          | Permission.AssignTask
                          | Permission.ManageMembers
            },
            new Role
            {
                RoleId = 2,
                Title = "Project Manager",
                Permissions = Permission.CreateTask
                          | Permission.EditTask
                          | Permission.AssignTask
                          | Permission.ManageMembers
            },
            new Role
            {
                RoleId = 3,
                Title = "Developer",
                Permissions = Permission.CreateTask
                          | Permission.EditTask
            },
            new Role
            {
                RoleId = 4,
                Title = "Viewer",
                Permissions = Permission.None
            }
        );
        

            modelBuilder.Entity<Models.Task>()
    .HasOne(t => t.Status)
    .WithMany(s => s.Tasks)
    .HasForeignKey(t => t.StatusId);

            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.Priority)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.PriorityId);

            modelBuilder.Entity<Status>().HasData(
                    new Status { StatusId = 1, Title = "Todo", Color = "White" },
                    new Status { StatusId = 2, Title = "In-Progress", Color = "Green" },
                    new Status { StatusId = 3, Title = "Complete", Color = "Red"}
                );
            modelBuilder.Entity<Priority>().HasData(
        new Priority { PriorityId = 1, Title = "Critical" },
        new Priority { PriorityId = 2, Title = "High" },
        new Priority { PriorityId = 3, Title = "Medium" },
        new Priority { PriorityId = 4, Title = "Low" }
         );
            modelBuilder.Entity<Severity>().HasData(
        new Severity { SeverityId = 1, Title = "Blocker" },
        new Severity { SeverityId = 2, Title = "Major" },
        new Severity { SeverityId = 3, Title = "Minor" },
        new Severity { SeverityId = 4, Title = "Trivial" }
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
            modelBuilder.Entity<Column>()
                .HasOne(c => c.Board)
                .WithMany(b => b.Columns)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.Column)
                .WithMany(b => b.Tasks)
                .HasForeignKey(t => t.ColumnId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Defect>()
                .HasOne(d => d.Column)
                .WithMany(b => b.Defects)
                .HasForeignKey(d => d.ColumnId)
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

            //modelBuilder.Entity<Comment>()
            //    .HasOne(c => c.Task)
            //    .WithMany(t => t.Comments)
            //    .HasForeignKey(c => c.TaskId)
            //    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Models.Task>()
                .HasMany(t => t.Comments)
                .WithOne(c => c.Task)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Models.Task>()
        .HasOne(t => t.Backlog)
        .WithMany(b => b.Tasks)
        .HasForeignKey(t => t.BacklogId)  // Должно быть BacklogId в Task
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Cascade);

            // Правильная конфигурация для Task-UserRole
            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.UserRole)
                .WithMany(ur => ur.Tasks)
                .HasForeignKey(t => t.UserRoleId)  // Должно быть UserRoleId в Task
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Конфигурация для Comment-Task

        }
    }
}
