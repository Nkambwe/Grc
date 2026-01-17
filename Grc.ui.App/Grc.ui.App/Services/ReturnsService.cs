using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
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

    public class ReturnsService : GrcBaseService, IReturnsService {

        public ReturnsService(IApplicationLoggerFactory loggerFactory, 
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

        #region Policy Statistics

        public async Task<GrcResponse<GrcPolicyDashboardResponse>> GetPolicyCountAsync(long userId, string ipAddress, string status) {
            try {

                var request = new GrcStatusStatisticRequest() {
                    UserId = userId,
                    Status = (status ?? string.Empty).Trim(),
                    IPAddress = ipAddress,
                    Action = Activity.POLICY_STATUS_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"POLICY STATUS STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/policy-statistics";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcStatusStatisticRequest, GrcPolicyDashboardResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<GrcPolicyDashboardResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcPolicyDashboardResponse>(error);
            }
        }

        public async Task<GrcResponse<GeneralComplianceReturnResponse>> GetAllReturnsStatisticAsync(long userId, string ipAddress) {
            try {

                var request = new GrcRequest() {
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = Activity.RETURNS_DASHBOARD_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"ALL RETURNS STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/dashboard-statistics";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcRequest, GeneralComplianceReturnResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<GeneralComplianceReturnResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GeneralComplianceReturnResponse>(error);
            }
        }

        #endregion

        #region Circular Statistics

        public async Task<GrcResponse<CircularDashboardResponses>> GetCircularStatisticAsync(long userId, string ipAddress) {
            try {

                var request = new GrcStatusStatisticRequest() {
                    UserId = userId,
                    Status = string.Empty,
                    IPAddress = ipAddress,
                    Action = Activity.CIRCULAR_ALL_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CIRCULAR ALL STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/all-statistics";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcStatusStatisticRequest, CircularDashboardResponses>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<CircularDashboardResponses>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<CircularDashboardResponses>(error);
            }
        }

        public async Task<GrcResponse<CircularExtensionDashboardResponses>> GetCircularExtensionStatisticAsync(long userId, string ipAddress, string authority) {
            try {

                var request = new GrcAuthorityStatisticRequest() {
                    UserId = userId,
                    Authority = (authority ?? string.Empty).Trim(),
                    IPAddress = ipAddress,
                    Action = Activity.CIRCULAR_AUTHORITY_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CIRCULAR STATUS STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/status-statistics";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcAuthorityStatisticRequest, CircularExtensionDashboardResponses>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<CircularExtensionDashboardResponses>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<CircularExtensionDashboardResponses>(error);
            }
        }

        public async Task<GrcResponse<CircularMiniDashboardResponses>> GetAuthorityCircularCountAsync(long userId, string ipAddress, string authority) {
            try {

                var request = new GrcAuthorityStatisticRequest() {
                    UserId = userId,
                    Authority = (authority ?? string.Empty).Trim(),
                    IPAddress = ipAddress,
                    Action = Activity.CIRCULAR_AUTHORITY_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CIRCULAR AUTHORITY STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/circular-authorities";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcAuthorityStatisticRequest, CircularMiniDashboardResponses>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "PROCESSES-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<CircularMiniDashboardResponses>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "PROCESSES-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<CircularMiniDashboardResponses>(error);
            }
        }


        #endregion

        #region Return Statistics
        
        public async Task<GrcResponse<ReturnsDashboardResponses>> GetReturnStatisticAsync(long userId, string ipAddress) {

            try {

                var request = new GrcStatusStatisticRequest() {
                    UserId = userId,
                    Status = string.Empty,
                    IPAddress = ipAddress,
                    Action = Activity.RETURNS_ALL_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"RETURNS ALL STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/all-statistics";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcStatusStatisticRequest, ReturnsDashboardResponses>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ReturnsDashboardResponses>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ReturnsDashboardResponses>(error);
            }
        }

        public async Task<GrcResponse<ComplianceExtensionReturnResponse>> GetReturnExtensionStatisticAsync(long userId, string ipAddress, string period) {
            try {

                var request = new GrcPeriodStatisticRequest() {
                    UserId = userId,
                    Period = (period ?? string.Empty).Trim(),
                    IPAddress = ipAddress,
                    Action = Activity.RETURNS_PERIOD_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"RETURNS PERIOD STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/returns-extension";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcPeriodStatisticRequest, ComplianceExtensionReturnResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ComplianceExtensionReturnResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ComplianceExtensionReturnResponse>(error);
            }
        }

        public async Task<GrcResponse<GrcReturnDashboardResponse>> GrcReturnDashboardResponseAsync(long userId, string ipAddress, string period) {
            try {

                var request = new GrcPeriodStatisticRequest() {
                    UserId = userId,
                    Period = (period ?? string.Empty).Trim(),
                    IPAddress = ipAddress,
                    Action = Activity.RETURNS_PERIOD_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"RETURNS PERIOD STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/period-returns";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcPeriodStatisticRequest, GrcReturnDashboardResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<GrcReturnDashboardResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcReturnDashboardResponse>(error);
            }
        }

        #endregion

        #region Tasks Statistics

        public async Task<GrcResponse<ComplianceTaskStatisticResponse>> GetTaskStatisticAsync(long userId, string ipAddress) {
            try {

                var request = new GrcStatusStatisticRequest() {
                    UserId = userId,
                    Status = string.Empty,
                    IPAddress = ipAddress,
                    Action = Activity.TASKS_ALL_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"RETURN ALL STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/tasks-statistics";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcStatusStatisticRequest, ComplianceTaskStatisticResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ComplianceTaskStatisticResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ComplianceTaskStatisticResponse>(error);
            }
        }

        public async Task<GrcResponse<ComplianceTaskMinStatisticResponse>> GetMiniTaskStatisticAsync(long userId, string ipAddress, string status) {
            try {

                var request = new GrcStatusStatisticRequest() {
                    UserId = userId,
                    Status = (status ?? string.Empty).Trim(),
                    IPAddress = ipAddress,
                    Action = Activity.TASK_STATUS_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"RETURN TASKS STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/authorities-statistics";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcStatusStatisticRequest, ComplianceTaskMinStatisticResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ComplianceTaskMinStatisticResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ComplianceTaskMinStatisticResponse>(error);
            }
        }

        #endregion

        #region Return

        public async Task<GrcResponse<GrcStatutoryReturnReportResponse>> GetReturnAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/return-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcStatutoryReturnReportResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcStatutoryReturnReportResponse>(error);
            }
        }
        
        public async Task<GrcResponse<ServiceResponse>> CreateReturnAsync(StatutoryReturnViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Return/Report record cannot be null", "Invalid Return/Report record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcStatutoryReturnReportRequest() {
                    ReturnName = model.ReturnName,
                    TypeId = model.ReturnTypeId,
                    DepartmentId = model.DepartmentId,
                    AuthorityId = model.AuthorityId,
                    StatuteId = model.SectionId,
                    FrequencyId = model.FrequencyId,
                    IsDeleted = model.IsDeleted,
                    Risk = model.RiskAttached,
                    Comments = model.Comments,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = Activity.COMPLIANCE_ITEM_UPDATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CREATE RETURN REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/create-return";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcStatutoryReturnReportRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }
        
        public async Task<GrcResponse<ServiceResponse>> UpdateReturnAsync(StatutoryReturnViewModel model, long userId, string ipAddress) {
            try {

                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Returns record cannot be null", "Invalid process record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcStatutoryReturnReportRequest() {
                    Id = model.Id,
                    ReturnName = model.ReturnName,
                    TypeId = model.ReturnTypeId,
                    StatuteId = model.SectionId,
                    DepartmentId = model.DepartmentId,
                    AuthorityId = model.AuthorityId,
                    FrequencyId = model.FrequencyId,
                    
                    IsDeleted = model.IsDeleted,
                    Risk = model.RiskAttached,
                    Comments = model.Comments,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = Activity.COMPLIANCE_ITEM_UPDATE.GetDescription()
                };
                
                //..map request
                Logger.LogActivity($"UPDATE RETURN/REPORT REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/update-return";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcStatutoryReturnReportRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError( GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }
        
        public async Task<GrcResponse<PagedResponse<GrcReturnsResponse>>> GetPagedReturnsAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST,"Invalid Request object","Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcReturnsResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/paged-returns-list";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcReturnsResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcReturnsResponse>>(error);
            }
        }
        
        public async Task<GrcResponse<ServiceResponse>> DeleteReturnAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST,"Return record cannot be null", "Invalid Return document record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE RETURN/REPORT REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/delete-return";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY,"Network error occurred",httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR,"An unexpected error occurred","Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }
        
        public async Task<GrcResponse<PagedResponse<GrcFrequencyResponse>>> GetFrequencyReturnsAsync(TableListRequest request) {

            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcFrequencyResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/frequency-returns";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcFrequencyResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcFrequencyResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcReturnsResponse>>> GetReturnsListAsync(TableListRequest model) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcReturnsResponse>>(error);
                }

                var request = new GrcRecordListRequest(){
                    RecordId = model.ActivityTypeId ?? 0,
                    UserId = model.UserId,
                    Action = model.Action,
                    SearchTerm = model.SearchTerm,
                    IPAddress = model.IPAddress,
                    PageIndex  = model.PageIndex,
                    PageSize = model.PageSize,
                    SortBy = model.SortBy,
                    SortDirection = model.SortDirection,
                };

                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/returns-list";
                return await HttpHandler.PostAsync<GrcRecordListRequest, PagedResponse<GrcReturnsResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcReturnsResponse>>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcReturnSubmissionResponse>>> GetReturnSubmissionsAsync(TableListRequest model) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcReturnSubmissionResponse>>(error);
                }

                var request = new GrcRecordListRequest() {
                    RecordId = model.ActivityTypeId ?? 0,
                    UserId = model.UserId,
                    Action = model.Action,
                    SearchTerm = model.SearchTerm,
                    IPAddress = model.IPAddress,
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    SortBy = model.SortBy,
                    SortDirection = model.SortDirection,
                };

                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/returns-submissions-list";
                return await HttpHandler.PostAsync<GrcRecordListRequest, PagedResponse<GrcReturnSubmissionResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcReturnSubmissionResponse>>(error);
            }
        }

        #endregion

        #region Submissions

        public async Task<GrcResponse<GrcReturnSubmissionResponse>> GetSubmissionAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/submission-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcReturnSubmissionResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcReturnSubmissionResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateSubmissionAsync(SubmissionViewModel submission, long userId, string ipAddress) {

            try {

                if (submission == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Submission record cannot be null", "Invalid Submission record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcReturnSubmissionRequest() {
                    Id = submission.Id,
                    Status = submission.Status,
                    File = submission.File,
                    BreachReason = submission.Reason,
                    Comments = submission.Comments,
                    OwnerId = submission.OwnerId,
                    SubmittedBy = submission.SubmittedBy,
                    SubmittedOn = submission.SubmittedOn,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "Update Return Submission"
                };

                //..map request
                Logger.LogActivity($"UPDATE SUBMISSION REQUEST : {JsonSerializer.Serialize(request)}");
                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/update-submission";
                Logger.LogActivity($"Endpoint: {endpoint}");
                return await HttpHandler.PostAsync<GrcReturnSubmissionRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);
            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<GrcCircularSubmissionResponse>> GetCircularSubmissionAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/submission-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcCircularSubmissionResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcCircularSubmissionResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateCircularSubmissionAsync(CircularSubmissionViewModel submission, long userId, string ipAddress) {
            try {

                if (submission == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Submission record cannot be null", "Invalid Submission record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcCircularSubmissionRequest() {
                    Id = submission.CircularId,
                    Reference = submission.Reference,
                    DepartmentId = submission.DepartmentId,
                    Status = submission.Status,
                    FilePath = submission.FilePath,
                    IsBreached = submission.IsBreached,
                    BreachReason = submission.BreachReason,
                    Comments = submission.Comments,
                    SubmittedBy = submission.SubmittedBy,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "Update Circular Submission"
                };

                //..map request
                Logger.LogActivity($"UPDATE SUBMISSION REQUEST : {JsonSerializer.Serialize(request)}");
                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/update-submission";
                Logger.LogActivity($"Endpoint: {endpoint}");
                return await HttpHandler.PostAsync<GrcCircularSubmissionRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);
            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        #endregion

        #region Circulars

        public async Task<GrcResponse<GrcCircularsResponse>> GetCircularAsync(GrcIdRequest request){
            try {
                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcCircularsResponse>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/retrieve-circular-record";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcCircularsResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcCircularsResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcCircularsResponse>>> GetPagedCircularAsync(TableListRequest request) {
            try {
                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcCircularsResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/paged-circulars-list";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcCircularsResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcCircularsResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateCircularRecordAsync(CircularViewModel model, long userId, string ipAddress) {
            try {

                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Circular issue record cannot be null", "Invalid circular issue");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                var request = new GrcCircularRequest {
                    Reference = (model.Reference ?? string.Empty).Trim(),
                    CircularTitle = model.CircularTitle ?? string.Empty,
                    Requirement = model.Requirement ?? string.Empty,
                    OwnerId = model.OwnerId,
                    AuthorityId = model.AuthorityId,
                    FrequencyId = model.FrequencyId,
                    Status = model.Status ?? "UNKNOWN",
                    RecievedOn = model.RecievedOn,
                    DeadlineOn = model.DeadlineOn,
                    BreachRisk = model.BreachRisk ?? string.Empty,
                    Comments = model.Comments ?? string.Empty,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = "Create circular record"
                };

                //..map request
                Logger.LogActivity($"CREATE CIRCULAR REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/create-circular";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcCircularRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateCircularAsync(CircularViewModel model, long userId, string ipAddress) {
            try {

                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Circular request cannot be null", "Invalid Issue record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                var request = new GrcCircularRequest {
                    Id = model.Id,
                    Reference = (model.Reference ?? string.Empty).Trim(),
                    CircularTitle = model.CircularTitle ?? string.Empty,
                    Requirement = model.Requirement ?? string.Empty,
                    OwnerId = model.OwnerId,
                    AuthorityId = model.AuthorityId,
                    FrequencyId = model.FrequencyId,
                    Status = model.Status ?? "UNKNOWN",
                    RecievedOn = model.RecievedOn,
                    DeadlineOn = model.DeadlineOn,
                    BreachRisk = model.BreachRisk ?? string.Empty,
                    Comments = model.Comments ?? string.Empty,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = "Create circular record"
                };

                //..map request
                Logger.LogActivity($"UPDATE CIRCULAR REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/update-circular";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcCircularRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteCircularAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Circular record cannot be null", "Invalid Request record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE CIRCULAR REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/delete-circular";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        #endregion

        #region Circular Issues

        public async Task<GrcResponse<GrcCircularIssueResponse>> GetIssueAsyncAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Circular issue record cannot be null", "Invalid Request record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<GrcCircularIssueResponse>(error);
                }

                //..map request
                Logger.LogActivity($"GET CIRCULAR ISSUE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/circular-issue-retrieve";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, GrcCircularIssueResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<GrcCircularIssueResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcCircularIssueResponse>(error);
            }
        }

        public async Task<GrcResponse<List<GrcCircularIssueResponse>>> GetCircularIssuesAsyncAsync(GrcCircularIssueListRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Circular issue request cannot be null", "Invalid Request record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<List<GrcCircularIssueResponse>>(error);
                }

                //..map request
                Logger.LogActivity($"GET LIST OF CIRCULAR ISSUE REQUESTS : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/circular-issues";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcCircularIssueListRequest, List<GrcCircularIssueResponse>>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<List<GrcCircularIssueResponse>>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<List<GrcCircularIssueResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateIssueAsync(CircularIssueViewModel model, long userId, string ipAddress) {
            try {

                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Circular issue record cannot be null", "Invalid circular issue");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                var request = new GrcCircularIssueRequest {
                    CircularId = model.CircularId,
                    IssueDescription = (model.IssueDescription ?? string.Empty).Trim(),
                    Resolution = model.Resolution ?? string.Empty,
                    IsDeleted = model.IsDeleted,
                    Status = model.Status,
                    RecievedOn = model.RecievedOn,
                    ResolvedOn = model.ResolvedOn,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.COMPLIANCE_ISSUE_CREATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CREATE CIRCULAR ISSUE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/create-circular-issue";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcCircularIssueRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateIssueAsync(CircularIssueViewModel model, long userId, string ipAddress) {
            try {

                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Circular issue request cannot be null", "Invalid Issue record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                var request = new GrcCircularIssueRequest {
                    Id = model.Id,
                    CircularId = model.CircularId,
                    IssueDescription = (model.IssueDescription ?? string.Empty).Trim(),
                    Resolution = model.Resolution ?? string.Empty,
                    IsDeleted = model.IsDeleted,
                    Status = model.Status,
                    RecievedOn = model.RecievedOn,
                    ResolvedOn = model.ResolvedOn,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.COMPLIANCE_ISSUE_UPDATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"UPDATE CIRCULAR ISSUE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/update-circular-issue";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcCircularIssueRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteIssueAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Circular issue record cannot be null", "Invalid Request record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE CIRCULAR ISSUE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/delete-circular-issue";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        #endregion

    }
}
