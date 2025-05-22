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

        Task<Project> GetProjectById(int projectId);

        Task<List<Project>> GetProjects(int currentUserId);

        Task<List<Board>> GetBoardsFromProject(int projectId);
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
                User = _userService.GetUserByUserIdAsync(currentUserId).Result,
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
            

            if (!_userService.IsAdmin(currentUserId, dto.ProjectId).Result)
                throw new UnauthorizedAccessException("Только администратор проекта может добавлять участников.");

            var userRole = _userService.GetUserRoleFromProject(dto.UserId, dto.ProjectId);


            if (userRole.Result != null)
                throw new InvalidOperationException("Пользователь уже состоит в проекте.");

            await _userService.AddUserToProject(dto.UserId, dto.ProjectId, dto.RoleId);
        }

        public async Task<Project?> GetProjectById(int projectId)
        {
            return await _context.Projects
                                  .Include(p => p.Boards)
                                  .ThenInclude(b => b.Columns)
                                  .ThenInclude(c => c.Tasks)
                                  .ThenInclude(t => t.Status)
                                  .Include(p => p.UserRoles)
                                  .ThenInclude(ur => ur.User)
                                  .FirstOrDefaultAsync(p => p.ProjectId == projectId);
        }

        public async Task<List<Project>> GetProjects(int currentUserId)
        {
            return await _context.Projects
                .Where(p => p.UserRoles.Any(ur => ur.UserId == currentUserId))
                .Include(p => p.Boards)
                .ThenInclude(b => b.Columns)
                .ThenInclude(c => c.Tasks)
                .ToListAsync();
        }

        public async Task<List<Board>> GetBoardsFromProject(int projectId)
        {
            return await _context.Boards
                .Where(b => b.ProjectId == projectId)
                .Include(b => b.Columns)
                .ThenInclude(c => c.Tasks)
                .ToListAsync();
        }
    }
}
