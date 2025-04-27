using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.Models;
using TaskTracker.Models.DTO;
using TaskTracker.Service;


namespace TaskTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BoardController : ControllerBase
    {
        private readonly IBoardOperateService _boardService;
        public BoardController(IBoardOperateService boardService)
            => _boardService = boardService;

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBoardDTO dto)
        {
            try
            {
                var board = await _boardService.CreateNewBoardAsync(dto, CurrentUserId);
                return Ok(board);
            }
            catch (ArgumentException ex)
            {
                return BadRequest("произошла ошибка при обработке данных");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "У вас нет доступа для создания досок");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound("проект не был найден, возможно он был удален или перемещен");
            }
            catch (Exception ex)
            {
                // Логирование неожиданных ошибок
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
            }
        }
    }
}
