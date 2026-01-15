using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
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
        public async Task<GrcResponse<AuditDashboardResponse>> GetAuditStatisticAsync(long userId, string iPAddress) {
            return await Task.FromResult(new GrcResponse<AuditDashboardResponse>() {
                HasError = false,
                Data = new AuditDashboardResponse() { 
                    Findings = new() {
                        {"Total", 80 },
                        {"< 1 Month", 10 },
                        {"1 to 2 Months", 50 },
                        {"> 3 Months", 20 }
                    },
                    Completions = new() {
                        {"Closed", 70 },
                        {"Open", 60 },
                        {"Over Due", 30 }
                    },
                    BarChart = new() {
                        {"BOU", new Dictionary<string, int>(){
                            {"Closed", 20 },
                            {"Open", 30 },
                            {"Over Due", 10 }
                        } },
                        {"KPMG",  new Dictionary<string, int>(){
                            {"Closed", 20 },
                            {"Open", 40 },
                            {"Over Due",15 }
                        } },
                        {"IRA",  new Dictionary<string, int>(){
                            {"Closed", 30 },
                            {"Open", 10 },
                            {"Over Due", 30 }
                        } },
                        {"ER",  new Dictionary<string, int>(){
                            {"Closed", 8 },
                            {"Open", 12 },
                            {"Over Due", 5 }
                        } }
                    }
                }
            });
        }

        public async Task<GrcResponse<GrcAuditMiniReportResponse>> GetAuditExceptionReportAsync(GrcIdRequest request) {
            return await Task.FromResult(new GrcResponse<GrcAuditMiniReportResponse>() {
                Data = new() {
                    Id = 1,
                    Reference = "BOU OUT/1",
                    ReportName = "Sample BOU Report",
                    Status = "Pending",
                    Total = 30,
                    Closed = 10,
                    Open = 12,
                    Overdue = 8,
                    AuditedOn = new DateTime(2024, 2, 15),
                    Completed = 80.0M,
                    Outstanding = 20.0M,
                    Exceptions = new List<GrcAuditExceptionResponse> {
                        new() {
                            Id = 1,
                            Finding = "Sample finding",
                            ProposedAction = "Sample proposed fix",
                            Notes = "Sample proposed notes",
                            TargetDate = new DateTime(2026,4,12),
                            RiskStatement = "CRITICAL",
                            RiskRating = 70,
                            Responsible = "MD's Office",
                            Excutioner = "Seccretary",
                            Status = "CLOSED",
                        },
                        new() {
                            Id = 1,
                            Finding = "Sample finding 2",
                            ProposedAction = "Sample proposed fix 2",
                            Notes = "Sample proposed notes 2",
                            TargetDate = new DateTime(2026,6,12),
                            RiskStatement = "MEDIUM",
                            RiskRating = 70,
                            Responsible = "MD's Office",
                            Excutioner = "Seccretary",
                            Status = "OPEN",
                        }
                        ,
                        new() {
                            Id = 1,
                            Finding = "Sample finding 3",
                            ProposedAction = "Sample proposed fix 3",
                            Notes = "Sample proposed notes 3",
                            TargetDate = new DateTime(2025,12,23),
                            RiskStatement = "LOW",
                            RiskRating = 70,
                            Responsible = "MD's Office",
                            Excutioner = "Seccretary",
                            Status = "OVER DUE",
                        }
                    }
                }
            });
        }

        public async Task<GrcResponse<AuditExtensionStatistics>> GetAuditExtensionStatisticAsync(long userId, string iPAddress, string period) {
            var statuses = new Dictionary<string, int>() { {"Total", 80 }, {"Open", 10 }, {"Close", 50 },{"Due", 20 }};

            return await Task.FromResult(new GrcResponse<AuditExtensionStatistics>() {
                Data = new AuditExtensionStatistics() {
                    Statuses = statuses,
                    Reports = new List<GrcAuditMiniReportResponse>() {
                        new () { 
                            Id =  1,
                            Reference = "BOU OUT/1",
                            ReportName = "Sample BOU Report",
                            Total = 30,
                            Closed = 10,
                            Open = 12,
                            Overdue = 8,
                            AuditedOn = new DateTime(2024,2,15),
                            Completed = 80.0M,
                            Outstanding = 20.0M
                        },
                        new () {
                            Id =  3,
                            Reference = "KMGT EXT/1",
                            ReportName = "Sample KMGT Report",
                            Total = 20,
                            Closed = 8,
                            Open = 10,
                            Overdue = 2,
                            AuditedOn = new DateTime(2024,6,15),
                            Completed = 40.0M,
                            Outstanding = 20.0M
                        },
                        new () {
                            Id =  3,
                            Reference = "IR EXT/1",
                            ReportName = "Sample IR Report",
                            Total = 50,
                            Closed = 50,
                            Open = 0,
                            Overdue = 0,
                            AuditedOn = new DateTime(2024,3,15),
                            Completed = 100.0M,
                            Outstanding = 0.0M
                        }
                    }
                }
            });
        }

        public async Task<GrcResponse<PagedResponse<GrcAuditMiniReportResponse>>> GetAuditMiniReportAsync(AuditListViewModel request, long userId, string ipAddress) {
            return await Task.FromResult(new GrcResponse<PagedResponse<GrcAuditMiniReportResponse>>() { 
                Data = new PagedResponse<GrcAuditMiniReportResponse>() {
                    Entities = new List<GrcAuditMiniReportResponse>() {
                        new () {
                            Id =  1,
                            Reference = "BOU OUT/1",
                            ReportName = "Sample BOU Report",
                            Total = 30,
                            Closed = 10,
                            Open = 12,
                            Overdue = 8,
                            AuditedOn = new DateTime(2024,2,15),
                            Completed = 80.0M,
                            Outstanding = 20.0M
                        },
                        new () {
                            Id =  2,
                            Reference = "KMGT EXT/1",
                            ReportName = "Sample KMGT Report",
                            Total = 20,
                            Closed = 8,
                            Open = 10,
                            Overdue = 2,
                            AuditedOn = new DateTime(2024,6,15),
                            Completed = 40.0M,
                            Outstanding = 20.0M
                        },
                        new () {
                            Id =  3,
                            Reference = "IR EXT/1",
                            ReportName = "Sample IR Report",
                            Total = 50,
                            Closed = 50,
                            Open = 0,
                            Overdue = 0,
                            AuditedOn = new DateTime(2024,3,15),
                            Completed = 100.0M,
                            Outstanding = 0.0M
                        }
                    },
                    Page = 1,
                    TotalPages = 1,
                    Size = 10,
                    TotalCount = 10

                }
            });
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

                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/types-list";
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

                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/reports-list";
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
                var request = new AuditReportRequest() {
                    Reference = model.Reference,
                    ReportName = model.ReportName,
                    Summery = model.Summery,
                    ReportStatus = model.ReportStatus,
                    ReportDate = model.ReportDate,
                    ExceptionCount = model.ExceptionCount,
                    ResponseDate = model.ResponseDate,
                    ManagementComments = model.ManagementComments,
                    AdditionalNotes = model.AdditionalNotes,
                    AuditTypeId = model.AuditTypeId,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = Activity.AUDIT_CREATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CREATE AUDIT REPORT REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/create-report";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<AuditReportRequest, ServiceResponse>(endpoint, request);
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
                var request = new AuditReportRequest() {
                    Id = model.Id,
                    Reference = model.Reference,
                    ReportName = model.ReportName,
                    Summery = model.Summery,
                    ReportStatus = model.ReportStatus,
                    ReportDate = model.ReportDate,
                    ExceptionCount = model.ExceptionCount,
                    ResponseDate = model.ResponseDate,
                    ManagementComments = model.ManagementComments,
                    AdditionalNotes = model.AdditionalNotes,
                    AuditTypeId = model.AuditTypeId,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = Activity.AUDIT_UPDATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"UPDATE AUDIT REPORT REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/update-report";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<AuditReportRequest, ServiceResponse>(endpoint, request);
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

        public async Task<GrcResponse<PagedResponse<GrcAuditExceptionResponse>>> GetAuditExceptionsAsync(AuditCategoryViewModel model) {
            try {

                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcAuditExceptionResponse>>(error);
                }

                var request = new GrcAuditCategoryRequest() {
                    ReportId = model.ReportId,
                    Status = model.Status,
                    UserId = model.UserId,
                    IPAddress = model.IPAddress,
                    Action = model.Action,
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
                var request = new AuditExceptionRequest() {
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
                    Action = Activity.AUDIT_EXCEPTIONCREATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"CREATE AUDIT EXCEPTION REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/create-exception";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<AuditExceptionRequest, ServiceResponse>(endpoint, request);
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
                var request = new AuditExceptionRequest() {
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
                    Action = Activity.AUDIT_EXCEPTIONUPDATE.GetDescription()
                };

                //..map request
                Logger.LogActivity($"UPDATE AUDIT EXCEPTION REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/update-exception";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<AuditExceptionRequest, ServiceResponse>(endpoint, request);
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

        public async Task<GrcResponse<PagedResponse<GrcAuditUpdateResponse>>> GetAuditUpdatesAsync(GrcAuditUpdateRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcAuditUpdateResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/exceptions-updates";
                return await HttpHandler.PostAsync<GrcAuditUpdateRequest, PagedResponse<GrcAuditUpdateResponse>>(endpoint, request);
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
                var request = new AuditUpdateRequest() {
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

                return await HttpHandler.PostAsync<AuditUpdateRequest, ServiceResponse>(endpoint, request);
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
                var request = new AuditUpdateRequest() {
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

                return await HttpHandler.PostAsync<AuditUpdateRequest, ServiceResponse>(endpoint, request);
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

        #region Audit Updates

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

        public async Task<GrcResponse<PagedResponse<GrcAuditTaskResponse>>> GetAuditTasksAsync(GrcAuditTaskRequest request) {
            try {

                if (request == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Invalid Request object", "Request object cannot be null");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<PagedResponse<GrcAuditTaskResponse>>(error);
                }

                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/audit-tasks-list";
                return await HttpHandler.PostAsync<GrcAuditTaskRequest, PagedResponse<GrcAuditTaskResponse>>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected Error: {ex.Message}", "ERROR");
                Logger.LogActivity(ex.StackTrace, "STACKTRACE");
                await ProcessErrorAsync(ex.Message, "AUDIT-SERVICE", ex.StackTrace);
                var error = new GrcResponseError(GrcStatusCodes.SERVERERROR, "An unexpected error occurred", "Cannot proceed! An error occurred, please try again later");
                return new GrcResponse<PagedResponse<GrcAuditTaskResponse>>(error);
            }
        }

        public async Task<GrcResponse<ServiceResponse>> CreateAuditTaskAsync(AuditTaskViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit task record cannot be null", "Invalid Audit task record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new AuditTaskRequest() {
                    TaskName = model.TaskName,
                    TaskDescription = model.TaskDescription,
                    Duedate = model.Duedate,
                    TaskStatus = model.TaskStatus,
                    OwnerId = model.OwnerId,
                    ExceptionId = model.ExceptionId,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "CREATE EXCPTION TASK"
                };

                //..map request
                Logger.LogActivity($"CREATE AUDIT TASK REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/create-task";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<AuditTaskRequest, ServiceResponse>(endpoint, request);
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

        public async Task<GrcResponse<ServiceResponse>> UpdateAuditUpdateAsync(AuditTaskViewModel model, long userId, string ipAddress) {
            try {
                if (model == null) {
                    var error = new GrcResponseError(GrcStatusCodes.BADREQUEST, "Audit task record cannot be null", "Invalid Audit task record");
                    Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                    return new GrcResponse<ServiceResponse>(error);
                }

                //..build request object
                var request = new AuditTaskRequest() {
                    Id = model.Id,
                    TaskName = model.TaskName,
                    TaskDescription = model.TaskDescription,
                    Duedate = model.Duedate,
                    TaskStatus = model.TaskStatus,
                    OwnerId = model.OwnerId,
                    ExceptionId = model.ExceptionId,
                    IsDeleted = model.IsDeleted,
                    UserId = userId,
                    IPAddress = ipAddress,
                    Action = "UPDATE EXCPTION TASK"
                };

                //..map request
                Logger.LogActivity($"UPDATE AUDIT TASK REQUEST : {JsonSerializer.Serialize(request)}");

                //..build endpoint
                var endpoint = $"{EndpointProvider.Compliance.AuditBase}/update-task";
                Logger.LogActivity($"Endpoint: {endpoint}");

                return await HttpHandler.PostAsync<AuditTaskRequest, ServiceResponse>(endpoint, request);
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

        public async Task<GrcResponse<ServiceResponse>> DeleteAuditTaskAsync(GrcIdRequest request) {
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
