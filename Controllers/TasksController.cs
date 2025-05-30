using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.Models;
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


        [HttpGet("{taskId:int}")]
        public async Task<ActionResult<Models.Task>> GetTaskById(int taskId)
        {
            return await _taskService.GetTaskInfo(taskId)
                   ?? (ActionResult<Models.Task>)NotFound();
        }


        [HttpPost]
        public async Task<ActionResult<Models.Task>> Create([FromBody] CreateTaskDTO dto)
        {
            try
            {
                var created = await _taskService.CreateNewTask(dto, CurrentUserId);
                return created;
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

        [HttpPost("newEpic")]
        public async Task<ActionResult<Models.Task>> CreateEpic([FromBody] CreateTaskDTO dto)
        {
            return await _taskService.CreateEpicAsync(dto, CurrentUserId);
        }


        [HttpPost("newDefect")]
        public async Task<ActionResult<Defect>> CreateDefect([FromBody] CreateDefectDTO dto)
        {
            try
            {
                var created = await _taskService.CreateNewDefect(dto, CurrentUserId);
                return created;
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

        [HttpGet("byUser")]
        public async Task<ActionResult<List<Models.Task>>> GetUserTasks()
        {
            try
            {
                var tasks = _taskService.GetTasksByUser(CurrentUserId);
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

        [HttpPut]
        public async Task<ActionResult<Models.Task>> Edit(
            [FromBody] EditTaskDTO dto)
        {
            try
            {
                var updated = await _taskService.EditTask(dto.taskId, dto, CurrentUserId);
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

        [HttpPost("assign")]
        public async Task<ActionResult<Models.Task>> AssignTask([FromBody] AssignTaskDTO dto)
        {
            try
            {
                var updated = await _taskService
                    .ResolveAssigneeUserRoleIdAsync(
                        taskId: dto.taskId,
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

        /// <summary>Создать новый эпик</summary>
        //[HttpPost("epic")]
        //public async Task<ActionResult<TaskModel>> CreateEpic([FromBody] CreateEpicDTO dto)
        //{
        //    try
        //    {
        //        var epic = await _taskService.CreateNewEpicAsync(dto, CurrentUserId);
        //        return epic;
        //    }
        //    catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        //    catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        //    catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
        //}

        /// <summary>Пометить существующую задачу как эпик</summary>
        [HttpPost("{taskId}/mark-epic")]
        public async Task<ActionResult<TaskModel>> MarkAsEpic(int taskId)
        {
            try
            {
                var t = await _taskService.MarkTaskAsEpicAsync(taskId, CurrentUserId);
                return Ok(t);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
        }

        /// <summary>Привязать задачу к эпику</summary>
        [HttpPost("{epicId}/attach/{subTaskId}")]
        public async Task<ActionResult<TaskModel>> AttachToEpic(int epicId, int subTaskId)
        {
            try
            {
                var epic = await _taskService.AttachTaskToEpicAsync(epicId, subTaskId, CurrentUserId);
                return Ok(epic);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
        }
    }
}
