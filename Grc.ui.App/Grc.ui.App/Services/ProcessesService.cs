using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
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
using Activity = Grc.ui.App.Enums.Activity;

namespace Grc.ui.App.Services {

    public class ProcessesService : GrcBaseService, IProcessesService {

        public ProcessesService(IApplicationLoggerFactory loggerFactory, 
            IHttpHandler httpHandler, IEnvironmentProvider environment, 
            IEndpointTypeProvider endpointType, IMapper mapper, 
            IWebHelper webHelper, SessionManager sessionManager, 
            IGrcErrorFactory errorFactory, IErrorService errorService) 
            : base(loggerFactory, httpHandler, environment, endpointType, 
                  mapper, webHelper, sessionManager, errorFactory, errorService) {
        }

        #region Statistics

        public async Task<GrcResponse<OperationsUnitCountResponse>> UnitStatisticAsync(long userId, string ipAddress) {
            try {

                var request = new GrcStatisticRequest() {
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = Activity.PROCESSES_UNIT_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/unit-statistics";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcStatisticRequest, OperationsUnitCountResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<OperationsUnitCountResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<OperationsUnitCountResponse>(error);
            }
           
        }

        public async Task<GrcResponse<UnitExtensionCountResponse>> UnitExtensionsCountAsync(string unit, long userId, string ipAddress) {
            try {

                var request = new GrcUnitStatisticRequest() {
                    UserId = userId,
                    UnitName = (unit ?? string.Empty).Trim(),
                    IPAddress = ipAddress,
                    Action = Activity.PROCESSES_UNIT_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"UNIT STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/unit-extensions-statistics";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcUnitStatisticRequest, UnitExtensionCountResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<UnitExtensionCountResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<UnitExtensionCountResponse>(error);
            }

        }

        public async Task<GrcResponse<CategoriesCountResponse>> CategoryCountAsync(long userId, string ipAddress, string unit) {
            try {

                var request = new GrcUnitStatisticRequest() {
                    UserId = userId,
                    UnitName = (unit ?? string.Empty).Trim(),
                    IPAddress = ipAddress,
                    Action = Activity.PROCESSES_UNIT_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"UNIT STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/category-statistics";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcUnitStatisticRequest, CategoriesCountResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<CategoriesCountResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<CategoriesCountResponse>(error);
            }
        }

        public async Task<GrcResponse<CategoryExtensionResponse>> CategoryExtensionsCountAsync(string category, long userId, string ipAddress)
        {
            try {

                var request = new GrcCategoryStatisticRequest() {
                    UserId = userId,
                    Category = (category ?? string.Empty).Trim(),
                    IPAddress = ipAddress,
                    Action = Activity.PROCESSES_CATEGORY_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CATEGORY STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/category-extensions-statistics";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcCategoryStatisticRequest, CategoryExtensionResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<CategoryExtensionResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<CategoryExtensionResponse>(error);
            }
        }

        public async Task<GrcResponse<List<StatisticTotalResponse>>> TotalExtensionsCountAsync(long userId, string ipAddress) {
            try {

                var request = new GrcRequest() {
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = Activity.PROCESSES_CATEGORY_STATISTIC.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                //..map request
                Logger.LogActivity($"CATEGORY TOTALS REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/total-process-statistics";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcRequest, List<StatisticTotalResponse>>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<List<StatisticTotalResponse>>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<List<StatisticTotalResponse>>(error);
            }
        }

        #endregion

        #region Process Registers

        public async Task<GrcResponse<List<GrcProcessRegisterResponse>>> GetAllProcessesAsync(GrcRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<List<GrcProcessRegisterResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/process-list";
                return await HttpHandler.PostAsync<GrcRequest, List<GrcProcessRegisterResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<List<GrcProcessRegisterResponse>>(error);
            }
        }

        public async Task<GrcResponse<GrcProcessSupportResponse>> GetProcessSupportItemsAsync(GrcRequest request) {
            try
            {
                if (request == null)
                {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcProcessSupportResponse>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/support-items";
                return await HttpHandler.PostAsync<GrcRequest, GrcProcessSupportResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<GrcProcessSupportResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcProcessRegisterResponse>>> GetProcessRegistersAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcProcessRegisterResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/registers-all";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcProcessRegisterResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcProcessRegisterResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcProcessRegisterResponse>>> GetNewProcessAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcProcessRegisterResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/processes-new";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcProcessRegisterResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcProcessRegisterResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcProcessRegisterResponse>>> GetReviewProcessAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcProcessRegisterResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/processes-reviews";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcProcessRegisterResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcProcessRegisterResponse>>(error);
            }
        }

        public async Task<GrcResponse<GrcProcessRegisterResponse>> GetProcessRegisterAsync(long recordId, long userId, string ipAddress) {

            try {
                if (recordId == 0) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process ID is required",
                        "Invalid Process request"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcProcessRegisterResponse>(error);
                }

                var request = new GrcIdRequest() {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.PROCESSES_RETRIEVE_PROCESS.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/register";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcProcessRegisterResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );

                return new GrcResponse<GrcProcessRegisterResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateProcessAsync(ProcessViewModel processModel, long userId, string ipAddress) {
            
            try
            {
                if (processModel == null)
                {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process record cannot be null",
                        "Invalid process record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = Mapper.Map<GrcProcessRegisterRequest>(processModel);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.PROCESSES_CREATE_PROCESS.GetDescription();

                //..map request
                Logger.LogActivity($"CREATE Process REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/create-process";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcProcessRegisterRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateProcessAsync(ProcessViewModel processModel, long userId, string ipAddress) {
            
            try
            {
                if (processModel == null)
                {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process record cannot be null",
                        "Invalid process record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }


                //..build request object
                var request = Mapper.Map<GrcProcessRegisterRequest>(processModel);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.PROCESSES_EDITED_PROCESS.GetDescription();

                //..map request
                Logger.LogActivity($"UPDATE PROCESS REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/update-process";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcProcessRegisterRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
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

        public async Task<GrcResponse<ServiceResponse>> DeleteProcessAsync(GrcIdRequest request) {
           
            try {

                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process record cannot be null",
                        "Invalid process record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE PROCESS REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/delete-process";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
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

        #endregion

        #region Process Groups

        public async Task<GrcResponse<GrcProcessGroupResponse>> GetProcessGroupAsync(long recordId, long userId, string ipAddress) {

            try {
                if (recordId == 0) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process Group ID is required",
                        "Invalid Process request"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcProcessGroupResponse>(error);
                }

                var request = new GrcIdRequest()
                {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.PROCESS_GROUP_RETRIVED.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/group";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcProcessGroupResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );

                return new GrcResponse<GrcProcessGroupResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcProcessGroupResponse>>> GetProcessGroupsAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcProcessGroupResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/groups-all";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcProcessGroupResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcProcessGroupResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateProcessGroupAsync(ProcessGroupViewModel groupModel, long userId, string ipAddress) {

            try {

                if (groupModel == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Process record cannot be null", "Invalid role record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcProcessGroupRequest {
                    Id = groupModel.Id,
                    GroupName = groupModel.GroupName,
                    GroupDescription = groupModel.GroupDescription,
                    CreatedBy = "SYSTEM",
                    CreatedOn = DateTime.UtcNow,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.PROCESS_GROUP_CREATE.GetDescription(),
                    Processes = groupModel.Processes,
                };

                //..map request
                Logger.LogActivity($"CREATE PROCESS GROUP REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/create-group";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcProcessGroupRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }

        }

        public async Task<GrcResponse<ServiceResponse>> UpdateProcessGroupAsync(ProcessGroupViewModel groupModel, long userId, string ipAddress) {
            
            try {
                if (groupModel == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process Group record cannot be null",
                        "Invalid process group record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }


                //..build request object
                var request = Mapper.Map<GrcProcessGroupRequest>(groupModel);
                request.UserId = userId;
                request.IpAddress = ipAddress;
                request.Action = Activity.PROCESS_GROUP_UPDATE.GetDescription();

                //..map request
                Logger.LogActivity($"UPDATE PROCESS GROUP REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/update-group";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcProcessGroupRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
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

        public async Task<GrcResponse<ServiceResponse>> DeleteProcessGroupAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process Group cannot be null",
                        "Invalid process group record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE PROCESS GROUP REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/delete-group";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
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

        #endregion

        #region Process Tags

        public async Task<GrcResponse<GrcProcessTagResponse>> GetProcessTagAsync(long recordId, long userId, string ipAddress) {

            try
            {
                if (recordId == 0)
                {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process Group ID is required",
                        "Invalid Process request"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcProcessTagResponse>(error);
                }

                var request = new GrcIdRequest() {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.PROCESS_TAG_RETRIVED.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/tag";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcProcessTagResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );

                return new GrcResponse<GrcProcessTagResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcProcessTagResponse>>> GetProcessTagsAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcProcessTagResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/tags-all";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcProcessTagResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcProcessTagResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateProcessTagAsync(ProcessTagViewModel tagModel, long userId, string ipAddress) {
            try {

                if (tagModel == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Process record cannot be null", "Invalid role record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcProcessTagRequest {
                    Id = tagModel.Id,
                    TagName = tagModel.TagName,
                    TagDescription = tagModel.TagDescription,
                    CreatedBy = "SYSTEM",
                    CreatedOn = DateTime.UtcNow,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.PROCESS_TAG_CREATE.GetDescription(),
                    Processes = tagModel.Processes
                };

                //..map request
                Logger.LogActivity($"CREATE PROCESS TAG REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/create-tag";
                Logger.LogActivity($"Endpoint: {endpoint}");
                return await HttpHandler.PostAsync<GrcProcessTagRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateProcessTagAsync(ProcessTagViewModel tagModel, long userId, string ipAddress)
        {
            try
            {
                if (tagModel == null)
                {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process Tag record cannot be null",
                        "Invalid process tag record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }


                //..build request object
                var request = new GrcProcessTagRequest {
                    Id = tagModel.Id,
                    TagName = tagModel.TagName,
                    TagDescription = tagModel.TagDescription,
                    CreatedBy = "SYSTEM",
                    CreatedOn = DateTime.UtcNow,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.PROCESS_TAG_UPDATE.GetDescription(),
                    Processes = tagModel.Processes
                };

                //..map request
                Logger.LogActivity($"UPDATE PROCESS TAG REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/update-tag";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcProcessTagRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
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

        public async Task<GrcResponse<ServiceResponse>> DeleteProcessTagAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process Tag cannot be null",
                        "Invalid process tag record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE PROCESS TAG REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/delete-tag";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
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

        #endregion

        #region Process TAT

        public async Task<GrcResponse<GrcProcessTatResponse>> GetProcessTatAsync(long recordId, long userId, string ipAddress) {

            try {
                if (recordId == 0) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Process Tag ID is required",
                        "Invalid Process request"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcProcessTatResponse>(error);
                }

                var request = new GrcIdRequest()
                {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.PROCESS_TAG_RETRIVED.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/tat";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcProcessTatResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );

                return new GrcResponse<GrcProcessTatResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcProcessTatResponse>>> GetProcessTatAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcProcessTatResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/tat-all";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcProcessTatResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcProcessTatResponse>>(error);
            }
        }

        public async Task<GrcResponse<List<GrcProcessTatResponse>>> GetTATReportAsync(GrcRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<List<GrcProcessTatResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/tat-report";
                return await HttpHandler.PostAsync<GrcRequest, List<GrcProcessTatResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<List<GrcProcessTatResponse>>(error);
            }
        }

        #endregion

        #region Approval Process

        public async Task<GrcResponse<GrcProcessApprovalStatusResponse>> GetApprovalRecordAsync(long recordId, long userId, string ipAddress) {
            try {
                if (recordId == 0) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Approval ID is required",
                        "Invalid Approval request"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcProcessApprovalStatusResponse>(error);
                }

                var request = new GrcIdRequest() {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.PROCESS_APPROVAL_RETRIVED.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/approval-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcProcessApprovalStatusResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );

                return new GrcResponse<GrcProcessApprovalStatusResponse>(error);
            }
        }

        public async Task<GrcResponse<GrcProcessApprovalStatusResponse>> GetNewApprovalRecordAsync(long recordId, long userId, string ipAddress) {
            try {
                if (recordId == 0) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Approval ID is required",
                        "Invalid Approval request"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcProcessApprovalStatusResponse>(error);
                }

                var request = new GrcIdRequest() {
                    UserId = userId,
                    RecordId = recordId,
                    IPAddress = ipAddress,
                    Action = Activity.PROCESS_APPROVAL_RETRIVED.GetDescription(),
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/new-approval-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcProcessApprovalStatusResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );

                return new GrcResponse<GrcProcessApprovalStatusResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcProcessApprovalStatusResponse>>> GetProcessApprovalStatusAsync(TableListRequest request)
        {
            try
            {
                if (request == null)
                {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Invalid Request object",
                        "Request object cannot be null"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcProcessApprovalStatusResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/approval-status";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcProcessApprovalStatusResponse>>(endpoint, request);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "An unexpected error occurred",
                    "Cannot proceed! An error occurred, please try again later"
                );
                return new GrcResponse<PagedResponse<GrcProcessApprovalStatusResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateApprovalAsync(GrcProcessApprovalView approvalModel, long userId, string ipAddress) {
            try {

                if (approvalModel == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Approval record cannot be null",
                        "Invalid approval record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcProcessApprovalRequest {
                    Id = approvalModel.Id,
                    ProcessId = approvalModel.ProcessId,
                    ProcessName = approvalModel.ProcessName,
                    HodStatus = approvalModel.HodStatus,
                    HodComment = approvalModel.HodComment,
                    RiskStatus = approvalModel.RiskStatus,
                    RiskComment = approvalModel.RiskComment,
                    ComplianceStatus = approvalModel.ComplianceStatus,
                    ComplianceComment = approvalModel.ComplianceComment,
                    BopRequired = approvalModel.BopRequired,
                    BopStatus = approvalModel.BopStatus,
                    BopComment = approvalModel.BopComment,
                    CreditRequired = approvalModel.CreditRequired,
                    CreditStatus = approvalModel.CreditStatus,
                    CreditComment = approvalModel.CreditComment,
                    TreasuryRequired = approvalModel.TreasuryRequired,
                    TreasuryStatus = approvalModel.TreasuryStatus,
                    TreasuryComment = approvalModel.TreasuryComment,
                    FintechRequired = approvalModel.FintechRequired,
                    FintechStatus = approvalModel.FintechStatus,
                    FintechComment = approvalModel.FintechComment,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.PROCESS_APPROVAL_UPDATE.GetDescription(),
                };

                //..map request
                Logger.LogActivity($"UPDATE PROCESS PROCESS REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/update-approval";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcProcessApprovalRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
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

        #endregion

        #region Process Review

        public async Task<GrcResponse<ServiceResponse>> InitiateProcessReviewAsync(ProcessReviewModel reviewModel, long userId, string ipAddress) {
            try {

                if (reviewModel == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Review record cannot be null",
                        "Invalid review record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new ProcessReviewRequest {
                    Id = reviewModel.Id,
                    ProcessName = reviewModel.ProcessName,
                    ProcessStatus = reviewModel.ProcessStatus,
                    UnlockReason = reviewModel.UnlockReason,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.PROCESS_REVIEW.GetDescription(),
                };

                //..map request
                Logger.LogActivity($"REQUEST PROCESS REVIEW REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/initiate-review";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<ProcessReviewRequest, ServiceResponse>(endpoint, request);
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(
                    GrcStatusCodes.BADGATEWAY,
                    "Network error occurred",
                    httpEx.Message
                );
                return new GrcResponse<ServiceResponse>(error);

            }
            catch (GRCException ex)
            {
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

        public async Task<GrcResponse<ServiceResponse>> HoldProcessReviewAsync(ProcessHoldModel holdModel, long userId, string ipAddress) {
            try {

                if (holdModel == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Hold Process record cannot be null",
                        "Invalid Hold record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new ProcessHoldRequest {
                    Id = holdModel.Id,
                    ProcessId = holdModel.ProcessId,
                    ProcessName = holdModel.ProcessName,
                    ProcessStatus = holdModel.ProcessStatus,
                    HoldReason = holdModel.HoldReason,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.PROCESS_HOLD.GetDescription(),
                };

                //..map request
                Logger.LogActivity($"HOLD PROCESS REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/hold-review";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<ProcessHoldRequest, ServiceResponse>(endpoint, request);
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

        public async Task<GrcResponse<ServiceResponse>> RequestProcessApprovalAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.BADREQUEST,
                        "Request record cannot be null",
                        "Invalid approval request record"
                    );

                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"PROCESS APPROVAL REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Operations.ProcessBase}/approval-request";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
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

        #endregion
    }

}