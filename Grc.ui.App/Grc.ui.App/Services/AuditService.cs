using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
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
using System;
using System.Net;
using System.Text.Json;

namespace Grc.ui.App.Services {
    public class AuditService : GrcBaseService, IAuditService {
        public AuditService(IApplicationLoggerFactory loggerFactory, 
            IHttpHandler httpHandler, 
            IEnvironmentProvider environment, 
            IEndpointTypeProvider endpointType, 
            IMapper mapper, 
            IWebHelper webHelper, 
            SessionManager sessionManager, 
            IGrcErrorFactory errorFactory, 
            IErrorService errorService) 
            : base(loggerFactory, httpHandler, environment, endpointType, mapper, 
                  webHelper, sessionManager, errorFactory, errorService) {
        }

        #region Dashboard

        public async Task<GrcResponse<GrcAuditDashboardResponse>> GetAuditStatisticAsync(long userId, string iPAddress) {
            try {
                var request = new GrcRequest() {
                    UserId = userId,
                    IPAddress = iPAddress,
                    Action = "Audit dashboard statistics request",
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/audit-dashboard-statistics";
                return await HttpHandler.PostAsync<GrcRequest, GrcAuditDashboardResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcAuditDashboardResponse>(error);
            }
        }

        public async Task<GrcResponse<GrcAuditMiniReportResponse>> GetAuditExceptionReportAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/exception-report";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcAuditMiniReportResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcAuditMiniReportResponse>(error);
            }
        }

        public async Task<GrcResponse<AuditExtensionStatistics>> GetAuditExtensionStatisticAsync(long userId, string iPAddress, string period) {
            try {
                var request = new GrcAuditExtensionExceptionRequest() {
                    Period = period,
                    UserId = userId,
                    IpAddress = iPAddress,
                    Action = $"Retrieve exception reports for the period {period}"
                };

                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/exception-extension-report";
                return await HttpHandler.PostAsync<GrcAuditExtensionExceptionRequest, AuditExtensionStatistics>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<AuditExtensionStatistics>(error);
            }
        }

