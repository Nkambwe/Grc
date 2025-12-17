using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Utils;
using System.Text.Json;

namespace Grc.ui.App.Services
{
    public class RegulatonCategoryService : GrcBaseService, IRegulatonCategoryService
    {
        public RegulatonCategoryService(IApplicationLoggerFactory loggerFactory, 
            IHttpHandler httpHandler, 
            IEnvironmentProvider environment, 
            IEndpointTypeProvider endpointType, 
            IMapper mapper, 
            IWebHelper webHelper, 
            SessionManager sessionManager, 
            IGrcErrorFactory errorFactory, 
            IErrorService errorService) 
            : base(loggerFactory, httpHandler, environment,
                  endpointType, mapper, webHelper, sessionManager, 
                  errorFactory, errorService) {
        }

        public async Task<GrcResponse<GrcRegulatoryCategoryResponse>> GetCategoryAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/regulatory-categories-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcRegulatoryCategoryResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "REGULATORY-CATEGORY-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );

                return new GrcResponse<GrcRegulatoryCategoryResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcRegulatoryCategoryResponse>>> GetPagedCategoriesAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcRegulatoryCategoryResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/paged-categories-list";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcRegulatoryCategoryResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "REGULATORY-CATEGORY-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcRegulatoryCategoryResponse>>(error);
            }
        }

        public async Task<GrcResponse<List<GrcRegulatoryCategoryResponse>>> GetRegulatoryCategories(GrcRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<List<GrcRegulatoryCategoryResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/category-list";
                return await HttpHandler.PostAsync<GrcRequest, List<GrcRegulatoryCategoryResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "REGULATORY-CATEGORY-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<List<GrcRegulatoryCategoryResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateCategoryAsync(RegulatoryCategoryRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Regulatory Category record cannot be null",
                        "Invalid Regulatory category record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"CREATE REGULATORY CATEGORY REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/create-category";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<RegulatoryCategoryRequest, ServiceResponse>(endpoint, request);
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
                await ProcessErrorAsync(ex.Message, "REGULATORY-CATEGORY-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateCategoryAsync(RegulatoryCategoryRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Category cannot be null", "Invalid category record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"UPDATE CATEGORY REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/update-category";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<RegulatoryCategoryRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "REGULATORY-CATEGORIES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "REGULATORY-CATEGORIES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteCategoryAsync(GrcIdRequest request)
        {
            try {

                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Regulatory category record cannot be null",
                        "Invalid regulatory category document record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE REGULATORY CATEGORIES REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/delete-category";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "REGULATORY-CATEGORIES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "REGULATORY-CATEGORIES-SERVICE", ex.StackTrace);
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
