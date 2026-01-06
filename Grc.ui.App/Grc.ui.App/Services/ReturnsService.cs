using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Utils;
using System.Net;
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

        public async Task<GrcResponse<CircularMiniDashboardResponses>> GetStatusCircularCountAsync(long userId, string ipAddress, string status) {
            try {

                var request = new GrcStatusStatisticRequest() {
                    UserId = userId,
                    Status = (status ?? string.Empty).Trim(),
                    IPAddress = ipAddress,
                    Action = Activity.CIRCULAR_STATUS_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CIRCULAR STATUS STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/status-statistics";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcStatusStatisticRequest, CircularMiniDashboardResponses>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<CircularMiniDashboardResponses>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<CircularMiniDashboardResponses>(error);
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
                var endpoint = $"{EndpointProvider.Compliance.CircularBase}/authorities-statistics";
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

        public async Task<GrcResponse<ReturnsMiniDashboardResponses>> GetStatusReturnStatisticAsync(long userId, string ipAddress, string status) {
            try {

                var request = new GrcStatusStatisticRequest() {
                    UserId = userId,
                    Status = (status ?? string.Empty).Trim(),
                    IPAddress = ipAddress,
                    Action = Activity.RETURNS_STATUS_STATISTIC.GetDescription()
                };

                //..map request
                Logger.LogActivity($"RETURNS STATUS STATISTIC REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/status-statistics";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcStatusStatisticRequest, ReturnsMiniDashboardResponses>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ReturnsMiniDashboardResponses>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ReturnsMiniDashboardResponses>(error);
            }
        }

        public async Task<GrcResponse<ReturnsMiniDashboardResponses>> GetPeriodReturnStatisticAsync(long userId, string ipAddress, string period) {
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
                var endpoint = $"{EndpointProvider.Compliance.ReturnBase}/period-statistics";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcPeriodStatisticRequest, ReturnsMiniDashboardResponses>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "RETURNS-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ReturnsMiniDashboardResponses>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "RETURNS-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ReturnsMiniDashboardResponses>(error);
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

        #endregion

        #region Circulars

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

        #endregion

    }
}
