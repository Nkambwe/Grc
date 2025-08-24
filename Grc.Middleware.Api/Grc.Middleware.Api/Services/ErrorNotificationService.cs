using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.SignalR;

namespace Grc.Middleware.Api.Services {

    public class ErrorNotificationService : IErrorNotificationService {
        private readonly IServiceLogger Logger;
        private readonly IHubContext<SystemBugHub> _hubContext;
        public ErrorNotificationService(IHubContext<SystemBugHub> hubContext,
                                        IServiceLoggerFactory loggerFactory, 
                                        IUnitOfWorkFactory uowFactory) {
             _hubContext = hubContext;
            Logger = loggerFactory.CreateLogger();;
        }

        public async Task NotifyCountAsync(int total, int count) {
            try {
                var countUpdate = new {
                    TotalErrors = total,
                    TodayErrors = count,
                    LastUpdated = DateTime.UtcNow
                };

                await _hubContext.Clients.Group("AdminGroup").SendAsync("ErrorCountUpdated", countUpdate);
                Logger.LogActivity($"Error count update sent: Total={total}, Today={count}");
            } catch (Exception ex) {
                 Logger.LogActivity($"Failed to send error notification via SignalR: {ex.Message}", "ERROR");
            }
        }

        public async Task NotifyNewErrorAsync(SystemError errorModel) {
            try{
                // Send to admin group
                await _hubContext.Clients.Group("AdminGroup").SendAsync("NewErrorOccurred", errorModel);
                Logger.LogActivity($"Error notification sent to admin group: {errorModel.ErrorSource}", "ERROR");
            } catch (Exception ex) {
                 Logger.LogActivity($"Failed to send error notification via SignalR: {ex.Message}", "ERROR");
            }
        }
    }
}
