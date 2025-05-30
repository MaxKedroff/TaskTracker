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


        [HttpGet("{boardId:int}")]
        public async Task<ActionResult<Board>> GetBoardById(int boardId)
        {
            return await _boardService.GetBoardByIdAsync(boardId);
        }
        [HttpPut("{boardId:int}")]
        public async Task<ActionResult<Board>> UpdateBoard(int boardId, [FromBody] UpdateBoardDTO dto)
        {
            return await _boardService.UpdateBoardAsync(dto, boardId);
        }

        [HttpPut("{boardId:int}/{columnId:int}")]
        public async Task<ActionResult<Column>> EditColumn([FromBody] EditColumnDTO dto, int boardId, int columnId)
        {
            return await _boardService.EditColumn(dto, boardId, columnId, CurrentUserId);
        }

        [HttpPost("{boardId:int}/newColumn")]
        public async Task<ActionResult<Column>> NewColumn(int boardId, [FromBody] CreateColumnDTO dto)
        {
            try
            {
                var column = await _boardService.CreateColumnAsync(dto, CurrentUserId, boardId);
                return column;
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is UnauthorizedAccessException)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
