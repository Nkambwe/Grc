using Microsoft.AspNetCore.SignalR;

namespace Grc.Middleware.Api {

    public class SystemBugHub : Hub {

        private readonly ILogger<SystemBugHub> _logger;

        public SystemBugHub(ILogger<SystemBugHub> logger) {
            _logger = logger;
        }

        public override async Task OnConnectedAsync() {
            var userRole = Context.User?.FindFirst("role")?.Value;
            if (userRole == "Administrator" || userRole == "Support") {
                await Groups.AddToGroupAsync(Context.ConnectionId, "AdminGroup");
                _logger.LogInformation($"Admin user connected to SystemBugHub: {Context.ConnectionId}");
            }
        
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            _logger.LogInformation($"User disconnected from SystemBugHub: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinAdminGroup() {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AdminGroup");
        }

        public async Task LeaveAdminGroup() {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminGroup");
        }
    }

}
