using Microsoft.EntityFrameworkCore;
using TaskTracker.db;
using TaskTracker.Models;
using TaskTracker.Models.DTO;

namespace TaskTracker.Service
{
    public interface IMetricsService
    {
        Task<IEnumerable<DailyReturnRateDTO>> GetDailyReturnRateAsync(DateTime startDate, DateTime endDate);
        Task<List<TaskHistory>> GetAllLogsAsync();
    }

    public class MetricsService : IMetricsService
    {
        private readonly AppDbContext _db;
        private const string DoneStatus = "Complete";
        private static readonly string[] ReturnedToStatuses = { "In-Progress", "Todo" };
        public MetricsService(AppDbContext db) => _db = db;

        public async Task<IEnumerable<DailyReturnRateDTO>> GetDailyReturnRateAsync(DateTime startDate, DateTime endDate)
        {
            var completedPerDay = await _db.Tasks
                .Where(t =>
                    t.Status != null &&
                    t.Status.Title == DoneStatus &&
                    t.DateUpdated >= startDate && t.DateUpdated <= endDate)
                .GroupBy(t => new { t.DateUpdated.Year, t.DateUpdated.Month, t.DateUpdated.Day })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    Count = g.Count()
                })
                .ToListAsync();

            var returnedPerDay = await _db.TaskHistories
                .Where(h =>
                h.OldStatus == DoneStatus &&
                ReturnedToStatuses.Contains(h.NewStatus) &&
                h.ChangeDate >= startDate && h.ChangeDate <= endDate)
                .GroupBy(h => new { h.ChangeDate.Year, h.ChangeDate.Month, h.ChangeDate.Day })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    Count = g.Count()
                })
                .ToListAsync();

            var compDict = completedPerDay.ToDictionary(x => x.Date, x => x.Count);
            var retDict = returnedPerDay.ToDictionary(x => x.Date, x => x.Count);

            var result = new List<DailyReturnRateDTO>();
            for (var day = startDate.Date; day <= endDate.Date; day = day.AddDays(1))
            {
                compDict.TryGetValue(day, out var c);
                retDict.TryGetValue(day, out var r);
                var rate = c > 0 ? (double)r / c * 100 : 0.0;

                result.Add(new DailyReturnRateDTO
                {
                    Date = day,
                    CompletedTasks = c,
                    ReturnedTasks = r,
                    ReturnRate = Math.Round(rate, 2)
                });
            }

            return result;
        }

        public async Task<List<TaskHistory>> GetAllLogsAsync()
        {
            // При желании: .Include(h => h.Task) чтобы подтянуть данные самой задачи
            return await _db.TaskHistories
                .Include(h => h.Task)
                            .OrderBy(h => h.ChangeDate)
                            .ToListAsync();
        }
    }
}
