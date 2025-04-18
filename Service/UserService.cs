using Microsoft.EntityFrameworkCore;
using TaskTracker.db;
using TaskTracker.Models;

namespace TaskTracker.Service
{
    public interface IUserService
    {
        Task<User> GetUserByUsernameAsync(string username);
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

        
    }
}
