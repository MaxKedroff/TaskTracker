using TaskTracker.Models.DTO;
using TaskTracker.Models;
using TaskTracker.db;
using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Service
{
    public interface IProjectOperateService
    {
        Task<Project> CreateNewProjectAsync(CreateProjectDTO dto, int currentUserId);

        System.Threading.Tasks.Task AddUserToProjectAsync(AddUsersToProjectDTO dto, int currentUserId);
    }
    public class ProjectOperatingService : IProjectOperateService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        public ProjectOperatingService(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<Project> CreateNewProjectAsync(CreateProjectDTO dto, int currentUserId)
        {
            if (dto.StartDate > dto.EndDate)
                throw new ArgumentException("время начала проекта не может быть позже времени окончания");
            var adminRole = new UserRole
            {
                UserId = currentUserId,
                User = _userService.GetUserByUsernameAsync(currentUserId.ToString()).Result,
                RoleId = SystemRoles.Admin
            };
            var project = new Project
            {
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                UserId = currentUserId,
                UserRoles = [
                    adminRole
                ]
            };
            _context.Projects.Add(project);
            _context.UserRoles.Add(adminRole);

            await _context.SaveChangesAsync();

            return project;
        }

        public async System.Threading.Tasks.Task AddUserToProjectAsync(AddUsersToProjectDTO dto, int currentUserId)
        {
            bool isAdmin = await _context.UserRoles.AnyAsync(ur =>
                ur.ProjectId == dto.ProjectId && ur.UserId == currentUserId && ur.RoleId == SystemRoles.Admin);

            if (!isAdmin)
                throw new UnauthorizedAccessException("Только администратор проекта может добавлять участников.");

            bool exists = await _context.UserRoles.AnyAsync(ur =>
            ur.ProjectId == dto.ProjectId && ur.UserId == dto.UserId);

            if (exists)
                throw new InvalidOperationException("Пользователь уже состоит в проекте.");

            _context.UserRoles.Add(new UserRole
            {
                ProjectId = dto.ProjectId,
                UserId = dto.UserId,
                RoleId = dto.RoleId
            });

            await _context.SaveChangesAsync();
        }
    }
}
