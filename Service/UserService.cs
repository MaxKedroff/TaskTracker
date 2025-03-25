using Microsoft.EntityFrameworkCore;
using TaskTracker.db;
using TaskTracker.Models;

namespace TaskTracker.Service
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(RegisterDTO register);
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
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        }

        public async Task<User> RegisterUserAsync(RegisterDTO register)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == register.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Пользователь с таким email уже существует.");

            }
            var password = BCrypt.Net.BCrypt.HashPassword(register.Password);
            var role = register.Email == "admin@example.com" ? "admin" : "user";

            var user = new User
            {
                Email = register.Email,
                Username = register.Username,
                PasswordHash = password,
                Role = role,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
