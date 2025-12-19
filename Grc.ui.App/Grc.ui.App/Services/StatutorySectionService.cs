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

    public class StatutorySectionService : GrcBaseService, IStatutorySectionService {

        public StatutorySectionService(IApplicationLoggerFactory loggerFactory, 
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

        public async Task<GrcResponse<GrcStatutorySectionResponse>> GetSectionAsyncAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/statute-section-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcStatutorySectionResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "STATUTE-ACT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );

                return new GrcResponse<GrcStatutorySectionResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcStatutorySectionResponse>>> GetLawSectionsAsync(StatueListRequest request, long userId, string ipAddress) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcStatutorySectionResponse>>(error);
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
                    Action = Activity.SECTION_RETRIEVE_LAWSECTIONLIST.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/statute-sections-list";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcStatutorySectionResponse>>(endpoint, tableListRequest);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "STATUTE-ACT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcStatutorySectionResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcStatutorySectionResponse>>> GetPagedSectionsAsync(StatueListRequest request, long userId, string ipAddress) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcStatutorySectionResponse>>(error);
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
                    Action = Activity.SECTION_RETRIEVE_SECTIONS_ALL.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/paged-statute-sections-list";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcStatutorySectionResponse>>(endpoint, tableListRequest);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "STATUTE-ACT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcStatutorySectionResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateSectionAsync(StatuteSectionViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Law section/Acticle record cannot be null", "Invalid Law section/Acticle record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                var request = Mapper.Map<GrcStatuteSectionRequest>(model);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.SECTION_CREATE_SECTION.GetDescription();

                //..map request
                Logger.LogActivity($"CREATE STATUTORY SECTION REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/create-statute-section";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcStatuteSectionRequest, ServiceResponse>(endpoint, request);
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
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateSectionAsync(StatuteSectionViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Statute section cannot be null", "Invalid statute section record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                var request = Mapper.Map<GrcStatuteSectionRequest>(model);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.SECTION_UPDATE_SECTION.GetDescription();

                //..map request
                Logger.LogActivity($"UPDATE STATUTE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/update-statute-section";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcStatuteSectionRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "STATUTE-ACT-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "STATUTE-ACT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteSectionAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Statutory section record cannot be null", "Invalid statutory section record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE STATUTE SECTION REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.RegisterBase}/delete-statute-section";
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

    }
}
