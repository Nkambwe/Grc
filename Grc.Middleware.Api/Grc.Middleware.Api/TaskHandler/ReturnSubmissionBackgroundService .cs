using Grc.Middleware.Api.Services.Compliance.Regulations;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.TaskHandler {
    public class ReturnSubmissionBackgroundService : BackgroundService {
        private readonly IServiceScopeFactory _scopeFactory;

        public ReturnSubmissionBackgroundService(IServiceScopeFactory scopeFactory) {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            using (var scope = _scopeFactory.CreateScope()) {
                var logger = scope.ServiceProvider.GetRequiredService<IServiceLoggerFactory>().CreateLogger();
                logger.LogActivity("Returns Submission service STARTED");
            }

            while (!stoppingToken.IsCancellationRequested) {
                using var scope = _scopeFactory.CreateScope();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<IServiceLoggerFactory>();
                var logger = loggerFactory.CreateLogger();

                try {
                    var service = scope.ServiceProvider.GetRequiredService<IReturnsSubmissionService>();
                    await service.GenerateMissingSubmissionsAsync(DateTime.UtcNow.Date, stoppingToken);
                } catch (Exception ex) {
                    logger.LogActivity($"ReturnSubmission background job failed {ex}", "ERROR");

                    var inner = ex.InnerException;
                    while (inner != null) {
                        logger.LogActivity($"Inner Exception: {inner.Message}", "ERROR");
                        inner = inner.InnerException;
                    }
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }

}
