using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.Models;
using TaskTracker.Models.DTO;
using TaskTracker.Service;

namespace TaskTracker.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId:int}/comments")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);


        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> List([FromQuery] int taskId, CancellationToken ct)
        {
            return Ok(await _commentService.GetCommentsByTaskAsync(taskId, ct));
        }

        [HttpPost]
        public async Task<ActionResult<Comment>> Create([FromBody] LeftCommentDTO dto, CancellationToken ct)
        {
            return Ok(_commentService.LeftCommentAsync(dto, CurrentUserId, ct));
        }
    }
}
