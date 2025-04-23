using Microsoft.EntityFrameworkCore;
using TaskTracker.db;
using TaskTracker.Models;

namespace TaskTracker.Service
{
    public interface IUserService
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByUserIdAsync(int userId);

        Task<UserRole> GetUserRoleFromProject(int userId, int projectId);

        Task<UserRole> GetUserFromEntireSystemAsync(int userId);

        Task<bool> IsAdmin(int userId, int projectId);

    }
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.User_name == username);

        }

        public async Task<User> GetUserByUserIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<UserRole?> GetUserRoleFromProject(int userId, int projectId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur =>
                    ur.UserId == userId &&
                    ur.ProjectId == projectId);
        }

        public async Task<UserRole?> GetUserFromEntireSystemAsync(int userId)
        {
            return await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserRoleId == userId);
        }

        public async Task<bool> IsAdmin(int userId, int projectId)
        {
            var userRole = GetUserRoleFromProject(userId, projectId).Result;
            return userRole.RoleId == 1 || userRole.RoleId == 2;
        }
    }
}
