using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.Models.DTO;
using TaskTracker.Service;
using TaskModel = TaskTracker.Models.Task;

namespace TaskTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private int CurrentUserId
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<ActionResult<Models.Task>> Create([FromBody] CreateTaskDTO dto)
        {
            try
            {
                var created = await _taskService.CreateNewTask(dto, CurrentUserId);
                return Created(
                    $"/api/tasks/{created.TaskId}",
                    created);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Models.Task>> Edit(
            int id,
            [FromBody] EditTaskDTO dto)
        {
            try
            {
                var updated = await _taskService.EditTask(id, dto, CurrentUserId);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/assign")]
        public async Task<ActionResult<Models.Task>> AssignTask(
            int id,
            [FromBody] AssignTaskDTO dto)
        {
            try
            {
                var updated = await _taskService
                    .ResolveAssigneeUserRoleIdAsync(
                        taskId: id,
                        assigneeUserRoleId: dto.AssignedUserRoleId,
                        currentUserId: CurrentUserId);

                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("board/{boardId}")]
        public async Task<ActionResult<IEnumerable<Models.Task>>> GetByBoard(int boardId)
        {
            try
            {
                var tasks = await _taskService.GetTasksByBoardAsync(boardId, CurrentUserId);
                return Ok(tasks);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
