using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using System.Text.Json;

namespace Grc.ui.App.Services {

    public class ErrorService : GrcBaseService, IErrorService {

        public ErrorService(IApplicationLoggerFactory loggerFactory,
            IHttpHandler httpHandler, IEnvironmentProvider environment, 
            IEndpointTypeProvider endpointType, IMapper mapper) 
            : base(loggerFactory, httpHandler, environment, endpointType, mapper) {
        }

        public async Task<GrcResponse<ServiceResponse>> SaveSystemErrorAsync(GrcErrorModel model, string ipAddress) {
             //..validate input
            if(model == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Request record cannot be empty",
                    "Error object is null and cannot be saved"
                );
        
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..map request
                Logger.LogActivity($"REQUEST MODEL: {JsonSerializer.Serialize(model)}");
                var request = Mapper.Map<ErrorRequest>(model);
                request.IPAddress = ipAddress;
                
                //..build endpoint
                var endpoint = $"{EndpointProvider.Error.CaptureError}";
                Logger.LogActivity($"Endpoint: {endpoint}");
        
                return await HttpHandler.PostAsync<ErrorRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);
        
            } catch (GRCException ex)  {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");    
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
        
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
