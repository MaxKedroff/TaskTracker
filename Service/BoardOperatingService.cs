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
            var board = await _context.Boards.Include(b => b.Project).FirstOrDefaultAsync(b => b.BoardId == boardId);
            return board;
        }

        public async Task<Board> CreateNewBoardAsync(CreateBoardDTO dto, int currentUserId)
        {
            if (dto.StartDate > dto.EndDate)
                throw new ArgumentException("StartDate не может быть позже EndDate");

            var project = _projectService.GetProjectById(dto.ProjectId);

            if (project == null)
                throw new KeyNotFoundException("Проект не найден");



            if (!_userService.IsAdmin(currentUserId, dto.ProjectId).Result)
                throw new UnauthorizedAccessException("У вас нет прав для создания доски в этом проекте");
            var board = new Board
            {
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ProjectId = dto.ProjectId,
            };
            _context.Boards.Add(board);
            await _context.SaveChangesAsync();

            return board;
        }

        public async Task<IEnumerable<Models.Task>> GetTasksByBoardAsync(int boardId)
        {
            return await _context.Tasks
                            .Where(t => t.BoardId == boardId)
                            .ToListAsync();
        }
    }
}
