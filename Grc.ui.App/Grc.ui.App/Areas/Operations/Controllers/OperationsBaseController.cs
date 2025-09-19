using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Factories;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Grc.ui.App.Areas.Operations.Controllers {

    public class OperationsBaseController : Controller {
        protected readonly IErrorService ErrorService;
        protected readonly IGrcErrorFactory ErrorFactory;
        protected readonly IApplicationLogger Logger;
        protected readonly IEnvironmentProvider Environment;
        protected readonly IWebHelper WebHelper;
        protected readonly ILocalizationService LocalizationService;
        protected readonly SessionManager SessionManager;

        public OperationsBaseController(IApplicationLoggerFactory loggerFactory,
                                  IEnvironmentProvider environment,
                                  IWebHelper webHelper,
                                  ILocalizationService localizationService,
                                  IErrorService errorService,
                                  IGrcErrorFactory errorFactory,
                                  SessionManager sessionManager) {
            Logger = loggerFactory.CreateLogger("ops_controllers");
            Environment = environment;
            WebHelper = webHelper;
            LocalizationService = localizationService;
            ErrorService = errorService;
            ErrorFactory = errorFactory;
            SessionManager = sessionManager;
        }

        /// <summary>
        /// Method used to process and capture errors to the database
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="source">Error source</param>
        /// <param name="stacktrace">Stack trace for the error</param>
        /// <returns></returns>
        protected async Task<GrcResponse<ServiceResponse>> ProcessErrorAsync(string message, string source, string stacktrace) {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var branch = SessionManager.GetWorkspace()?.AssignedBranch;
            Logger.LogActivity($"WORKSPACE BRANCH: {JsonSerializer.Serialize(branch)}");
            long conpanyId = branch?.OrganizationId ?? 0;
            var errModel = await ErrorFactory.PrepareErrorModelAsync(conpanyId, message, source, stacktrace);
            Logger.LogActivity($"ERROR MODEL: {JsonSerializer.Serialize(errModel)}");
            var response = await ErrorService.SaveSystemErrorAsync(errModel, ipAddress);
            Logger.LogActivity($"RESPONSE: {JsonSerializer.Serialize(response)}");

            return response;
        }
    }
}
