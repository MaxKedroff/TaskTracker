using Microsoft.AspNetCore.Mvc;
using TaskTracker.Models.DTO;
using TaskTracker.Service;

namespace TaskTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly IMetricsService _metrics;

        public MetricsController(IMetricsService metrics)
            => _metrics = metrics;

        [HttpGet("returnRate")]
        public async Task<IActionResult> GetReturnRate(
            [FromBody] AskMetricsDTO dto
            )
        {
            if (dto.StartDate.Date > dto.EndDate.Date)
                return BadRequest("startDate must be on or before endDate.");
            var data = await _metrics.GetDailyReturnRateAsync(dto.StartDate, dto.EndDate);
            return Ok(data);
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs()
        {
            return Ok(_metrics.GetAllLogsAsync());
        }

        [HttpGet("team-load")]
        public async Task<IActionResult> GetTeamLoad([FromBody] AskMetricsDTO dto)
        {
            if (dto.StartDate > dto.EndDate)
                return BadRequest("startDate должен быть не позже endDate");

            var result = await _metrics.GetTeamLoadAsync(dto.StartDate, dto.EndDate);
            return Ok(result);
        }

        [HttpGet("velocity")]
        public async Task<IActionResult> GetWeeklyVelocity(
        [FromBody] AskMetricsDTO dto)
        {
            if (dto.StartDate.Date > dto.EndDate.Date)
                return BadRequest("startDate должен быть не позже endDate");

            var data = await _metrics.GetWeeklyVelocityAsync(dto.StartDate, dto.EndDate);
            return Ok(data);
        }

        [HttpGet("task-duration")]
        public async Task<IActionResult> GetTaskDurations(
        [FromBody] AskMetricsDTO dto)
        {
            if (dto.StartDate.Date > dto.EndDate.Date)
                return BadRequest("startDate должен быть не позже endDate");

            var result = await _metrics.GetTaskDurationsAsync(dto.StartDate, dto.EndDate);
            return Ok(result);
        }

        [HttpGet("user-progress")]
        public async Task<IActionResult> GetUserProgress([FromQuery] int projectId)
        {
            var data = await _metrics.GetUserTaskProgressAsync(projectId);
            return Ok(data);
        }
    }
}
