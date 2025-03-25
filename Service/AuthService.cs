using System.Reflection.Metadata.Ecma335;
using TaskTracker.Models;

namespace TaskTracker.Service
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(LoginDTO login);
    }
    public class AuthService : IAuthService
    {

        private readonly IUserService _userService;

        public AuthService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<string> AuthenticateAsync(LoginDTO login)
        {
            var user = await _userService.GetUserByUsernameAsync(login.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash)) {
                throw new UnauthorizedAccessException("Неверное имя пользователя или пароль.");
            }
            return AuthOptions.GenerateJwtToken(user.Username);
        }
       
       
    }
}
