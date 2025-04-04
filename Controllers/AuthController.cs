using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskTracker.Models;
using TaskTracker;
using Microsoft.AspNetCore.Authorization;
using TaskTracker.Service;
using System.Threading.Tasks;


namespace TaskTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            try
            {
                var user = await _userService.RegisterUserAsync(register);
                return Ok(new { Message = "Пользователь успешно зарегистрирован." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            try
            {
                var token = await _authService.AuthenticateAsync(login);
                return Ok(new {Token = token});
            }
            catch (UnauthorizedAccessException ex) {
            
                return Unauthorized(ex.Message);
            }

        }
        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            return Ok();
        }

        [HttpGet("test2")]
        public async Task<IActionResult> Test2()
        {
            return Ok();
        }


        [HttpGet("admin-data")]
        [Authorize(Roles = "admin")]
        public IActionResult AdminData()
        {
            return Ok(new {Message = "Это данные только для администратора." });
        }

        [HttpGet("data")]
        [Authorize(Roles = "user,admin")]
        public IActionResult Data()
        {
            return Ok(new { Message = "Это данные для пользователей и администраторов." });
        }
        
    }
}
