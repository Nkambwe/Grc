using Grc.ui.App.Defaults;
using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Grc.ui.App.Controllers {

    public class ComplianceAuditController : GrcBaseController {

        private readonly IAuthenticationService _authService;
        private readonly ISystemAccessService _accessService;
        private readonly IDashboardFactory _dashboardFactory;
        private readonly IAuditService _auditService;
        public ComplianceAuditController(IApplicationLoggerFactory loggerFactory, 
            IEnvironmentProvider environment, 
            IWebHelper webHelper, 
            ILocalizationService localizationService,
            IAuditService auditService,
            IErrorService errorService, 
            IAuthenticationService authService, 
            IDashboardFactory dashboardFactory, 
            IGrcErrorFactory errorFactory, 
            SessionManager sessionManager) 
            : base(loggerFactory, environment, webHelper, localizationService, errorService, errorFactory, sessionManager) {
            Logger.Channel = $"AUDITS-{DateTime.Now:yyyyMMddHHmmss}";
             _authService = authService;
            _auditService = auditService;
            _dashboardFactory = dashboardFactory;
        }

        #region Statistics
        public async Task<IActionResult> AuditDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-AUDIT-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Dashboard", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareAuditDashboardModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> MonthLessDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "AUDIT-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Dashboard", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareAuditExtensionDashboardModelAsync(grcResponse.Data, "MONTHLESS"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> TwoMonthsLessDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "AUDIT-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Dashboard", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareAuditExtensionDashboardModelAsync(grcResponse.Data, "TWOMONTHSLESS"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ThreeMonthsAboveDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "AUDIT-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Dashboard", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareAuditExtensionDashboardModelAsync(grcResponse.Data, "THREEMONTHSABOVE"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> AuditReports() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-AUDIT-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("AuditHome", "ComplianceAudit"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserDashboardModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> AuditExceptions() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-AUDIT-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("AuditHome", "ComplianceAudit"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserDashboardModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> AuditTypes() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-AUDIT-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("AuditHome", "ComplianceAudit"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserDashboardModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        [LogActivityResult("Retrieve Act", "User retrieved legal act", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_EXCEPTIONS, "AuditException")]
        public async Task<IActionResult> GetAuditExceptionReport(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Legal Audit Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.AUDIT_EXCEPTION_RETRIVE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _auditService.GetAuditExceptionReportAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving legacl Act";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var exceRecord = new {
                    id = response.Id,
                    reference = response.Reference ?? string.Empty,
                    reportName = response.ReportName ?? string.Empty,
                    auditedOn = response.AuditedOn,
                    status = response.Status ?? string.Empty,
                    total = response.Total,
                    open = response.Open,
                    close = response.Closed,
                    overdue = response.Overdue,
                    exceptions = response.Exceptions.Select(a => new {
                        id = a.Id,
                        findings = a.Finding,
                        proposedAction = a.ProposedAction,
                        status = a.Status,
                        notes = a.Notes,
                        targetDate = a.TargetDate,
                        riskStatement = a.RiskStatement,
                        riskrating = a.RiskRating,
                        responsible = a.Responsible,
                        excutioner = a.Excutioner,
                    }).ToList()
                };

                return Ok(new { success = true, data = exceRecord });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving regulatory act: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Failed to retrieve regulatory acts, an Unexpected error occurred", data = new { } });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetMiniExceptionReports([FromBody] AuditListViewModel request) {
            try {

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) {
                    Logger.LogActivity($"AUDIT EXCEPTION REPORT DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(userResponse)}");
                }

                var currentUser = userResponse.Data;
                var result = await _auditService.GetAuditMiniReportAsync(request, currentUser.UserId, ipAddress);
                PagedResponse<GrcAuditMiniReportResponse> list = new();
                if (result.HasError) {
                    Logger.LogActivity($"AUDIT EXCEPTION REPORT DATA ERROR: Failed to retrieve category statutes - {JsonSerializer.Serialize(result)}");
                } else {
                    list = result.Data;
                    Logger.LogActivity($"AUDIT EXCEPTION REPORT DATA - {list.TotalCount}");
                }

                var reportData = list.Entities ?? new();
                if (reportData.Any()) {
                    var reports = reportData.Select(report => new {
                        id = report.Id,
                        reference = report.Reference,
                        reportName = report.ReportName,
                        total = report.Total,
                        closed = report.Closed,
                        open = report.Open,
                        overdue = report.Overdue,
                        auditedOn = report.AuditedOn,
                        completed = report.Completed,
                        outstanding = report.Outstanding
                    }).ToList();
                    return Ok(new { last_page = 1, total_records = reports.Count, data = reports });
                }

                return Ok(new { last_page = 0, total_records = 0, data = Array.Empty<object>() });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving regulatory acts: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-ACTS", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

        #region Audit Types

        [LogActivityResult("Retrieve Audit type", "User retrieved audit type", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_AUDIT_TYPES, "AuditType")]
        public async Task<IActionResult> GetAuditType(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Type Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _auditService.GetAuditTypeAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving audit type";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var report = new {
                    id = response.Id,
                    typeCodeTypeCodearticle = response.TypeCode ?? string.Empty,
                    typeName = response.TypeName ?? string.Empty,
                    description = response.Description ?? string.Empty,
                    isDeleted = response.IsDeleted
                };

                return Ok(new { success = true, data = report });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving audit type: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to retrieve audit type.Something went wrong" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAuditTypes([FromBody] TableListRequest request) {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"AUDIT TYPES DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.AUDIT_TYPE_RETRIVE.GetDescription();

                //..map to ajax object
                var returnsData = await _auditService.GetAuditTypesAsync(request);
                PagedResponse<GrcAuditTypeResponse> returnsList = new();

                if (returnsData.HasError) {
                    Logger.LogActivity($"AUDIT TYPES DATA ERROR: Failed to retrieve audit types - {JsonSerializer.Serialize(returnsData)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                } else {
                    returnsList = returnsData.Data;
                    Logger.LogActivity($"AUDIT TYPES DATA - {JsonSerializer.Serialize(returnsList)}");
                }

                var pagedEntities = returnsList.Entities
                    .Select(report => new {
                        id = report.Id,
                        TypeCode = report.TypeCode ?? string.Empty,
                        typeName = report.TypeName ?? string.Empty,
                        description = report.Description ?? string.Empty,
                        isDeleted = report.IsDeleted 
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)returnsList.TotalCount / returnsList.Size);
                return Ok(new {
                    last_page = totalPages,
                    total_records = returnsList.TotalCount,
                    data = pagedEntities
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving audit types: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Add Audit type", "User added audit type", ActivityTypeDefaults.COMPLIANCE_CREATE_RETURN, "AuditType")]
        public async Task<IActionResult> CreateAuditType([FromBody] AuditTypeViewModel request) {
            try {
                if (!ModelState.IsValid) {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    string combinedErrors = string.Join("; ", errors);
                    return Ok(new { success = false, message = $"Please correct these errors: {combinedErrors}", data = (object)null });
                }

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                if (request == null) {
                    return Ok(new { success = false, message = "Invalid audit type data" });
                }


                var result = await _auditService.CreateAuditTypeAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create audit type" });

                var created = result.Data;
                return Ok(new { success = true, message = "Audit type created successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving audit type: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to create audit type.Something went wrong" });
            }
        }

        [HttpPost]
        [LogActivityResult("Update audit type", "User updated audit type", ActivityTypeDefaults.COMPLIANCE_EDITED_AUDIT_TYPE, "AuditType")]
        public async Task<IActionResult> UpdateAuditType([FromBody] AuditTypeViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                var result = await _auditService.UpdateAuditTypeAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update audit type" });

                var updated = result.Data;
                return Ok(new {
                    success = true,
                    message = "Audit type updated successfully",
                    data = new { }
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating audit type: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to updating audit type.Something went wrong" });
            }
        }

        [HttpPost]
        [LogActivityResult("Delete Audit type", "User delete audit type", ActivityTypeDefaults.COMPLIANCE_DELETED_AUDI_TYPE, "AuditType")]
        public async Task<IActionResult> DeleteAudityType(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Audit type Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.AUDIT_TYPE_DELETE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _auditService.DeleteTypeAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete audit type" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating audit type: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to updating audit type.Something went wrong" });
            }
        }

        #endregion

        #region Audit Report

        [HttpPost]
        public async Task<IActionResult> GetAuditReports([FromBody] TableListRequest request) {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"AUDIT REPORTS DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.AUDIT_TYPE_RETRIVE.GetDescription();

                //..map to ajax object
                var returnsData = await _auditService.GetAuditReportsAsync(request);
                PagedResponse<GrcAuditReportResponse> returnsList = new();

                if (returnsData.HasError) {
                    Logger.LogActivity($"AUDIT REPORTS DATA ERROR: Failed to retrieve audit reports - {JsonSerializer.Serialize(returnsData)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                } else {
                    returnsList = returnsData.Data;
                    Logger.LogActivity($"AUDIT REPORTS DATA - {JsonSerializer.Serialize(returnsList)}");
                }

                var pagedEntities = returnsList.Entities
                    .Select(report => new {
                        id = report.Id,
                        reference = report.Reference ?? string.Empty,
                        reportName = report.ReportName ?? string.Empty,
                        summery = report.Summery ?? string.Empty,
                        ReportStatus = report.ReportStatus ?? string.Empty,
                        reportDate = report.ReportDate,
                        exceptionCount = report.ExceptionCount,
                        responseDate = report.ResponseDate,
                        ManagementComments = report.ManagementComments ?? string.Empty,
                        AdditionalNotes = report.AdditionalNotes ?? string.Empty,
                        AuditType = report.AuditType ?? string.Empty,
                        isDeleted = report.IsDeleted
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)returnsList.TotalCount / returnsList.Size);
                return Ok(new {
                    last_page = totalPages,
                    total_records = returnsList.TotalCount,
                    data = pagedEntities
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving audit reports: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

        #region Audit Exceptions

        [HttpPost]
        public async Task<IActionResult> GetAuditExceptions([FromBody] AuditCategoryViewModel request) {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"AUDIT EXCEPTION DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.AUDIT_TYPE_RETRIVE.GetDescription();

                //..map to ajax object
                var returnsData = await _auditService.GetAuditExceptionsAsync(request);
                PagedResponse<GrcAuditExceptionResponse> returnsList = new();

                if (returnsData.HasError) {
                    Logger.LogActivity($"AUDIT EXCEPTION DATA ERROR: Failed to retrieve audit types - {JsonSerializer.Serialize(returnsData)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                } else {
                    returnsList = returnsData.Data;
                    Logger.LogActivity($"AUDIT EXCEPTION DATA - {JsonSerializer.Serialize(returnsList)}");
                }

                var pagedEntities = returnsList.Entities
                    .Select(report => new {
                        id = report.Id,
                        reportId = report.ReportId,
                        finding = report.Finding ?? string.Empty,
                        proposedAction = report.ProposedAction ?? string.Empty,
                        notes = report.Notes ?? string.Empty,
                        targetDate = report.TargetDate,
                        riskStatement = report.RiskStatement ?? string.Empty,
                        riskRating = report.RiskRating,
                        status = report.Status ?? string.Empty,
                        responsibleId = report.ResponsibleId,
                        excutioner = report.Excutioner ?? string.Empty,
                        isDeleted = report.IsDeleted,
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)returnsList.TotalCount / returnsList.Size);
                return Ok(new {
                    last_page = totalPages,
                    total_records = returnsList.TotalCount,
                    data = pagedEntities
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving audit types: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion
    }

}
