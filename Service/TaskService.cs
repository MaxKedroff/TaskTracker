using TaskTracker.Models.DTO;
using TaskTracker.Models;
using Task = TaskTracker.Models.Task;
using TaskTracker.db;
using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Service
{

    public interface ITaskService
    {
        Task<Task> CreateNewTask(CreateTaskDTO dto, int currentUserId);
        Task<Task> EditTask(int taskId, EditTaskDTO dto, int currentUserId);

        Task<Task> GetTaskInfo(int taskId);

        Task<Task> ResolveAssigneeUserRoleIdAsync(
            int taskId,
            int assigneeUserRoleId,
            int currentUserId);

        Task<IEnumerable<Task>> GetTasksByBoardAsync(int boardId, int currentUserId);

    }

    public class TaskService : ITaskService
    {
        private readonly AppDbContext _db;
        private readonly IBoardOperateService _boardService;
        private readonly IUserService _userService;

        public TaskService(AppDbContext db, IBoardOperateService boardService, IUserService userService)
        {
            _db = db;
            _userService = userService;
            _boardService = boardService;
        }

        public async Task<Task> CreateNewTask(CreateTaskDTO dto, int currentUserId)
        {
            var board = _boardService.GetBoardByIdAsync(dto.BoardId).Result;
            if (board == null)
                throw new KeyNotFoundException("Доска не найдена");
            var columnId = _boardService.FindColumnIdByStatus(dto.BoardId, dto.currentColumn).Result;
            var userRole = _userService.GetUserRoleFromProject(currentUserId, board.ProjectId).Result;
            var hasPermission = userRole.Role.Permissions.HasFlag(Permission.CreateTask);
            if (userRole == null || !userRole.Role.Permissions.HasFlag(Permission.CreateTask))
            {
                throw new UnauthorizedAccessException(
                    "У вас нет прав на создание задач в этом проекте");
            }
            var task = new Task
            {
                Title = dto.Title,
                Description = dto.Description,
                Deadline = dto.Deadline,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                StatusId = dto.statusId,
                ColumnId = columnId,
                PriorityId = dto.priorityId,
            };

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();
            return task;
        }

        public async Task<Task> EditTask(int taskId, EditTaskDTO dto, int currentUserId)
        {
            var task = GetTaskInfo(taskId).Result;
            if (task == null)
                throw new KeyNotFoundException("Задача не найдена");
            var projectId = task.Column.Board.ProjectId;
            var userRole = _userService.GetUserRoleFromProject(currentUserId, projectId).Result;
            if (userRole == null
                || !userRole.Role.Permissions.HasFlag(Permission.EditTask))
            {
                throw new UnauthorizedAccessException(
                    "У вас нет прав на редактирование этой задачи");
            }

            if (!string.IsNullOrWhiteSpace(dto.Title))
                task.Title = dto.Title;
            if (dto.Description != null)
                task.Description = dto.Description;
            if (dto.Deadline.HasValue)
                task.Deadline = dto.Deadline.Value;
            if (!string.IsNullOrWhiteSpace(dto.statusId.ToString()))
                task.StatusId = dto.statusId;
            if (!string.IsNullOrWhiteSpace(dto.priorityId.ToString()))
                task.PriorityId = dto.priorityId;
            if (!string.IsNullOrEmpty(dto.currentColumn))
            {
                int newColumnId = await _boardService
            .FindColumnIdByStatus(task.Column.Board.BoardId, dto.currentColumn);
                task.ColumnId = newColumnId;
            }
                

            task.DateUpdated = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return task;
        }

        public async Task<Task?> GetTaskInfo(int taskId)
        {
            return await _db.Tasks
                            .Include(t => t.Column.Board)
                            .ThenInclude(b => b.Project)
                            .FirstOrDefaultAsync(t => t.TaskId == taskId);
        }

        public async Task<IEnumerable<Task>> GetTasksByBoardAsync(int boardId, int currentUserId)
        {
            var board = _boardService.GetBoardByIdAsync(boardId).Result;
            if (board == null)
                throw new KeyNotFoundException("Доска не найдена");
            var userRole = _userService.GetUserRoleFromProject(currentUserId, board.ProjectId).Result;
            if (userRole == null)
                throw new UnauthorizedAccessException("Вы не участвуете в проекте этой доски");
            return await _boardService.GetTasksByBoardAsync(boardId);
        }

        public async Task<Task> ResolveAssigneeUserRoleIdAsync(int taskId, int assigneeUserRoleId, int currentUserId)
        {
            var task = GetTaskInfo(taskId).Result;
            if (task == null)
                throw new KeyNotFoundException("Задача не найдена");

            var projectId = task.Column.Board.ProjectId;
            var currentUserRole = _userService.GetUserRoleFromProject(currentUserId, projectId).Result;
            if (currentUserRole == null)
                throw new UnauthorizedAccessException(
                    "Вы не участвуете в этом проекте");

            if (!currentUserRole.Role.Permissions.HasFlag(Permission.AssignTask))
                throw new UnauthorizedAccessException(
                    "У вас нет прав назначать задачи другим участникам");

            var assigneeRole = _userService.GetUserFromEntireSystemAsync(assigneeUserRoleId).Result;
            if (assigneeRole == null || assigneeRole.ProjectId != projectId)
                throw new KeyNotFoundException(
                    "Участник с такой ролью не найден в этом проекте");

            task.UserRoleId = assigneeRole.UserRoleId;
            task.DateUpdated = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return task;
        }
    }
}
