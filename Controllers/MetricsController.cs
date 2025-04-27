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
    }
}
