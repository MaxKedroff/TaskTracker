using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTracker.db;

namespace TaskTracker.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _db;
        public RoleController(AppDbContext db) => _db = db;

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IEnumerable<RoleDTO>> GetAll()
        {
            return await _db.Roles
            .Select(r => new RoleDTO
            {
                RoleId = r.RoleId,
                Title = r.Title,
                Permissions = (long)r.Permissions
            })
            .ToListAsync();
        }
    }

    public class RoleDTO
    {
        public int RoleId { get; set; }
        public string Title { get; set; }
        public long Permissions { get; set; }
    }
}
