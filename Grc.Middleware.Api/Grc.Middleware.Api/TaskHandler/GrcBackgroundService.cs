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
                logger.LogActivity("GRC Background Service STARTED", "INFO");
            }

            await Task.WhenAll(ProcessQueuedTasks(stoppingToken),RunReturnSubmissionJob(stoppingToken),SendPolicyNotificationJobAsync(stoppingToken));
        }

        private async Task SendPolicyNotificationJobAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                using var scope = _scopeFactory.CreateScope();
                var logger = scope.ServiceProvider
                    .GetRequiredService<IServiceLoggerFactory>()
                    .CreateLogger();

                try {
                    var policyService = scope.ServiceProvider.GetRequiredService<IRegulatoryDocumentService>();

                    logger.LogActivity($"Policy notification job started at {DateTime.Now:yyyy-MM-dd HH:mm:ss}", "INFO");
                    await policyService.SendNotificationMailsAsync();
                    logger.LogActivity($"Policy notification job completed at {DateTime.Now:yyyy-MM-dd HH:mm:ss}", "INFO");

                } catch (Exception ex) {
                    logger.LogActivity($"Policy notification job failed: {ex}", "ERROR");
                }

                //..run every 6 hours
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }
        }

        private async Task RunReturnSubmissionJob(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                using var scope = _scopeFactory.CreateScope();
                var logger = scope.ServiceProvider
                    .GetRequiredService<IServiceLoggerFactory>()
                    .CreateLogger();

                try {
                    var submissionService = scope.ServiceProvider.GetRequiredService<IReturnsSubmissionService>();

                    logger.LogActivity($"Generate missing submissions {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    await submissionService.GenerateMissingSubmissionsAsync(DateTime.UtcNow.Date, stoppingToken);
                } catch (Exception ex) {
                    logger.LogActivity($"ReturnSubmission job failed {ex}", "ERROR");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task ProcessQueuedTasks(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                try {
                    var workItem = await _mailTaskQueue.DequeueAsync(stoppingToken);

                    using var scope = _scopeFactory.CreateScope();
                    var logger = scope.ServiceProvider.GetRequiredService<IServiceLoggerFactory>().CreateLogger();

                    logger.LogActivity("Processing queued mail task", "DEBUG");

                    try {
                        await workItem(scope.ServiceProvider, stoppingToken);
                    } catch (Exception ex) {
                        logger.LogActivity($"Background mail task failed: {ex}", "ERROR");
                    }
                } catch (OperationCanceledException) {
                    //..normal shutdown, don't log as error
                    break;
                } catch (Exception ex) {
                    //..log unexpected errors but keep the loop running
                    using var scope = _scopeFactory.CreateScope();
                    var logger = scope.ServiceProvider.GetRequiredService<IServiceLoggerFactory>().CreateLogger();
                    logger.LogActivity($"Critical error in mail queue processing: {ex}", "ERROR");

                    //..add a small delay to prevent tight error loops
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }
    }

}
