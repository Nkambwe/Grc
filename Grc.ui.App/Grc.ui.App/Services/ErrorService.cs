using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using System.Text.Json;

namespace Grc.ui.App.Services {

    public class ErrorService : IErrorService {
        private readonly IApplicationLogger _logger;
        private readonly IMapper _mapper;
        private readonly IHttpHandler _httpHandler;
        private readonly IEndpointTypeProvider _endpointProvider;

        public ErrorService(IApplicationLoggerFactory loggerFactory,
            IHttpHandler httpHandler, IEndpointTypeProvider endpointProvider, IMapper mapper) {
            _mapper = mapper;
            _httpHandler = httpHandler;
            _endpointProvider = endpointProvider;
            _logger = loggerFactory.CreateLogger();
        }

        public async Task<GrcResponse<ServiceResponse>> SaveSystemErrorAsync(GrcErrorModel model, string ipAddress) {
             //..validate input
            if(model == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Request record cannot be empty",
                    "Error object is null and cannot be saved"
                );
        
                _logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<ServiceResponse>(error);
            }

            try {
                //..map request
                _logger.LogActivity($"REQUEST MODEL: {JsonSerializer.Serialize(model)}");
                var request = _mapper.Map<ErrorRequest>(model);
                request.IPAddress = ipAddress;
                //..build endpoint
                var endpoint = $"{_endpointProvider.Error.CaptureError}";
                _logger.LogActivity($"Endpoint: {endpoint}");
        
                return await _httpHandler.PostAsync<ErrorRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                _logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                _logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
        
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);
        
            } catch (GRCException ex)  {
                _logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");    
                _logger.LogActivity(ex.StackTrace, "STACKTRACE");
        
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
