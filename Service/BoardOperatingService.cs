using TaskTracker.Models.DTO;
using TaskTracker.Models;
using TaskTracker.db;
using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Service
{

    public interface IBoardOperateService
    {

        Task<Board> CreateNewBoardAsync(CreateBoardDTO dto, int currentUserId);
        Task<Board> GetBoardByIdAsync(int boardId);

        Task<int> FindColumnIdByStatus(int boardId, string status);

        Task<IEnumerable<Models.Task>> GetTasksByBoardAsync(int boardId);
    }

    public class BoardOperatingService : IBoardOperateService
    {
        private readonly AppDbContext _context;
        private readonly IProjectOperateService _projectService;
        private readonly IUserService _userService;

        public BoardOperatingService(AppDbContext context, IProjectOperateService projectService, IUserService userService )
        {
            _context = context;
            _projectService = projectService;
            _userService = userService;
        }

        public async Task<Board?> GetBoardByIdAsync(int boardId)
        {
            var board = await _context.Boards.Include(b => b.Project).Include(b => b.Columns).FirstOrDefaultAsync(b => b.BoardId == boardId);
            return board;
        }

        public async Task<Board> CreateNewBoardAsync(CreateBoardDTO dto, int currentUserId)
        {
            if (dto.StartDate > dto.EndDate)
                throw new ArgumentException("StartDate не может быть позже EndDate");

            var project = _projectService.GetProjectById(dto.ProjectId).Result;

            if (project == null)
                throw new KeyNotFoundException("Проект не найден");



            if (!_userService.IsAdmin(currentUserId, dto.ProjectId).Result)
                throw new UnauthorizedAccessException("У вас нет прав для создания доски в этом проекте");

            var defaultColumns = new List<Column>
            {
                new Column { Title = "Артефакты",    Color = "#6c757d" },
                new Column { Title = "Новые задачи", Color = "#6c757d" },
                new Column { Title = "В работе",     Color = "#6c757d" }, 
                new Column { Title = "Готово",       Color = "#6c757d" } 
            };

            var board = new Board
            {
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ProjectId = dto.ProjectId,
                Project = project,
                Columns = defaultColumns
            };
            if (project.Boards == null)
            {
                project.Boards = new List<Board>();
                project.Boards.Add(board);
            }


            _context.Projects.Update(project);
            _context.Boards.Add(board);
            await _context.SaveChangesAsync();

            return board;
        }

        public async Task<IEnumerable<Models.Task>> GetTasksByBoardAsync(int boardId)
        {
            return await _context.Tasks
                            .Where(t => t.Column.Board.BoardId == boardId)
                            .ToListAsync();
        }





        public async Task<int> FindColumnIdByStatus(int boardId, string status)
        {
            // Ищем ColumnID напрямую в таблице Columns
            var columnId = await _context.Columns
                .Where(c => c.Board.BoardId == boardId && c.Title == status)
                .Select(c => c.ColumnID)
                .FirstOrDefaultAsync();

            if (columnId == default)
            {
                throw new InvalidOperationException(
                    $"Column with title '{status}' not found in board (ID = {boardId}).");
            }

            return columnId;
        }
    }
}
