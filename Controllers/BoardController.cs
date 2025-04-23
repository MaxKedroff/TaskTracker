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
        public async Task<ActionResult<Board>> Create([FromBody] CreateBoardDTO dto)
        {
            try
            {
                var board = await _boardService.CreateNewBoardAsync(dto, CurrentUserId);
                var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}/{board.BoardId}");

                return Created(location, board);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
