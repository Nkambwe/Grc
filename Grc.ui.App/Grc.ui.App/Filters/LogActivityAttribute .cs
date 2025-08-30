using Grc.ui.App.Infrastructure;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace Grc.ui.App.Filters {

    public class LogActivityResultAttribute : Attribute, IAsyncResultFilter {
        private readonly string _activityType;
        private readonly string _comment;
        private readonly string _systemKeyword;
        private readonly string _entityName;

        public LogActivityResultAttribute(string activityType, string comment = null, string systemKeyword = null, string entityName = null) {
            _activityType = activityType;
            _comment = comment;
            _systemKeyword = systemKeyword;
            _entityName = entityName;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            await next();

            IApplicationLogger logger = null;
            try {
                //..create logger
                var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<IApplicationLoggerFactory>();
                logger = loggerFactory.CreateLogger();

                //..get user IP Address
                var webHelper = context.HttpContext.RequestServices.GetRequiredService<IWebHelper>();
                string ipAddress = webHelper.GetCurrentIpAddress();
                
                //..get user info
                var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();
                var grcResponse = await authService.GetCurrentUserAsync(ipAddress);
                
                if (grcResponse.HasError) {
                    logger.LogActivity($"ACTIVITY LOG ERROR: Failed to get current user - {JsonSerializer.Serialize(grcResponse)}");
                    return;
                }
                
                var userId = grcResponse.Data.UserId;
                var activityService = context.HttpContext.RequestServices.GetRequiredService<ISystemActivityService>();
                
                var comment = _comment ?? $"{context.ActionDescriptor.DisplayName} Successfully completed";
                await activityService.InsertActivityAsync(userId, _activityType, comment, _systemKeyword, _entityName, ipAddress);

                //...activity logging
                logger.LogActivity($"ACTIVITY LOGGED: {_activityType} for user {userId}");
            
            } catch (Exception ex) {
                if (logger != null) {
                    logger.LogActivity($"ACTIVITY LOG EXCEPTION: Failed to log activity {_activityType} - {JsonSerializer.Serialize(new { 
                        Error = ex.Message,
                        ex.StackTrace,
                        ActivityType = _activityType,
                        SystemKeyword = _systemKeyword,
                        EntityName = _entityName
                    })}");
                } else {
                    //..fallback to default logger if custom logger creation failed
                    var fallbackLogger = context.HttpContext.RequestServices.GetService<ILogger<LogActivityResultAttribute>>();
                    fallbackLogger?.LogError(ex, "Failed to log activity: {ActivityType}", _activityType);
                }
            }
        }
    }
}
