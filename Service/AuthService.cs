using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using TaskTracker.db;
using TaskTracker.Models;
using TaskTracker.Models.DTO;

namespace TaskTracker.Service
{
    public interface IAuthService
    {
        Task<User> RegisterUserAsync(CreateUserDTO register);

        Task<string> AuthenticateAsync(LoginDTO login);
    }
    public class AuthService : IAuthService
    {

        private readonly IUserService _userService;
        private readonly AppDbContext _context;


        public AuthService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<string> AuthenticateAsync(LoginDTO login)
        {
            var user = await _userService.GetUserByUsernameAsync(login.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password)) {
                throw new UnauthorizedAccessException("Неверное имя пользователя или пароль.");
            }
            return AuthOptions.GenerateJwtToken(user.User_name);
        }

        public async Task<User> RegisterUserAsync(CreateUserDTO register)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.User_name == register.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Пользователь с таким логином уже существует.");

            }
            var password = BCrypt.Net.BCrypt.HashPassword(register.Password);

            var user = new User
            {
                User_name = register.Username,
                Password = password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
