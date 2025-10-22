using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Services;
using Grc.Middleware.Api.Services.Organization;
using Grc.Middleware.Api.Utils;
using System.Text.Json;

namespace Grc.Middleware.Api.Helpers {
    public class GlobalExceptionMiddleware {

        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            try {
                await _next(context);
            } catch (Exception ex) {
                var loggerFactory = context.RequestServices.GetService<IServiceLoggerFactory>();
                var logger = loggerFactory?.CreateLogger();
                logger?.LogActivity($"An unhandled exception occurred: {ex.Message}", "ERROR");
                logger?.LogActivity($"Stack trace: {ex.StackTrace}", "STACKTRACE");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception) {
            try {
                // Get services
                var errorNotificationService = context.RequestServices.GetService<IErrorNotificationService>();
                var errorService = context.RequestServices.GetService<ISystemErrorService>();
                var compayService = context.RequestServices.GetService<ICompanyService>();

                long companyId = 0;
                if(compayService != null){ 
                    var company =  await compayService.GetDefaultCompanyAsync();
                    if(company != null){ 
                        companyId = company.Id;    
                    }
                }

                if (errorService != null) {
                    //..create error model
                    var errorModel = new SystemError {
                        ErrorMessage = exception.Message,
                        ErrorSource = $"GLOBAL_MIDDLEWARE_{context.Request.Path}",
                        StackTrace = exception.StackTrace,
                        CompanyId = companyId,
                        Assigned = false,
                        IsDeleted = false,
                        IsUserReported = false,
                        FixStatus = "OPEN",
                        Severity = "CRITICAL",
                        ReportedOn = DateTime.Now,
                        CreatedOn = DateTime.Now,
                        CreatedBy = "System"
                    };

                    //..save to database
                    var savedError = await errorService.SaveErrorAsync(errorModel);

                    //..send SignalR notification if error was saved
                    if (errorNotificationService != null) {
                        await errorNotificationService.NotifyNewErrorAsync(errorModel);

                        //..update counts
                        var errorCounts = await errorService.GetErrorCountsAsync(errorModel.CompanyId);
                        await errorNotificationService.NotifyCountAsync(
                            errorCounts.TotalBugs,
                            errorCounts.NewBugs
                        );
                    }
                }
            } catch (Exception ex) {
                var loggerFactory = context.RequestServices.GetService<IServiceLoggerFactory>();
                var logger = loggerFactory?.CreateLogger();
                logger?.LogActivity($"An unhandled exception occurred: {ex.Message}", "ERROR");
                logger?.LogActivity($"Stack trace: {ex.StackTrace}", "STACKTRACE");
                logger.LogActivity($"Failed to handle exception in global middleware: {ex}", "ERROR");
            }

            // Return error response
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var response = new {
                Status = false,
                StatusCode = 500,
                Message = "An internal server error occurred",
                Error = exception.Message
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }

    }
}
