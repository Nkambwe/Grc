using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using System.Text.Json;

namespace Grc.ui.App.Services {

    public class InstallService : GrcBaseService, IInstallService {

        public InstallService(IApplicationLoggerFactory loggerFactory, 
                              IHttpHandler httpHandler,
                              IHttpClientFactory httpClientFactory,
                              IEnvironmentProvider environment, 
                              IEndpointTypeProvider endpointType,
                              IMapper mapper,
                              IWebHelper webHelper,
                              SessionManager sessionManager,
                              IGrcErrorFactory errorFactory,
                              IErrorService errorService) 
        : base(loggerFactory, httpHandler, environment, endpointType, mapper,webHelper,sessionManager,errorFactory,errorService) {
            Logger.Channel = $"COMPANY-{DateTime.Now:yyyyMMddHHmmss}";
        }

        public async Task<GrcResponse<ServiceResponse>> RegisterCompanyAsync(CompanyRegistrationModel model, string ipAddress) {
            //..validate input
            if(model == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Request record cannot be empty",
                    "The company registration model cannot be null"
                );
        
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..map request
                Logger.LogActivity($"REQUEST MODEL: {JsonSerializer.Serialize(model)}");
                var request = Mapper.Map<CompanyRegiatrationRequest>(model);
                request.IPAddress = ipAddress;
                
                //..build endpoint
                var endpoint = $"{EndpointProvider.Registration.Register}";
                Logger.LogActivity($"Endpoint: {endpoint}");
        
                return await HttpHandler.PostAsync<CompanyRegiatrationRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message,"INSTALL-SERVICE" , httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);
        
            } catch (GRCException ex)  {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");    
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message,"INSTALL-SERVICE" , ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }
    }

}
