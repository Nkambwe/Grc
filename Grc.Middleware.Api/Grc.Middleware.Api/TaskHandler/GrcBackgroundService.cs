using Grc.Middleware.Api.Services.Compliance.Regulations;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.TaskHandler {
    public class GrcBackgroundService : BackgroundService {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMailTaskQueue _mailTaskQueue;
        public GrcBackgroundService(IServiceScopeFactory scopeFactory, IMailTaskQueue mailTaskQueue) {
            _scopeFactory = scopeFactory;
            _mailTaskQueue = mailTaskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            using (var scope = _scopeFactory.CreateScope()) {
                var logger = scope.ServiceProvider.GetRequiredService<IServiceLoggerFactory>().CreateLogger();
                logger.LogActivity("Returns Submission service STARTED");
            }

            //..execute background work
            await Task.WhenAll(ProcessQueuedTasks(stoppingToken), RunReturnSubmissionJob(stoppingToken));
        }

        private async Task RunReturnSubmissionJob(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                using var scope = _scopeFactory.CreateScope();
                var logger = scope.ServiceProvider
                    .GetRequiredService<IServiceLoggerFactory>()
                    .CreateLogger();

                try {
                    var service = scope.ServiceProvider
                        .GetRequiredService<IReturnsSubmissionService>();

                    await service.GenerateMissingSubmissionsAsync(
                        DateTime.UtcNow.Date, stoppingToken);
                } catch (Exception ex) {
                    logger.LogActivity($"ReturnSubmission job failed {ex}", "ERROR");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }


        private async Task ProcessQueuedTasks(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                var workItem = await _mailTaskQueue.DequeueAsync(stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                try{
                    await workItem(scope.ServiceProvider, stoppingToken);
                } catch (Exception ex) {
                    var logger = scope.ServiceProvider
                        .GetRequiredService<IServiceLoggerFactory>()
                        .CreateLogger();

                    logger.LogActivity($"Background task failed: {ex}", "ERROR");
                }
            }
        }

    }

}
