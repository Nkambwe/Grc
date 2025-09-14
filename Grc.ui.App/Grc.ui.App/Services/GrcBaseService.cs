using AutoMapper;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Factories;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Utils;
using System.Text.Json;

namespace Grc.ui.App.Services {

    public class GrcBaseService : IGrcBaseService {
        protected readonly IApplicationLogger Logger;
        protected readonly IEnvironmentProvider Environment;
        protected readonly IEndpointTypeProvider EndpointProvider;
        protected readonly JsonSerializerOptions JsonOptions;
        protected readonly IMapper Mapper;
        protected readonly IWebHelper WebHelper;
        protected readonly IHttpHandler HttpHandler;
        protected readonly SessionManager SessionManager;
        protected readonly IGrcErrorFactory ErrorFactory;
        protected readonly IErrorService ErrorService;
        public GrcBaseService(IApplicationLoggerFactory loggerFactory, 
                              IHttpHandler httpHandler,
                              IEnvironmentProvider environment,
                              IEndpointTypeProvider endpointType,
                              IMapper mapper,
                              IWebHelper webHelper,
                              SessionManager sessionManager,
                              IGrcErrorFactory errorFactory,
                              IErrorService errorService) {
            Logger = loggerFactory.CreateLogger();
            Environment = environment;
            EndpointProvider = endpointType;
            HttpHandler = httpHandler;
            Mapper = mapper;
            JsonOptions = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            WebHelper = webHelper;
            SessionManager = sessionManager;
            ErrorFactory = errorFactory;
            ErrorService = errorService;
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
