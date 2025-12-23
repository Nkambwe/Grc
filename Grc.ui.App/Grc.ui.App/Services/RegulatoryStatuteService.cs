using AutoMapper;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using System.Net;
using System.Text.Json;

namespace Grc.ui.App.Services {

    public class RegulatoryStatuteService : GrcBaseService, IRegulatoryStatuteService {

        public RegulatoryStatuteService(IApplicationLoggerFactory loggerFactory, 
            IHttpHandler httpHandler, 
            IEnvironmentProvider environment, 
            IEndpointTypeProvider endpointType, 
            IMapper mapper, IWebHelper webHelper, 
            SessionManager sessionManager, 
            IGrcErrorFactory errorFactory, 
            IErrorService errorService) 
            : base(loggerFactory, httpHandler, environment, endpointType, mapper, webHelper, sessionManager, errorFactory, errorService) {
        }

        public async Task<GrcResponse<GrcStatuteSupportResponse>> GetStatuteSupportItemsAsync(GrcRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcStatuteSupportResponse>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/law-support-items";
                return await HttpHandler.PostAsync<GrcRequest, GrcStatuteSupportResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "STATUTE-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcStatuteSupportResponse>(error);
            }
        }

        public async Task<GrcResponse<GrcStatutoryLawResponse>> GetStatuteAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/statute-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcStatutoryLawResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "STATUTE-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );

                return new GrcResponse<GrcStatutoryLawResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcStatutoryLawResponse>>> GetCategoryStatutes(StatueListRequest request, long userId, string ipAddress) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST,"Invalid Request object","Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcStatutoryLawResponse>>(error);
                }

                TableListRequest tableListRequest = new() { 
                    UserId = userId,
                    IPAddress = ipAddress,
                    ActivityTypeId = request.ActivityTypeId,
                    SearchTerm = request.SearchTerm,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    SortBy = request.SortBy,
                    SortDirection = request.SortDirection,
                    IncludeDeleted = false,
                    Action = Activity.STATUS_RETRIEVE_CATEGORYLIST.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/category-statute-list";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcStatutoryLawResponse>>(endpoint, tableListRequest);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "STATUTE-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcStatutoryLawResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcStatutoryLawResponse>>> GetPagedStatutesAsync(StatueListRequest request, long userId, string ipAddress) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object","Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcStatutoryLawResponse>>(error);
                }

                TableListRequest tableListRequest = new() {
                    UserId = userId,
                    IPAddress = ipAddress,
                    ActivityTypeId = request.ActivityTypeId,
                    SearchTerm = request.SearchTerm,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    SortBy = request.SortBy,
                    SortDirection = request.SortDirection,
                    IncludeDeleted = false,
                    Action = Activity.STATUS_RETRIEVE_ALL.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/paged-statute-list";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcStatutoryLawResponse>>(endpoint, tableListRequest);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "STATUTE-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcStatutoryLawResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcObligationResponse>>> GetStatutoryObligations(TableListRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcObligationResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/paged-obligations-list";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcObligationResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "STATUTE-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcObligationResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateStatuteAsync(StatuteViewModel model, long userId, string ipAddress) {
            try {

                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Statutory law record cannot be null", "Invalid Statutory law record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                var request = Mapper.Map<GrcStatutoryLawRequest>(model);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.LAW_CREATE.GetDescription();

                //..map request
                Logger.LogActivity($"CREATE STATUTORY LAW REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/create-statute";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcStatutoryLawRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "STATUTE-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY,"Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "STATUTE-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR,"An unexpected error occurred","Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateStatuteAsync(StatuteViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Statute cannot be null", "Invalid statute record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                var request = Mapper.Map<GrcStatutoryLawRequest>(model);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.LAW_UPDATE.GetDescription();

                //..map request
                Logger.LogActivity($"UPDATE STATUTE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/update-statute";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcStatutoryLawRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "STATUTE-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY,"Network error occurred",httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "STATUTE-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR,"An unexpected error occurred","Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteStatuteAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Statutory record cannot be null", "Invalid statutory document record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }
                //..map request
                Logger.LogActivity($"DELETE STATUTE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/delete-statute";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "STATUTE-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY,"Network error occurred",httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "STATUTE-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

    }
}
