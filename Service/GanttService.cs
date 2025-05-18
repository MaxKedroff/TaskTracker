using Microsoft.EntityFrameworkCore;
using TaskTracker.db;
using TaskTracker.Models.DTO;

namespace TaskTracker.Service
{
    public interface IGanttService
    {
        Task<IReadOnlyList<GanttItemDTO>> GetGanttItemsAsync();
    }

    public class GanttService : IGanttService
    {
        private readonly AppDbContext _db;
        public GanttService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GanttItemDTO>> GetGanttItemsAsync()
        {
            var epics = await _db.Tasks
                .Where(t => t.IsEpic)
                .Include(t => t.SubTasks)
                .ThenInclude(st => st.Task)
                .ToListAsync();

            var result = new List<GanttItemDTO>();
            foreach (var epic in epics)
            {
                result.Add(new GanttItemDTO
                {
                    Id = epic.TaskId,
                    Title = epic.Title,
                    Start = epic.DateCreated,
                    End = epic.Deadline,
                    ParentId = null,
                    IsEpic = true
                });

                foreach (var sub in epic.SubTasks)
                {
                    if (sub.Task == null)
                        continue;

                    result.Add(new GanttItemDTO
                    {
                        Id = sub.Task.TaskId,
                        Title = sub.Task.Title,
                        Start = sub.Task.DateCreated,
                        End = sub.Task.Deadline,
                        ParentId = epic.TaskId,
                        IsEpic = false
                    });

                }
            }
            return result;
        }
        

    }
}
