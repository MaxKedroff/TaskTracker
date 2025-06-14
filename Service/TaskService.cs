using TaskTracker.Models.DTO;
using TaskTracker.Models;
using Task = TaskTracker.Models.Task;
using TaskTracker.db;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Net.WebSockets;

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

        Task<Task> CreateNewEpicAsync(CreateEpicDTO dto, int currentUserId);

        /// <summary>Пометить существующую задачу как эпик</summary>
        Task<Task> MarkTaskAsEpicAsync(int taskId, int currentUserId);

        Task<Task> CreateEpicAsync(CreateTaskDTO dto, int currentUserId);

        /// <summary>Привязать задачу к эпику</summary>
        Task<Task> AttachTaskToEpicAsync(int epicId, int subTaskId, int currentUserId);

        Task<Defect> CreateNewDefect(CreateDefectDTO defectDTO, int currentUserId);

        List<Task> GetTasksByUser(int currentUserId);


        List<Task> GetTasksByProjectAsync(int projectId, int currentUserId);

    }

    public class TaskService : ITaskService
    {
        private readonly AppDbContext _db;
        private readonly IBoardOperateService _boardService;
        private readonly IProjectOperateService _projectService;
        private readonly IUserService _userService;

        public TaskService(AppDbContext db, IBoardOperateService boardService, IUserService userService, IProjectOperateService projectService)
        {
            _db = db;
            _userService = userService;
            _boardService = boardService;
            _projectService = projectService;
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
                UserRoleId = dto.AssignedUserRoleId
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
            {
                task.StatusId = dto.statusId;
                _db.Entry(task).Property(t => t.StatusId).IsModified = true;

            }
            if (!string.IsNullOrEmpty(dto.Color))
            {
                task.color = dto.Color;
            }
            if (!string.IsNullOrWhiteSpace(dto.priorityId.ToString()))
                task.PriorityId = dto.priorityId;
            if (!string.IsNullOrEmpty(dto.currentColumn))
            {
                var board = task.Column?.Board;
                if (board == null)
                    throw new InvalidOperationException("Доска задачи не найдена");

                if (board.Columns == null)
                    throw new InvalidOperationException("Колонки доски не загружены");

                var newColumn = board.Columns.FirstOrDefault(c => c.Title == dto.currentColumn);
                if (newColumn == null)
                    throw new KeyNotFoundException($"Колонка с названием '{dto.currentColumn}' не найдена, доступные для перетаскивания колонки {board.Columns}");

                task.ColumnId = newColumn.ColumnID;
                if (dto.currentColumn == "Готово")
                {
                    task.endDate = DateTime.UtcNow;
                    task.IsDone = true;
                }
            }


            task.DateUpdated = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return task;
        }

        public async Task<Task?> GetTaskInfo(int taskId)
        {
            return await _db.Tasks
                            .Include(t => t.Column)
                            .ThenInclude(t => t.Board)
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

        public async Task<Task> MarkTaskAsEpicAsync(int taskId, int currentUserId)
        {
            // 1) Загрузим задачу с колонкой → доской → проектом
            var task = await _db.Tasks
                .Include(t => t.Column).ThenInclude(c => c.Board)
                .FirstOrDefaultAsync(t => t.TaskId == taskId);

            if (task == null)
                throw new KeyNotFoundException("Задача не найдена");

            // 2) Проверим, что пользователь состоит в проекте и имеет право EDIT
            var projectId = task.Column.Board.ProjectId;
            var ur = await _userService.GetUserRoleFromProject(currentUserId, projectId);
            if (ur == null || !ur.Role.Permissions.HasFlag(Permission.EditTask))
                throw new UnauthorizedAccessException("Недостаточно прав для пометки эпиком");

            // 3) Ставим флаг и сохраняем
            task.IsEpic = true;
            task.DateUpdated = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return task;
        }

        public async Task<Task> AttachTaskToEpicAsync(int epicId, int subTaskId, int currentUserId)
        {
            // 1) Загрузим эпик
            var epic = await _db.Tasks
                .Include(t => t.Column).ThenInclude(c => c.Board)
                .Include(t => t.SubTasks)            // чтобы потом вернуть с уже вложенными
                    .ThenInclude(st => st.Task)
                .FirstOrDefaultAsync(t => t.TaskId == epicId);

            if (epic == null)
                throw new KeyNotFoundException("Эпик не найден");

            if (!epic.IsEpic)
                throw new InvalidOperationException("Задача не помечена как эпик");

            // 2) Загрузим подпадающую задачу
            var sub = await _db.Tasks.FindAsync(subTaskId);
            if (sub == null)
                throw new KeyNotFoundException("Подзадача не найдена");

            // 3) Проверим доступ того же пользователя
            var projectId = epic.Column.Board.ProjectId;
            var ur = await _userService.GetUserRoleFromProject(currentUserId, projectId);
            if (ur == null || !ur.Role.Permissions.HasFlag(Permission.EditTask))
                throw new UnauthorizedAccessException("Недостаточно прав для привязки к эпику");

            // 4) Записать связь в таблицу SubTasks
            var link = new SubTask
            {
                TaskId = epicId,
                SubtaskId = subTaskId
            };
            _db.Subtasks.Add(link);

            // 5) Обновим дату эпика
            epic.DateUpdated = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // 6) Перезагрузим коллекцию подзадач, чтобы вернуть актуальный список
            await _db.Entry(epic)
                     .Collection(e => e.SubTasks)
                     .Query()
                     .Include(st => st.Task)
                     .LoadAsync();

            return epic;
        }

        public async Task<Task> CreateNewEpicAsync(CreateEpicDTO dto, int currentUserId)
        {
            // Проверяем доску
            var board = await _boardService.GetBoardByIdAsync(dto.BoardId);
            if (board == null)
                throw new KeyNotFoundException("Доска не найдена");

            // Проверяем права
            var userRole = await _userService.GetUserRoleFromProject(currentUserId, board.ProjectId);
            if (userRole == null || !userRole.Role.Permissions.HasFlag(Permission.CreateTask))
                throw new UnauthorizedAccessException("У вас нет прав на создание эпиков в этом проекте");

            // Найти колонку по имени
            var columnId = await _boardService.FindColumnIdByStatus(dto.BoardId, dto.CurrentColumn);

            // Собираем модель
            var epic = new Task
            {
                Title = dto.Title,
                Description = dto.Description,
                Deadline = dto.Deadline,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                StatusId = dto.StatusId,
                PriorityId = dto.PriorityId,
                UserRoleId = dto.AssignedUserRoleId,
                ColumnId = columnId,
                IsEpic = true
            };

            _db.Tasks.Add(epic);
            await _db.SaveChangesAsync();
            return epic;
        }

        public async Task<Defect> CreateNewDefect(CreateDefectDTO defectDTO, int currentUserId)
        {
            var project = _projectService.GetProjectById(defectDTO.projectId);
            if (project == null)
                throw new KeyNotFoundException("Проект не найден");
            var board = _boardService.GetBoardByIdAsync(defectDTO.boardId).Result;
            if (board == null)
                throw new KeyNotFoundException("Доска не найдена");
            var userRole = _userService.GetUserRoleFromProject(currentUserId, board.ProjectId).Result;
            var hasPermission = userRole.Role.Permissions.HasFlag(Permission.CreateTask);
            if (userRole == null || !userRole.Role.Permissions.HasFlag(Permission.CreateTask))
            {
                throw new UnauthorizedAccessException(
                    "У вас нет прав на создание задач в этом проекте");
            }
            var defect = new Defect
            {
                Title = defectDTO.Title,
                Description = defectDTO.Description,
                DateUpdated = DateTime.UtcNow,
                Status = defectDTO.Status,
                Priority = defectDTO.Priority,
                Severity = defectDTO.Severity,
                StartDate = defectDTO.StartDate,
                EndDate = defectDTO.EndDate,
                ColumnId = defectDTO.ColumnId,
            };

            _db.Defects.Add(defect);
            await _db.SaveChangesAsync();
            return defect;
        }

        public List<Task> GetTasksByUser(int currentUserId)
        {
            return [.. _db.UserRoles
                      .Include(ur => ur.Tasks)          // загружаем связанные задачи
                      .Where(ur => ur.UserId == currentUserId)
                      .SelectMany(ur => ur.Tasks)];                        // ← никогда не вернёт null
        }

        public async Task<Task> CreateEpicAsync(CreateTaskDTO dto, int currentUserId)
        {
            var board = _boardService.GetBoardByIdAsync(dto.BoardId).Result;
            var userRole = _userService.GetUserRoleFromProject(currentUserId, board.ProjectId).Result;
            var hasPermission = userRole.Role.Permissions.HasFlag(Permission.CreateTask);
            if (userRole == null || !userRole.Role.Permissions.HasFlag(Permission.CreateTask))
            {
                throw new UnauthorizedAccessException(
                    "У вас нет прав на создание Эпиков в этом проекте");
            }

            var task = new Task
            {
                Title = dto.Title,
                Description = dto.Description,
                Deadline = dto.Deadline,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                StatusId = dto.statusId,
                ColumnId = dto.columnId,
                PriorityId = dto.priorityId,
                UserRoleId = dto.AssignedUserRoleId,
                IsEpic = true
            };

            await _db.SaveChangesAsync();
            return task;
        }


        List<Task> ITaskService.GetTasksByProjectAsync(int projectId, int currentUserId)
        {
            var project = _db.Projects
                .Include(p => p.Boards)
                    .ThenInclude(b => b.Columns)
                        .ThenInclude(c => c.Tasks)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);



            // Проверяем, что Boards не null


            var tasks = new List<Task>();

            foreach (var board in project.Result.Boards)
            {
                // Проверяем, что Columns не null
                if (board.Columns == null) continue;

                foreach (var column in board.Columns)
                {
                    // Проверяем, что Tasks не null
                    if (column.Tasks != null)
                    {
                        tasks.AddRange(column.Tasks);
                    }
                }
            }

            return tasks;
        }
    }
}
