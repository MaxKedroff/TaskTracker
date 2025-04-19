using Microsoft.AspNetCore.Mvc;
using TaskTracker.Models.DTO;
using TaskTracker.Service;


namespace TaskTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUserDTO([FromBody] CreateUserDTO register)
        {
            try
            {
                var user = await _authService.RegisterUserAsync(register);
                return Ok(new { Message = "Пользователь успешно создан, передайте ему данные от учетной записи." });
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
    }
}
