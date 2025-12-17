using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using System.Text.Json;

namespace Grc.ui.App.Services {
    public class RegulatonTypeService : GrcBaseService, IRegulatonTypeService
    {
        public RegulatonTypeService(IApplicationLoggerFactory loggerFactory, 
            IHttpHandler httpHandler, 
            IEnvironmentProvider environment, 
            IEndpointTypeProvider endpointType, 
            IMapper mapper, 
            IWebHelper webHelper, 
            SessionManager sessionManager, 
            IGrcErrorFactory errorFactory, 
            IErrorService errorService) 
            : base(loggerFactory, httpHandler, environment, endpointType, 
                  mapper, webHelper, sessionManager, errorFactory, errorService) {
        }

        public async Task<GrcResponse<GrcRegulatoryTypeResponse>> GetTypeAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/regulatory-types-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcRegulatoryTypeResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "REGULATORY-TYPES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );

                return new GrcResponse<GrcRegulatoryTypeResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcRegulatoryTypeResponse>>> GetPagedTypesAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcRegulatoryTypeResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/paged-regulatory-type-list";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcRegulatoryTypeResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "REGULATORY-TYPES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcRegulatoryTypeResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateTypeAsync(RegulatoryViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Regulatory type record cannot be null",
                        "Invalid Regulatory type record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                var request = new RegulatoryTypeRequest {
                    TypeName = model.TypeName,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = Activity.CREATEREGULATORYTYPE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CREATE REGULATORY TYPE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/create-regulatory-type";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<RegulatoryTypeRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "REGULATORY-TYPES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "REGULATORY-TYPES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateTypeAsync(RegulatoryViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Regulatory type cannot be null", "Invalid regulatory type record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }


                //..build request object
                var request = Mapper.Map<GrcPolicyDocumentRequest>(model);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_EDITED_TYPE.GetDescription();

                //..map request
                Logger.LogActivity($"UPDATE REGULATORY TYPE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/update-regulatory-type";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcPolicyDocumentRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "REGULATORY-TYPES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "REGULATORY-TYPES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteTypeAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Regulatory type record cannot be null",
                        "Invalid regulatory type document record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE REGULATORY TYPE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/delete-regulatory-type";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "REGULATORY-TYPES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "REGULATORY-TYPES-SERVICE", ex.StackTrace);
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
