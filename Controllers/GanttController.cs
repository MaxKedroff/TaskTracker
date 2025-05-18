using Microsoft.AspNetCore.Mvc;
using TaskTracker.Models.DTO;
using TaskTracker.Service;

namespace TaskTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GanttController : ControllerBase
    {
        private readonly IGanttService _gantt;

        public GanttController(IGanttService gantt)
        {
            _gantt = gantt;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<GanttItemDTO>>> Get()
        {
            var items = await _gantt.GetGanttItemsAsync();
            return Ok(items);
        }
    }
}
