using AutoMapper;
using Grc.ui.App.Dtos;
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

    public class PolicyService : GrcBaseService, IPolicyService {
        
        public PolicyService(IApplicationLoggerFactory loggerFactory, 
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

        public async Task<GrcResponse<PolicyDocumentResponse>> GetPolicyDocumentAsync(GrcIdRequest request) {

            try {
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/document-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, PolicyDocumentResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "POLICY-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );

                return new GrcResponse<PolicyDocumentResponse>(error);
            }
        }

        public async Task<GrcResponse<List<PolicyDocumentResponse>>> GetDocumentListAsync(GrcRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<List<PolicyDocumentResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/document-list";
                return await HttpHandler.PostAsync<GrcRequest, List<PolicyDocumentResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "POLICY-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<List<PolicyDocumentResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<PolicyDocumentResponse>>> GetPagedDocumentsAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<PolicyDocumentResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/paged-document-list";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<PolicyDocumentResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<PolicyDocumentResponse>>(error);
            }
        }

        public async Task<GrcResponse<GrcPolicySupportResponse>> GetPolicySupportItemsAsync(GrcRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcPolicySupportResponse>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/support-items";
                return await HttpHandler.PostAsync<GrcRequest, GrcPolicySupportResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "POLICY-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<GrcPolicySupportResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateDocumentAsync(PolicyViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Policy document record cannot be null",
                        "Invalid Policy document  record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = Mapper.Map<GrcPolicyDocumentRequest>(model);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.PROCESSES_CREATE_POLICY.GetDescription();

                //..map request
                Logger.LogActivity($"CREATE POLICY DOCUMENT REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/create-document";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcPolicyDocumentRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "POLICY-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "POLICY-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateDocumentAsync(PolicyViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process record cannot be null",
                        "Invalid process record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }


                //..build request object
                var request = Mapper.Map<GrcPolicyDocumentRequest>(model);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.PROCESSES_EDITED_POLICY.GetDescription();

                //..map request
                Logger.LogActivity($"UPDATE POLICY DOCUMENT REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/update-document";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcPolicyDocumentRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeletePolicyAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Policy document record cannot be null",
                        "Invalid Policy document record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE POLICY DOCUMENT REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/delete-document";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "POLICY-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "POLICY-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> LockPolicyAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Policy document record cannot be null",
                        "Invalid Policy document record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"LOCK POLICY DOCUMENT REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/lock-document";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "POLICY-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "POLICY-SERVICE", ex.StackTrace);
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
