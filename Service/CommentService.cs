using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskTracker.db;
using TaskTracker.Models;
using TaskTracker.Models.DTO;
using TaskTracker.SignalRSockets;

namespace TaskTracker.Service
{
    public interface ICommentService
    {
        Task<Comment> LeftCommentAsync(LeftCommentDTO dto, int userId, CancellationToken ct = default);
        Task<IReadOnlyList<Comment>> GetCommentsByTaskAsync(int taskId, CancellationToken ct = default);
    }

    public class CommentService : ICommentService
    {

        private readonly AppDbContext _db;
        private readonly IUserService _users;
        private readonly IHubContext<CommentsHub> _hub;

        public CommentService(AppDbContext db,
                              IUserService users,
                              IHubContext<CommentsHub> hub)
        {
            _db = db;
            _users = users;
            _hub = hub;
        }

        public async Task<IReadOnlyList<Comment>> GetCommentsByTaskAsync(int taskId, CancellationToken ct)
        {
            return await _db.Comments
                .Where(c => c.TaskId == taskId)
                .Include(c => c.Author)
                .OrderBy(c => c.DateCreated)
                .ToListAsync();
        }

        public async Task<Comment> LeftCommentAsync(LeftCommentDTO dto, int userId, CancellationToken ct)
        {
            var task = await _db.Tasks.FindAsync(new object[] { dto.taskId }, ct)
                    ?? throw new KeyNotFoundException($"Task {dto.taskId} not found");

            var user = await _users.GetUserByUserIdAsync(userId)
                    ?? throw new KeyNotFoundException($"User {userId} not found");

            var comment = new Comment
            {
                TaskId = dto.taskId,
                AuthorId = userId,
                Text = dto.text,
                DateCreated = DateTime.Now
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            await _hub.Clients.Group(dto.taskId.ToString())
                .SendAsync("ReceiveComment", new
                {
                    comment.CommentId,
                    comment.TaskId,
                    comment.Text,
                    comment.DateCreated
                }, ct);
            return comment;
        }
    }
}