        public async Task<GrcResponse<List<GrcAuditMiniReportResponse>>> GetAuditMiniReportAsync(AuditListViewModel model, long userId, string ipAddress) {
            try {

                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<List<GrcAuditMiniReportResponse>>(error);
                }

                //..action message
                string actionMsg = "Retrieving audit mini report for ";
                actionMsg = string.IsNullOrWhiteSpace(model.PeportPeriod) ?
                    $"{actionMsg} period {model.PeportPeriod}" :
                    $"{actionMsg} Report ID {model.ReportId}";

                var request = new GrcAuditListRequest() {
                    ReportId = model.ReportId,
                    Period = model.PeportPeriod?? string.Empty,
                    SearchTerm = model.SearchTerm ?? string.Empty,
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    SortBy  = model.SortBy ?? string.Empty,
                    SortDirection   = model.SortDirection ?? string.Empty,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = actionMsg
                };

                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/audit-mini-report";
                return await HttpHandler.PostAsync<GrcAuditListRequest, List<GrcAuditMiniReportResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<List<GrcAuditMiniReportResponse>>(error);
            }
        }

        #endregion

        #region Audits

        public async Task<GrcResponse<GrcAuditResponse>> GetAuditAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/audit-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcAuditResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcAuditResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcAuditResponse>>> GetAuditsAsync(TableListRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcAuditResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/paged-audit-list";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcAuditResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcAuditResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateAuditAsync(AuditViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit record cannot be null", "Invalid Audit record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcAuditRequest() {
                    AuditName = model.AuditName,
                    Notes = model.Notes,
                    AuthorityId = model.AuthorityId,
                    TypeId = model.TypeId,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = Activity.AUDIT_CREATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CREATE AUDIT REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/create-audit";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcAuditRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDI-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateAuditAsync(AuditViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit record cannot be null", "Invalid Audit record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcAuditRequest() {
                    Id = model.Id,
                    AuditName = model.AuditName,
                    Notes = model.Notes,
                    AuthorityId = model.AuthorityId,
                    TypeId = model.TypeId,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = Activity.AUDIT_UPDATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"UPDATE AUDIT REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/update-audit";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcAuditRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDIT-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteAuditAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit record cannot be null", "Invalid Audit record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE AUDIT REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/delete-audit";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDIT-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        #endregion

        #region Audit Types
        public async Task<GrcResponse<GrcAuditTypeResponse>> GetAuditTypeAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/type-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcAuditTypeResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcAuditTypeResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcAuditTypeResponse>>> GetAuditTypesAsync(TableListRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcAuditTypeResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/paged-types-list";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcAuditTypeResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcAuditTypeResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateAuditTypeAsync(AuditTypeViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit type record cannot be null", "Invalid Audit type record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcAuditTypeRequest() {
                    TypeCode = model.TypeCode,
                    TypeName = model.TypeName,
                    Description = model.Description,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = Activity.AUDIT_TYPE_CREATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CREATE AUDIT TYPE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/create-type";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcAuditTypeRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDI-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateAuditTypeAsync(AuditTypeViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit type record cannot be null", "Invalid Audit type record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcAuditTypeRequest() {
                    Id = model.Id,
                    TypeCode = model.TypeCode,
                    TypeName = model.TypeName,
                    Description = model.Description,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = Activity.AUDIT_TYPE_UPDATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"UPDATE AUDIT TYPE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/update-type";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcAuditTypeRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDIT-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteTypeAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit type record cannot be null", "Invalid Audit type record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE AUDIT TYPE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/delete-type";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDIT-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        #endregion

        #region Audit Report

        public async Task<GrcResponse<GrcAuditReportResponse>> GetAuditReportAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/report-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcAuditReportResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcAuditReportResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcAuditReportResponse>>> GetAuditReportsAsync(TableListRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcAuditReportResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/paged-reports-list";
                return await HttpHandler.PostAsync<TableListRequest, PagedResponse<GrcAuditReportResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcAuditReportResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateAuditReportAsync(AuditReportViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit report record cannot be null", "Invalid Audit report record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcAuditReportRequest() {
                    Reference = model.Reference,
                    ReportName = model.ReportName,
                    Summery = model.Summery,
                    Status = model.ReportStatus,
                    AuditedOn = model.ReportDate,
                    ExceptionCount = model.ExceptionCount,
                    RespondedOn = model.ResponseDate,
                    ManagementComment = model.ManagementComments,
                    AdditionalNotes = model.AdditionalNotes,
                    AuditId = model.AuditId,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.AUDIT_CREATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CREATE AUDIT REPORT REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/create-report";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcAuditReportRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDI-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateAuditReportAsync(AuditReportViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit report record cannot be null", "Invalid Audit report record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcAuditReportRequest() {
                    Id = model.Id,
                    Reference = model.Reference,
                    ReportName = model.ReportName,
                    Summery = model.Summery,
                    Status = model.ReportStatus,
                    AuditedOn = model.ReportDate,
                    ExceptionCount = model.ExceptionCount,
                    RespondedOn = model.ResponseDate,
                    ManagementComment = model.ManagementComments,
                    AdditionalNotes = model.AdditionalNotes,
                    AuditId = model.AuditId,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = Activity.AUDIT_UPDATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"UPDATE AUDIT REPORT REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/update-report";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcAuditReportRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDIT-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteReportAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit report record cannot be null", "Invalid Audit report record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE AUDIT REPORT REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/delete-report";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDIT-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        #endregion

        #region Audit Exceptions
        public async Task<GrcResponse<GrcAuditExceptionResponse>> GetAuditExceptionAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/exception-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcAuditExceptionResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcAuditExceptionResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcAuditExceptionResponse>>> GetAuditExceptionsAsync(AuditCategoryViewModel model, long userId, string ipAddress, string action) {
            try {

                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcAuditExceptionResponse>>(error);
                }

                var request = new GrcAuditCategoryRequest() {
                    ReportId = model.ReportId,
                    Status = model.Status,
                    SearchTerm = model.SearchTerm ?? string.Empty,
                    SortDirection = model.SortDirection ?? string.Empty,
                    SortBy = model.SortBy ?? string.Empty,
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = action,
                };

                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/report-exceptions";
                return await HttpHandler.PostAsync<GrcAuditCategoryRequest, PagedResponse<GrcAuditExceptionResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcAuditExceptionResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateAuditExceptionAsync(AuditExceptionViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit report record cannot be null", "Invalid Audit report record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcAuditExceptionRequest() {
                    ReportId = model.ReportId,
                    Findings = model.Findings,
                    Recomendations = model.Recomendations,
                    ProposedAction = model.ProposedAction,
                    Notes = model.Notes,
                    ResponsibileId = model.ResponsibileId,
                    Executioner = model.Executioner,
                    TargetDate = model.TargetDate,
                    RiskLevel = model.RiskLevel,
                    RiskRate = model.RiskRate,
                    Status = model.Status,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = Activity.AUDIT_EXCEPTION_CREATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CREATE AUDIT EXCEPTION REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/create-exception";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcAuditExceptionRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDI-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateAuditExceptionAsync(AuditExceptionViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit exception record cannot be null", "Invalid Audit exception record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcAuditExceptionRequest() {
                    Id = model.Id,
                    ReportId = model.ReportId,
                    Findings = model.Findings,
                    Recomendations = model.Recomendations,
                    ProposedAction = model.ProposedAction,
                    Notes = model.Notes,
                    ResponsibileId = model.ResponsibileId,
                    Executioner = model.Executioner,
                    TargetDate = model.TargetDate,
                    RiskLevel = model.RiskLevel,
                    RiskRate = model.RiskRate,
                    Status = model.Status,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = Activity.AUDIT_EXCEPTION_UPDATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"UPDATE AUDIT EXCEPTION REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/update-exception";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcAuditExceptionRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDIT-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteExceptionAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit exception record cannot be null", "Invalid Audit exception record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE AUDIT EXCEPTION REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/delete-exception";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDIT-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        #endregion

        #region Audit Updates

        public async Task<GrcResponse<GrcAuditUpdateResponse>> GetAuditUpdateAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/audit-update-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcAuditUpdateResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcAuditUpdateResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcAuditUpdateResponse>>> GetAuditUpdatesAsync(GrcAuditMiniUpdateRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcAuditUpdateResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/exceptions-updates";
                return await HttpHandler.PostAsync<GrcAuditMiniUpdateRequest, PagedResponse<GrcAuditUpdateResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcAuditUpdateResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateAuditUpdateAsync(AuditUpdateViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit report record cannot be null", "Invalid Audit report record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcAuditUpdateRequest() {
                    ReportId = model.ReportId,
                    UpdateNotes = model.UpdateNotes,
                    AddedDate = model.AddedDate,
                    SendReminders = model.SendReminders,
                    SendDate = model.SendDate,
                    ReminderMessage = model.ReminderMessage,
                    SendToEmails = model.SendToEmails,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "CREATE EXCPTION UPDATE NOTES"
                };

                //..map request
                Logger.LogActivity($"CREATE AUDIT EXCEPTION-NOTES REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/create-exception-update";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcAuditUpdateRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDI-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateAuditUpdateAsync(AuditUpdateViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit exception update record cannot be null", "Invalid Audit exception update record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcAuditUpdateRequest() {
                    Id = model.Id,
                    ReportId = model.ReportId,
                    UpdateNotes = model.UpdateNotes,
                    AddedDate = model.AddedDate,
                    SendReminders = model.SendReminders,
                    SendDate = model.SendDate,
                    ReminderMessage = model.ReminderMessage,
                    SendToEmails = model.SendToEmails,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "UPDATE EXCPTION UPDATE NOTES"
                };

                //..map request
                Logger.LogActivity($"UPDATE AUDIT EXCEPTION-UPDATE REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/update-exception";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcAuditUpdateRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDIT-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteAuditUpdateAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit notes update  record cannot be null", "Invalid Audit notes update record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE AUDIT NOTES REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/delete-audit-notes";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDIT-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        #endregion

        #region Audit Tasks

        public async Task<GrcResponse<GrcAuditTaskResponse>> GetAuditTaskAsync(GrcIdRequest request) {
            try {
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/audit-task-retrieve";
                return await HttpHandler.PostAsync<GrcIdRequest, GrcAuditTaskResponse>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<GrcAuditTaskResponse>(error);
            }
        }

        public async Task<GrcResponse<PagedResponse<GrcAuditTaskResponse>>> GetExceptionTasksAsync(GrcExceptionTaskViewModel model, long userId, string ipAddress) {
            try {

                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcAuditTaskResponse>>(error);
                }

                var request = new GrcAuditMiniTaskRequest() {
                    ExceptionId = model.ExceptionId,
                    SearchTerm = model.SearchTerm ?? string.Empty,
                    UserId = userId,
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    SortBy = model.SortBy ?? string.Empty,
                    SortDirection = model.SortDirection ?? string.Empty,
                    IPAddress = ipAddress ?? string.Empty,
                    Action = Activity.AUDIT_TASK_RETRIVE.GetDescription()
                };

                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/audit-tasks-list";
                return await HttpHandler.PostAsync<GrcAuditMiniTaskRequest, PagedResponse<GrcAuditTaskResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcAuditTaskResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateExceptionTaskAsync(GrcAuditTaskViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit task record cannot be null", "Invalid Audit task record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcAuditTaskRequest() {
                    TaskName = model.TaskName,
                    Description = model.Description,
                    DueDate = model.DueDate,
                    Status = model.Status,
                    OwnerId = model.OwnerId,
                    Interval = model.Interval,
                    IntervalType = model.IntervalType,
                    Reminder = model.Reminder,
                    ExceptionId = model.ExceptionId,
                    IsDeleted = model.IsDeleted,
                    SendReminder = model.SendReminder,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = "CREATE EXCPTION TASK"
                };

                //..map request
                Logger.LogActivity($"CREATE AUDIT TASK REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/create-task";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcAuditTaskRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDI-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> UpdateExceptionTaskAsync(GrcAuditTaskViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit task record cannot be null", "Invalid Audit task record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new GrcAuditTaskRequest() {
                    Id = model.Id,
                    TaskName = model.TaskName,
                    Description = model.Description,
                    DueDate = model.DueDate,
                    Status = model.Status,
                    OwnerId = model.OwnerId,
                    Interval = model.Interval,
                    IntervalType = model.IntervalType,
                    Reminder = model.Reminder,
                    ExceptionId = model.ExceptionId,
                    IsDeleted = model.IsDeleted,
                    SendReminder = model.SendReminder,
                    UserId = userId,
                    IpAddress = ipAddress,
                    Action = "UPDATE EXCPTION TASK"
                };

                //..map request
                Logger.LogActivity($"UPDATE AUDIT TASK REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/update-task";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcAuditTaskRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDIT-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteExceptionTaskAsync(GrcIdRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit task record cannot be null", "Invalid Audit task record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..map request
                Logger.LogActivity($"DELETE AUDIT TASK REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/delete-task";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<GrcIdRequest, ServiceResponse>(endpoint, request);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"HTTP Request Error: {httpEx.Message}", "ERROR");
                Logger.LogActivity(httpEx.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(httpEx.Message, "AUDIT-SERVICE", httpEx.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.BADGATEWAY, "Network error occurred", httpEx.Message);
                return new GrcResponse<ServiceResponse>(error);

            } catch (GRCException ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<ServiceResponse>(error);
            }
        }

        #endregion
    }

}
