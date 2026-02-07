using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Factories;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Grc.ui.App.Controllers {

    public class GrcBaseController : Controller  {
        protected readonly IErrorService ErrorService;
        protected readonly IGrcErrorFactory ErrorFactory;
        protected readonly IApplicationLogger Logger;
        protected readonly IEnvironmentProvider Environment;
        protected readonly IWebHelper WebHelper;
        protected readonly ILocalizationService LocalizationService;
        protected readonly SessionManager SessionManager;
        public GrcBaseController(IApplicationLoggerFactory loggerFactory, 
                                 IEnvironmentProvider environment,
                                 IWebHelper webHelper,
                                 ILocalizationService localizationService,
                                 IErrorService errorService, 
                                 IGrcErrorFactory errorFactory,
                                 SessionManager sessionManager) {
            Logger = loggerFactory.CreateLogger();
            Environment = environment;
            WebHelper = webHelper;
            LocalizationService = localizationService;
            ErrorService = errorService;
            ErrorFactory = errorFactory;
            SessionManager = sessionManager;
        }
        
        protected async Task<GrcResponse<ServiceResponse>> ProcessErrorAsync(string message, string source, string stacktrace) {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var branch = SessionManager.GetWorkspace()?.AssignedBranch;
            Logger.LogActivity($"WORKSPACE BRANCH: {JsonSerializer.Serialize(branch)}");
            long conpanyId = branch?.CompanyId ?? 0;
            var errModel = await ErrorFactory.PrepareErrorModelAsync(conpanyId, message, source, stacktrace);
            Logger.LogActivity($"ERROR MODEL: {JsonSerializer.Serialize(errModel)}");
            var response = await ErrorService.SaveSystemErrorAsync(errModel, ipAddress);
            Logger.LogActivity($"RESPONSE: {JsonSerializer.Serialize(response)}");

            return response;
        }

        public void Notify(string message, string title = "GRC NOTIFICATION", NotificationType type=NotificationType.Success) {
            var notificationMessage = new NotificationMessage() {
                Title = title,
                Message = message,
                Icon = type.GetEnumMemberValue(),
                Type  = type.GetEnumMemberValue()
            };

            TempData["Message"] = JsonSerializer.Serialize(notificationMessage); 
        }
    }

}
