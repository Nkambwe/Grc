using AutoMapper;
using DocumentFormat.OpenXml.EMMA;
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

    public class ComplianceControlService : GrcBaseService, IComplianceControlService {
        public ComplianceControlService(IApplicationLoggerFactory loggerFactory,
            IHttpHandler httpHandler, IEnvironmentProvider environment,
            IEndpointTypeProvider endpointType,
            IMapper mapper, IWebHelper webHelper,
            SessionManager sessionManager,
            IGrcErrorFactory errorFactory,
            IErrorService errorService) :
            base(loggerFactory, httpHandler, environment, endpointType, mapper,
                webHelper, sessionManager, errorFactory, errorService) {
        }

        public async Task<GrcResponse<GrcControlCategoryResponse>> GetCategoryAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/control-category-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcControlCategoryResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "COMPLIANCE-CONTROLS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcControlCategoryResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcControlCategoryResponse>>> GetControlCategoriesAsync(TableListRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcControlCategoryResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/paged-control-categories";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcControlCategoryResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "COMPLIANCE-CONTROLS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcControlCategoryResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateControlCategoryAsync(ControlCategoryViewModel model, long userId, string ipAddress) {
            try {

                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Control Category record cannot be null", "Invalid Control Category record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                var request = new GrcControlCategoryRequest {
                    CategoryName = model.CategoryName,
                    Comments = model.Comments,
                    IsDeleted = model.IsDeleted,
                    Exclude = model.IsExcluded,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.COMPLIANCE_CATEGORY_CONTROL.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CREATE CATEGORY CONTROL REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/create-control-category";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcControlCategoryRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "COMPLIANCE-CONTROLS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "STATUTE-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateCategoryAsync(ControlCategoryViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Category control cannot be null", "Invalid category record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                var request = new GrcControlCategoryRequest {
                    Id = model.Id,
                    CategoryName = model.CategoryName,
                    Comments = model.Comments,
                    IsDeleted = model.IsDeleted,
                    Exclude = model.IsExcluded,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.COMPLIANCE_CONTROL_UPDATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"UPDATE CATEGORY REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/update-control-category";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcControlCategoryRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "COMPLIANCE-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY,"Network error occurred",httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "COMPLIANCE-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR,"An unexpected error occurred","Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<GrcControlItemResponse>> GetItemAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/control-item-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcControlItemResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "COMPLIANCE-CONTROLS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcControlItemResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteItemAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Control Item record cannot be null", "Invalid Request record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE CONTROL ITEM REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/delete-control-item";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "STATUTE-ACT-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "STATUTE-ACT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateItemAsync(ItemViewModel model, long userId, string ipAddress) {
            try {

                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Control Category record cannot be null", "Invalid Control Category record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                var request = new GrcControlItemRequest {
                    CategoryId = model.CategoryId,
                    ItemName = model.ItemName,
                    Comments = model.Comments,
                    IsDeleted = model.IsDeleted,
                    Exclude = model.IsExcluded,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.COMPLIANCE_ITEM_CREATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CREATE CONTROL ITEM REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/create-control-item";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcControlItemRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "COMPLIANCE-CONTROLS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "STATUTE-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateItemAsync(ItemViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Category item cannot be null", "Invalid category item record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                var request = new GrcControlItemRequest {
                    Id = model.Id,
                    CategoryId = model.CategoryId,
                    ItemName = model.ItemName,
                    Comments = model.Comments,
                    IsDeleted = model.IsDeleted,
                    Exclude = model.IsExcluded,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.COMPLIANCE_ITEM_UPDATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"UPDATE ITEM REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/update-control-item";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcControlItemRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "COMPLIANCE-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY,"Network error occurred",httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "COMPLIANCE-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR,"An unexpected error occurred","Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }
    }
}
