using Microsoft.EntityFrameworkCore;
using TaskTracker.db;
using TaskTracker.Models;
using TaskTracker.Models.DTO;

namespace TaskTracker.Service
{
    public interface IMetricsService
    {
        Task<IEnumerable<DailyReturnRateDTO>> GetDailyReturnRateAsync(DateTime startDate, DateTime endDate);

        Task<IEnumerable<TeamLoadDTO>> GetTeamLoadAsync(DateTime startDate, DateTime endDate);

        Task<IEnumerable<WeeklyVelocityDTO>> GetWeeklyVelocityAsync(DateTime startDate, DateTime endDate);

        Task<TaskDurationResultDTO> GetTaskDurationsAsync(DateTime startDate, DateTime endDate);

        Task<IEnumerable<UserTaskProgressDTO>> GetUserTaskProgressAsync(int projectId);

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
            return await _db.TaskHistories
                .Include(h => h.Task)
                            .OrderBy(h => h.ChangeDate)
                            .ToListAsync();
        }

        public async Task<IEnumerable<TeamLoadDTO>> GetTeamLoadAsync(DateTime startDate, DateTime endDate)
        {
            var data = await _db.Tasks
                .Where(t =>
                t.UserRoleId != null &&
                t.DateCreated >= startDate &&
                t.DateCreated <= endDate)
                .GroupBy(t => t.UserRole!.User.User_name)
                .Select(g => new TeamLoadDTO
                {
                    UserName = g.Key,
                    TaskCount = g.Count()
                })
                .ToListAsync();

            return data;
        }

        public async Task<IEnumerable<WeeklyVelocityDTO>> GetWeeklyVelocityAsync(DateTime startDate, DateTime endDate)
        {
            var completed = await _db.Tasks
                .Where(t =>
                t.Status != null &&
                t.Status.Title == DoneStatus &&
                t.DateUpdated >= startDate &&
                t.DateUpdated <= endDate)
                .Select(t => t.DateUpdated)
                .ToListAsync();

            var cursor = startDate.Date;
            var diff = (int)cursor.DayOfWeek - (int)DayOfWeek.Monday;
            if (diff < 0) diff += 7;
            var weekStart = cursor.AddDays(-diff);

            var result = new List<WeeklyVelocityDTO>();
            while (weekStart <= endDate.Date)
            {
                var weekEnd = weekStart.AddDays(6);
                // реальный диапазон внутри [startDate..endDate]
                var periodStart = weekStart < startDate.Date ? startDate.Date : weekStart;
                var periodEnd = weekEnd > endDate.Date ? endDate.Date : weekEnd;

                var days = (periodEnd - periodStart).Days + 1;
                var count = completed
                    .Count(d => d.Date >= periodStart && d.Date <= periodEnd);

                var velocity = days > 0 ? (double)count / days : 0.0;

                result.Add(new WeeklyVelocityDTO
                {
                    WeekStart = weekStart,
                    TasksCompleted = count,
                    DaysInPeriod = days,
                    Velocity = Math.Round(velocity, 2)
                });

                weekStart = weekStart.AddDays(7);
            }
            return result;
        }

        public async Task<TaskDurationResultDTO> GetTaskDurationsAsync(DateTime startDate, DateTime endDate)
        {
            // 1) Вытягиваем из БД только то, что нужно — идентификатор задачи и её даты
            var raw = await _db.Tasks
                .Where(t =>
                    t.Status != null &&
                    t.Status.Title == "Complete" &&
                    t.DateUpdated >= startDate &&
                    t.DateUpdated <= endDate)
                .Select(t => new
                {
                    t.TaskId,
                    Start = t.DateCreated,
                    End = t.DateUpdated
                })
                .ToListAsync();

            // 2) Считаем длительности в днях целочисленно
            var list = raw
                .Select(x => new TaskDurationDTO
                {
                    TaskId = x.TaskId,
                    DurationDays = (int)(x.End.Date - x.Start.Date).TotalDays,
                    IsAnomaly = false   // пометим ниже
                })
                .ToList();

            // 3) Если нет завершённых задач — сразу вернём «пустой» результат
            if (!list.Any())
            {
                return new TaskDurationResultDTO
                {
                    AverageDurationDays = 0,
                    Durations = Array.Empty<TaskDurationDTO>()
                };
            }

            // 4) Считаем среднее
            var avg = list.Average(d => d.DurationDays);

            // 5) Определяем порог аномалий (больше 125% от среднего)
            var threshold = avg * 1.25;

            // 6) Помечаем аномалии
            foreach (var dto in list)
            {
                dto.IsAnomaly = dto.DurationDays > threshold;
            }

            // 7) Собираем и возвращаем результат
            return new TaskDurationResultDTO
            {
                AverageDurationDays = Math.Round(avg, 2),
                Durations = list
            };
        }

        public async Task<IEnumerable<UserTaskProgressDTO>> GetUserTaskProgressAsync(int projectId)
        {
            var today = DateTime.UtcNow.Date;
            var userRoles = await _db.UserRoles
            .Include(ur => ur.User)
            .Where(ur => ur.ProjectId == projectId)
            .ToListAsync();

            var tasks = await _db.Tasks
            .Where(t => t.Column.Board.ProjectId == projectId)
            .Select(t => new
            {
                t.UserRoleId,
                StatusId = t.StatusId ?? 0,
                Deadline = t.Deadline.Date
            })
            .ToListAsync();
            var result = userRoles
                .Select(ur =>
                {
                var userTasks = tasks.Where(t => t.UserRoleId == ur.UserRoleId);
                var complete = userTasks.Count(t => t.StatusId == 3);
                var inProgress = userTasks.Count(t => t.StatusId == 2);
                var overdue = userTasks.Count(t => t.Deadline < today);
                var total = userTasks.Count();

                    var pct = total > 0 ? Math.Round((double)complete / total * 100, 2) : 0.0;
                    var user = ur.User.User_name;
                    return new UserTaskProgressDTO
                    {
                        DisplayName = user,
                        CompleteCount=complete,
                        InProgressCount=inProgress,
                        OverdueCount=overdue,
                        TotalCount = total,
                        PercentComplete=pct
                    };
                })
                .OrderBy(x => x.DisplayName)
                .ToList();
            return result;
        }
    }
}
