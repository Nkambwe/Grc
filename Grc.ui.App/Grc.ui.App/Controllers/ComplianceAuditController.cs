using DocumentFormat.OpenXml.Bibliography;
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
                        excutioner = a.Executioner,
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
                List<GrcAuditMiniReportResponse> list = new();
                if (result.HasError) {
                    Logger.LogActivity($"AUDIT EXCEPTION REPORT DATA ERROR: Failed to retrieve category statutes - {JsonSerializer.Serialize(result)}");
                } else {
                    list = result.Data;
                    Logger.LogActivity($"AUDIT EXCEPTION REPORT DATA - {list.Count}");
                }

                var reportData = list ?? new();
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

        #region Audits

        [LogActivityResult("Retrieve Audit", "User retrieved audit", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_AUDIT, "Audit")]
        public async Task<IActionResult> GetAudit(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Audit Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _auditService.GetAuditAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving audit";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var report = new {
                    id = response.Id,
                    auditName = response.AuditName ?? string.Empty,
                    notes = response.Notes ?? string.Empty,
                    authorityId = response.AuthorityId,
                    authority = response.Authority ?? string.Empty,
                    typeId = response.TypeId,
                    typeName = response.TypeName ?? string.Empty,
                    isDeleted = response.IsDeleted
                };

                return Ok(new { success = true, data = report });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving audit: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to retrieve audit.Something went wrong" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAudits([FromBody] TableListRequest request) {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"AUDITS DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.AUDIT_TYPE_RETRIVE.GetDescription();

                //..map to ajax object
                var auditData = await _auditService.GetAuditsAsync(request);
                PagedResponse<GrcAuditResponse> auditList = new();

                if (auditData.HasError) {
                    Logger.LogActivity($"AUDITS DATA ERROR: Failed to retrieve audits - {JsonSerializer.Serialize(auditData)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                } else {
                    auditList = auditData.Data;
                    Logger.LogActivity($"AUDITS DATA - {JsonSerializer.Serialize(auditList)}");
                }

                var pagedEntities = auditList.Entities
                    .Select(report => new {
                        id = report.Id,
                        auditName = report.AuditName ?? string.Empty,
                        notes = report.Notes ?? string.Empty,
                        authorityId = report.AuthorityId,
                        authority = report.Authority ?? string.Empty,
                        typeId = report.TypeId,
                        typeName = report.TypeName ?? string.Empty,
                        isDeleted = report.IsDeleted
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)auditList.TotalCount / auditList.Size);
                return Ok(new {
                    last_page = totalPages,
                    total_records = auditList.TotalCount,
                    data = pagedEntities
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving audits: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Add Audit", "User added audit", ActivityTypeDefaults.COMPLIANCE_CREATE_AUDIT, "Audit")]
        public async Task<IActionResult> CreateAuditType([FromBody] AuditViewModel request) {
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
                    return Ok(new { success = false, message = "Invalid audit data" });
                }


                var result = await _auditService.CreateAuditAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create audit" });

                var created = result.Data;
                return Ok(new { success = true, message = "Audit created successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving audit: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to create audit.Something went wrong" });
            }
        }

        [HttpPost]
        [LogActivityResult("Update audit", "User updated audit", ActivityTypeDefaults.COMPLIANCE_EDITED_AUDIT, "Audit")]
        public async Task<IActionResult> UpdateAudit([FromBody] AuditViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                var result = await _auditService.UpdateAuditAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update audit" });

                var updated = result.Data;
                return Ok(new {
                    success = true,
                    message = "Audit updated successfully",
                    data = new { }
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating audit: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to updating audit.Something went wrong" });
            }
        }

        [HttpPost]
        [LogActivityResult("Delete Audit", "User delete audit", ActivityTypeDefaults.COMPLIANCE_DELETED_AUDIT, "Audit")]
        public async Task<IActionResult> DeleteAudit(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Audit Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.AUDIT_DELETE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _auditService.DeleteAuditAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete audit" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating audit: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to updating audit.Something went wrong" });
            }
        }

        #endregion

        #region Audit Types

        [LogActivityResult("Retrieve Audit type", "User retrieved audit type", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_AUDIT_TYPE, "AuditType")]
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
                    typeCode = response.TypeCode ?? string.Empty,
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
        [LogActivityResult("Add Audit type", "User added audit type", ActivityTypeDefaults.COMPLIANCE_CREATE_AUDIT_TYPE, "AuditType")]
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
        [LogActivityResult("Delete Audit type", "User delete audit type", ActivityTypeDefaults.COMPLIANCE_DELETED_AUDIT_TYPE, "AuditType")]
        public async Task<IActionResult> DeleteAuditType(long id) {
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

        [LogActivityResult("Retrieve Audit report", "User retrieved audit report", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_AUDIT_TASK, "AuditReport")]
        public async Task<IActionResult> GeAuditReport(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Report Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _auditService.GetAuditReportAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving audit report";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var report = new {
                    id = response.Id,
                    reference = response.Reference ?? string.Empty,
                    reportName = response.ReportName ?? string.Empty,
                    summery = response.Summery ?? string.Empty,
                    status = response.ReportStatus ?? string.Empty,
                    reportDate = response.ReportDate,
                    exceptions = response.ExceptionCount,
                    responseDate = response.ResponseDate,
                    managementComments = response.ManagementComments ?? string.Empty,
                    AdditionalNotes = response.AdditionalNotes ?? string.Empty,
                    typeId = response.AuditTypeId,
                    type = response.AuditType ?? string.Empty,
                    isDeleted = response.IsDeleted,
                    update = response.Updates != null && response.Updates.Any() ?
                            response.Updates.Select( update => new {
                                id = update.Id,
                                reportId = update.ReportId,
                                notes = update.UpdateNotes ?? string.Empty,
                                sendReminder = update.SendReminders,
                                sendDate = update.SendDate,
                                isDeleted = update.IsDeleted,
                                message = update.ReminderMessage ?? string.Empty,
                                emails = update.SendToEmails ?? string.Empty,
                                addedBy = update.AddedBy ?? string.Empty,
                            }).ToArray() :
                            Array.Empty<object>(),
                };

                return Ok(new { success = true, data = report });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving audit report: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to retrieve audit report.Something went wrong" });
            }
        }

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
                request.Action = Activity.AUDIT_REPORT_RETRIVE.GetDescription();

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
                        isDeleted = report.IsDeleted,
                        findings = report.Findings != null && report.Findings.Any()?
                                report.Findings.Select(exception => new { 
                                    id = exception.Id,
                                    reportId = exception.ReportId,
                                    findings = exception.Finding ?? string.Empty,
                                    targetDate = exception.TargetDate,
                                    risk = exception.RiskRating,
                                    assessment = exception.RiskStatement,
                                    status = exception.Status ?? string.Empty,
                                    responsible = exception.Responsible ?? string.Empty,
                                    excutioner = exception.Executioner ?? string.Empty,
                                }).ToArray():
                                Array.Empty<object>(),
                        updates = report.Updates != null && report.Updates.Any() ?
                                report.Updates.Select(notes => new {
                                    id = notes.Id,
                                    reportId = notes.ReportId,
                                    notes = notes.UpdateNotes ?? string.Empty,
                                    isDeleted = notes.IsDeleted

                                }).ToArray() :
                                Array.Empty<object>(),
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

        [HttpPost]
        [LogActivityResult("Add Audit report", "User added audit report", ActivityTypeDefaults.COMPLIANCE_CREATE_AUDIT_TASK, "AuditReport")]
        public async Task<IActionResult> CreateAuditReport([FromBody] AuditReportViewModel request) {
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
                    return Ok(new { success = false, message = "Invalid audit report data" });
                }


                var result = await _auditService.CreateAuditReportAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create audit report" });

                var created = result.Data;
                return Ok(new { success = true, message = "Audit report created successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving audit report: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to create audit report.Something went wrong" });
            }
        }

        [HttpPost]
        [LogActivityResult("Update audit report", "User updated audit report", ActivityTypeDefaults.COMPLIANCE_EDITED_AUDIT_TASK, "AuditReport")]
        public async Task<IActionResult> UpdateAuditReport([FromBody] AuditReportViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                var result = await _auditService.UpdateAuditReportAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update audit report" });

                var updated = result.Data;
                return Ok(new {
                    success = true,
                    message = "Audit report updated successfully",
                    data = new { }
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating audit report: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to updating audit report.Something went wrong" });
            }
        }

        [HttpPost]
        [LogActivityResult("Delete Audit report", "User delete audit report", ActivityTypeDefaults.COMPLIANCE_DELETED_AUDIT_TASK, "AuditReport")]
        public async Task<IActionResult> DeleteAuditReport(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Audit report Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.AUDIT_REPORT_DELETE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _auditService.DeleteReportAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete audit report" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating audit report: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to updating audit report.Something went wrong" });
            }
        }

        #endregion

        #region Audit Exceptions

        [LogActivityResult("Retrieve Audit exception", "User retrieved audit exception", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_EXCEPTIONS, "AuditException")]
        public async Task<IActionResult> GetAuditException(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Exception Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _auditService.GetAuditExceptionAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving audit exception";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var exception = new {
                    id = response.Id,
                    reportId = response.ReportId,
                    finding = response.Finding ?? string.Empty,
                    proposedAction = response.ProposedAction ?? string.Empty,
                    correctiveAction = response.CorrectiveAction ?? string.Empty,
                    notes = response.Notes ?? string.Empty,
                    targetDate = response.TargetDate,
                    riskStatement = response.RiskStatement ?? string.Empty,
                    riskRating = response.RiskRating,
                    responsibleId = response.ResponsibleId,
                    responsible = response.Responsible ?? string.Empty,
                    executioner = response.Executioner ?? string.Empty,
                    isDeleted = response.IsDeleted,
                    tasks = response.Tasks != null && response.Tasks.Any() ?
                        response.Tasks.Select(task => new { 
                            id = task.Id,
                            exceptionId = task.ExceptionId,
                            taskName = task.TaskName ?? string.Empty,
                            description = task.TaskDescription ?? string.Empty,
                            status = task.TaskStatus ?? string.Empty,
                            dueDate = task.Duedate,
                            ownerId = task.OwnerId,
                            owner = task.Owner?? string.Empty,
                            isDeleted = task.IsDeleted
                        }).ToArray():
                        Array.Empty<object>()
                            
                };

                return Ok(new { success = true, data = exception });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving audit exception: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to retrieve audit exception.Something went wrong" });
            }
        }

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
                var returnsData = await _auditService.GetAuditExceptionsAsync(request, currentUser.UserId, ipAddress, Activity.AUDIT_TYPE_RETRIVE.GetDescription());
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
                        excutioner = report.Executioner ?? string.Empty,
                        isDeleted = report.IsDeleted,
                        tasks = report.Tasks != null ? 0 : report.Tasks.Count
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
        [LogActivityResult("Add Audit exception", "User added audit exception", ActivityTypeDefaults.COMPLIANCE_CREATE_EXCEPTION, "AuditException")]
        public async Task<IActionResult> CreateExecption([FromBody] AuditExceptionViewModel request) {
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
                    return Ok(new { success = false, message = "Invalid audit exception data" });
                }


                var result = await _auditService.CreateAuditExceptionAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create audit exception" });

                var created = result.Data;
                return Ok(new { success = true, message = "Audit exception created successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving audit exception: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to create audit exception.Something went wrong" });
            }
        }

        [HttpPost]
        [LogActivityResult("Update audit exception", "User updated audit exception", ActivityTypeDefaults.COMPLIANCE_EDITED_EXCEPTION, "AuditException")]
        public async Task<IActionResult> UpdateException([FromBody] AuditExceptionViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                var result = await _auditService.UpdateAuditExceptionAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update audit exception" });

                var updated = result.Data;
                return Ok(new {
                    success = true,
                    message = "Audit exception exception successfully",
                    data = new { }
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating audit exception: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to updating audit exception.Something went wrong" });
            }
        }

        [HttpPost]
        [LogActivityResult("Delete Audit exception", "User delete audit exception", ActivityTypeDefaults.COMPLIANCE_DELETED_EXCEPTION, "AuditException")]
        public async Task<IActionResult> DeleteException(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Audit exception Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.AUDIT_EXCEPTION_DELETE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _auditService.DeleteExceptionAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete audit exception" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating audit exception: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to updating audit exception.Something went wrong" });
            }
        }

        #endregion

        #region Audit Tasks

        [LogActivityResult("Retrieve Audit task", "User retrieved audit task", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_AUDIT_TASK, "AuditTask")]
        public async Task<IActionResult> GeExceptionTask(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Task Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _auditService.GetAuditTaskAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving audit task";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var report = new {
                    id = response.Id,
                    taskStatus = response.TaskStatus ?? string.Empty,
                    taskName = response.TaskName ?? string.Empty,
                    description = response.TaskDescription ?? string.Empty,
                    duedate = response.Duedate,
                    ownerId = response.OwnerId,
                    owner = response.Owner ?? string.Empty,
                    isDeleted = response.IsDeleted
                };

                return Ok(new { success = true, data = report });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving audit task: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to retrieve audit task.Something went wrong" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetExceptionTasks([FromBody] GrcExceptionTaskViewModel request) {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"AUDIT TASK DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
               
                //..map to ajax object
                var tasksData = await _auditService.GetExceptionTasksAsync(request, currentUser.UserId, ipAddress);
                PagedResponse<GrcAuditTaskResponse> tasksList = new();

                if (tasksData.HasError) {
                    Logger.LogActivity($"AUDIT TASKS DATA ERROR: Failed to retrieve audit tasks - {JsonSerializer.Serialize(tasksData)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                } else {
                    tasksList = tasksData.Data;
                    Logger.LogActivity($"AUDIT TASKS DATA - {JsonSerializer.Serialize(tasksList)}");
                }

                var pagedEntities = tasksList.Entities
                    .Select(task => new {
                        id = task.Id,
                        taskStatus = task.TaskStatus ?? string.Empty,
                        taskName = task.TaskName ?? string.Empty,
                        description = task.TaskDescription ?? string.Empty,
                        duedate = task.Duedate,
                        ownerId = task.OwnerId,
                        owner = task.Owner ?? string.Empty,
                        isDeleted = task.IsDeleted
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)tasksList.TotalCount / tasksList.Size);
                return Ok(new {
                    last_page = totalPages,
                    total_records = tasksList.TotalCount,
                    data = pagedEntities
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving audit tasks: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Add Audit task", "User added audit task", ActivityTypeDefaults.COMPLIANCE_CREATE_AUDIT_TASK, "AuditTask")]
        public async Task<IActionResult> CreateExecptionTask([FromBody] GrcAuditTaskViewModel request) {
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
                    return Ok(new { success = false, message = "Invalid audit tasks data" });
                }


                var result = await _auditService.CreateExceptionTaskAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create audit tasks" });

                var created = result.Data;
                return Ok(new { success = true, message = "Audit task created successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving audit task: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to create audit task.Something went wrong" });
            }
        }

        [HttpPost]
        [LogActivityResult("Update audit task", "User updated audit task", ActivityTypeDefaults.COMPLIANCE_EDITED_AUDIT_TASK, "AuditTask")]
        public async Task<IActionResult> UpdateAuditTask([FromBody] GrcAuditTaskViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                var result = await _auditService.UpdateExceptionTaskAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update audit task" });

                var updated = result.Data;
                return Ok(new {
                    success = true,
                    message = "Audit task updated successfully",
                    data = new { }
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating audit task: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to updating audit task.Something went wrong" });
            }
        }

        [HttpPost]
        [LogActivityResult("Delete Audit task", "User delete audit task", ActivityTypeDefaults.COMPLIANCE_DELETED_AUDIT_TASK, "AuditTask")]
        public async Task<IActionResult> DeleteAuditTask(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Audit task Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.AUDIT_TASK_DELETE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _auditService.DeleteExceptionTaskAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete audit task" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating audit task: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "AUDIT-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to updating audit task.Something went wrong" });
            }
        }

        #endregion

    }

}
