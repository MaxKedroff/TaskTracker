using Microsoft.AspNetCore.SignalR;

namespace TaskTracker.SignalRSockets
{
    public class CommentsHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            var taskId = http?.Request.Query["taskId"].ToString();
            if (!string.IsNullOrWhiteSpace(taskId))
                await Groups.AddToGroupAsync(Context.ConnectionId, taskId);

            await base.OnConnectedAsync();
        }
    }
}
