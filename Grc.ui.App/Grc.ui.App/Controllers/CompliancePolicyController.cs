using ClosedXML.Excel;
using Grc.ui.App.Defaults;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Extensions.Http;
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
using System.Security.Policy;
using System.Text.Json;

namespace Grc.ui.App.Controllers {

    public class CompliancePolicyController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly IPolicyService _policyService;
        private readonly IPolicyTaskService _policyTasksService;
        private readonly IDashboardFactory _dashboardFactory;
        public CompliancePolicyController(IApplicationLoggerFactory loggerFactory, 
            IEnvironmentProvider environment, IWebHelper webHelper, ILocalizationService localizationService, 
            IErrorService errorService, IAuthenticationService authService,
            IPolicyService policyService,
            IDashboardFactory dashboardFactory,
            IPolicyTaskService policyTasksService, IGrcErrorFactory errorFactory, SessionManager sessionManager) 
            : base(loggerFactory, environment, webHelper, localizationService, errorService, errorFactory, sessionManager) {

            Logger.Channel = $"POLICY-{DateTime.Now:yyyyMMddHHmmss}";
             _authService = authService;
            _policyService = policyService;
            _policyTasksService = policyTasksService;
            _dashboardFactory = dashboardFactory;
        }

        #region Policy statistics

        public async Task<IActionResult> PoliciesTotals() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    //..try getting current user logged in
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (grcResponse.HasError) {
                        //..log error to database
                        _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-POLICY-CONTROLLER", "Unable to process user information");
                        return Redirect(Url.Action("Login", "Application"));
                    }

                    //..redirect to dashboard
                    var model = await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data);
                    model.WelcomeMessage = $"{model.WelcomeMessage} >> Total Policies";
                    return View(model);
                } else {
                    return Redirect(Url.Action("Dashboard", "Application"));
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Policies Register view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        public async Task<IActionResult> PoliciesOnHold() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    //..try getting current user logged in
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (grcResponse.HasError) {
                        //..log error to database
                        _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-POLICY-CONTROLLER", "Unable to process user information");
                        return Redirect(Url.Action("Login", "Application"));
                    }

                    //..redirect to dashboard
                    var model = await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data);
                    model.WelcomeMessage = $"{model.WelcomeMessage} >> Policies On Hold";
                    return View(model);
                } else {
                    return Redirect(Url.Action("Dashboard", "Application"));
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Policies Register view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        public async Task<IActionResult> PoliciesNeedReview() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    //..try getting current user logged in
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (grcResponse.HasError) {
                        //..log error to database
                        _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-POLICY-CONTROLLER", "Unable to process user information");
                        return Redirect(Url.Action("Login", "Application"));
                    }

                    //..redirect to dashboard
                    var model = await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data);
                    model.WelcomeMessage = $"{model.WelcomeMessage} >> Policies Needing Review";
                    return View(model);
                } else {
                    return Redirect(Url.Action("Dashboard", "Application"));
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Policies Register view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        public async Task<IActionResult> PoliciesPendingBoard() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    //..try getting current user logged in
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (grcResponse.HasError) {
                        //..log error to database
                        _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-POLICY-CONTROLLER", "Unable to process user information");
                        return Redirect(Url.Action("Login", "Application"));
                    }

                    //..redirect to dashboard
                    var model = await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data);
                    model.WelcomeMessage = $"{model.WelcomeMessage} >> Policies Pending Board Approval";
                    return View(model);
                } else {
                    return Redirect(Url.Action("Dashboard", "Application"));
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Policies Register view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        public async Task<IActionResult> PoliciesPendingDepartment() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    //..try getting current user logged in
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (grcResponse.HasError) {
                        //..log error to database
                        _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-POLICY-CONTROLLER", "Unable to process user information");
                        return Redirect(Url.Action("Login", "Application"));
                    }

                    //..redirect to dashboard
                    var model = await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data);
                    model.WelcomeMessage = $"{model.WelcomeMessage} >> Policies Pending Department Approval";
                    return View(model);
                } else {
                    return Redirect(Url.Action("Dashboard", "Application"));
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Policies Register view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        public async Task<IActionResult> PoliciesUptodate() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    //..try getting current user logged in
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (grcResponse.HasError) {
                        //..log error to database
                        _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-POLICY-CONTROLLER", "Unable to process user information");
                        return Redirect(Url.Action("Login", "Application"));
                    }

                    //..redirect to dashboard
                    var model = await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data);
                    model.WelcomeMessage = $"{model.WelcomeMessage} >> Policies Uptodate";
                    return View(model);
                } else {
                    return Redirect(Url.Action("Dashboard", "Application"));
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Policies Register view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        public async Task<IActionResult> PoliciesStandard() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    //..try getting current user logged in
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (grcResponse.HasError) {
                        //..log error to database
                        _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-POLICY-CONTROLLER", "Unable to process user information");
                        return Redirect(Url.Action("Login", "Application"));
                    }

                    //..redirect to dashboard
                    var model = await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data);
                    model.WelcomeMessage = $"{model.WelcomeMessage} >> Standard Policies [Policies that don't change]";
                    return View(model);
                } else {
                    return Redirect(Url.Action("Dashboard", "Application"));
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Error loading Policies Register view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        #endregion

        #region Policy Registers

        public async Task<IActionResult> PoliciesRegisters() {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null)
                    {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel
                    {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        Statistics = new()
                    };

                    return View(userDashboard);
                }
                else
                {
                    return RedirectToAction("Login", "Application");
                }
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error loading Policies Register view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        [LogActivityResult("Retrieve Policy", "User retrieved policy", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_POLICY, "Policy")]
        public async Task<IActionResult> GetPolicy(long id)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0)
                {
                    return BadRequest(new { success = false, message = "Policy Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new()
                {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_GET_POLICY.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _policyService.GetPolicyDocumentAsync(request);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving policy";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var policyRecord = new
                {
                    id = response.Id,
                    documentName = response.DocumentName,
                    comments = response.Comments,
                    documentTypeId = response.DocumentTypeId,
                    ownerId = response.ResponsibilityId,
                    departmentId = response.DepartmentId,
                    isDeleted = response.IsDeleted,
                    lastReviewDate = response.LastRevisionDate,
                    nextReviewDate = response.NextRevisionDate,
                    frequencyId = response.FrequencyId,
                    documentStatus = response.Status ?? string.Empty,
                    isAligned = response.IsAligned,
                    isLocked = response.IsLocked,
                    isApproved = string.IsNullOrWhiteSpace(response.ApprovedBy) ? 2 : 1,
                    approvalDate = response.ApprovalDate,
                    approvedBy = response.ApprovedBy ?? string.Empty
                };

                return Ok(new { success = true, data = policyRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error retrieving policy: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Redirect(Url.Action("PoliciesRegisters", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [LogActivityResult("Add Policy", "User added policy", ActivityTypeDefaults.COMPLIANCE_CREATE_POLICY, "Policy")]
        public async Task<IActionResult> CreatePolicy([FromBody] PolicyViewModel request) {
            try
            {
                if (!ModelState.IsValid) {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    string combinedErrors = string.Join("; ", errors);
                    return Ok(new {
                        success = false,
                        message = $"Please correct these errors: {combinedErrors}",
                        data = (object)null
                    });
                }

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                if (request == null)
                {
                    return Ok(new { success = false, message = "Invalid Policy/Procedure data" });
                }


                var result = await _policyService.CreateDocumentAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create policy" });

                var created = result.Data;
                return Ok(new {
                    success = true,
                    message = "Policy created successfully",
                    data = new { }
                });

            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error creating policy: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Redirect(Url.Action("PoliciesRegisters", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [LogActivityResult("Update Policy", "User updated policy", ActivityTypeDefaults.COMPLIANCE_EDITED_POLICY, "Policy")]
        public async Task<IActionResult> UpdatePolicy([FromBody] PolicyViewModel request)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                var result = await _policyService.UpdateDocumentAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update policy" });

                var updated = result.Data;
                return Ok(new
                {
                    success = true,
                    message = "Policy updated successfully",
                    data = new { }
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error updating policy: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Redirect(Url.Action("PoliciesRegisters", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [LogActivityResult("Lock Policy", "User locked policy document", ActivityTypeDefaults.COMPLIANCE_LOCK_POLICY, "Policy")]
        public async Task<IActionResult> LockPolicy(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Policy Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_LOCK_POLCIY.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _policyService.LockPolicyAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to lock policy document" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error lock policy document: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Redirect(Url.Action("PoliciesRegisters", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [LogActivityResult("Delete Policy", "User delete policy", ActivityTypeDefaults.COMPLIANCE_DELETED_POLICY, "Policy")]
        public async Task<IActionResult> DeletePolicy(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Policy Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_DELETED_POLCIY.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _policyService.DeletePolicyAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete policy" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error deleting policy: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Redirect(Url.Action("PoliciesRegisters", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [LogActivityResult("Export Policy", "User exported policies to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_POLICY, "Policy")]
        public IActionResult ExcelExportPolicies([FromBody] List<PolicyDocumentResponse> data)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Policies");

            ws.Cell(1, 1).Value = "POLICY/PROCEDURE NAME";
            ws.Cell(1, 2).Value = "DOCUMENT TYPE";
            ws.Cell(1, 3).Value = "REVIEW STATUS";
            ws.Cell(1, 4).Value = "APPROVED BY";
            ws.Cell(1, 5).Value = "APPROVAL DATE";
            ws.Cell(1, 6).Value = "LAST REVISION";
            ws.Cell(1, 7).Value = "NEXT REVISION";
            ws.Cell(1, 8).Value = "DEPARTMENT/FUNCTION";
            ws.Cell(1, 9).Value = "POLICY OWNER";
            ws.Cell(1, 10).Value = "POLICY ALIGNED";
            ws.Cell(1, 11).Value = "COMMENTS";

            int row = 2;
            foreach (var p in data) {
                ws.Cell(row, 1).Value = p.DocumentName;
                ws.Cell(row, 2).Value = p.DocumentTypeName;
                ws.Cell(row, 3).Value = p.Status;
                ws.Cell(row, 4).Value = p.ApprovedBy;
                ws.Cell(row, 5).Value = p.ApprovalDate;
                ws.Cell(row, 6).Value = p.LastRevisionDate;
                ws.Cell(row, 7).Value = p.NextRevisionDate;
                ws.Cell(row, 8).Value = p.ResponsibilityName ?? string.Empty;
                ws.Cell(row, 9).Value = p.DepartmentName ?? string.Empty;
                ws.Cell(row, 10).Value = p.IsAligned ? "YES" : "NO";
                ws.Cell(row, 11).Value = p.Comments;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Policies.xlsx");
        }

        [HttpPost]
        [LogActivityResult("Export Policy", "User exported policies to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_POLICY, "Policy")]
        public async Task<IActionResult> ExcelExportAllPolicies() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new TableListRequest
            {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                PageIndex = 1,
                PageSize = int.MaxValue,
                Action = Activity.COMPLIANCE_EXPORT_POLICIES.GetDescription()
            };

            var result = await _policyService.GetPagedDocumentsAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = "Failed to retrieve policies" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Policies");

            ws.Cell(1, 1).Value = "POLICY/PROCEDURE NAME";
            ws.Cell(1, 2).Value = "DOCUMENT TYPE";
            ws.Cell(1, 3).Value = "REVIEW STATUS";
            ws.Cell(1, 4).Value = "APPROVED BY";
            ws.Cell(1, 5).Value = "APPROVAL DATE";
            ws.Cell(1, 6).Value = "LAST REVISION";
            ws.Cell(1, 7).Value = "NEXT REVISION";
            ws.Cell(1, 8).Value = "DEPARTMENT/FUNCTION";
            ws.Cell(1, 9).Value = "POLICY OWNER";
            ws.Cell(1, 10).Value = "POLICY ALIGNED";
            ws.Cell(1, 11).Value = "COMMENTS";

            int row = 2;
            foreach (var p in result.Data.Entities) {
                ws.Cell(row, 1).Value = p.DocumentName;
                ws.Cell(row, 2).Value = p.DocumentTypeName;
                ws.Cell(row, 3).Value = p.Status;
                ws.Cell(row, 4).Value = p.ApprovedBy;
                ws.Cell(row, 5).Value = p.ApprovalDate;
                ws.Cell(row, 6).Value = p.LastRevisionDate;
                ws.Cell(row, 7).Value = p.NextRevisionDate;
                ws.Cell(row, 8).Value = p.ResponsibilityName ?? string.Empty;
                ws.Cell(row, 9).Value = p.DepartmentName ?? string.Empty;
                ws.Cell(row, 10).Value = p.IsAligned ? "YES" : "NO";
                ws.Cell(row, 11).Value = p.Comments;

                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Policies.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPolicies()
        {
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    Logger.LogActivity($"POLICY LIST ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { success = false, message = "Unable to resolve current user" });
                }

                var currentUser = grcResponse.Data;
                GrcRequest request = new()
                {
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_RETRIEVE_POLICY.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                //..get list of all document types
                var doctypeData = await _policyService.GetDocumentListAsync(request);

                List<PolicyDocumentResponse> policies;
                if (doctypeData.HasError)
                {
                    policies = new();
                    Logger.LogActivity($"POLICY DATA ERROR: Failed to retrieve type items - {JsonSerializer.Serialize(doctypeData)}");
                }
                else
                {
                    policies = doctypeData.Data;
                    Logger.LogActivity($"POLICY DATA - {JsonSerializer.Serialize(policies)}");
                }

                //..get ajax data
                List<object> select2Data = new();
                if (policies.Any())
                {
                    select2Data = policies.Select(type => new {
                        id = type.Id,
                        documentName = type.DocumentName
                    }).Cast<object>().ToList();
                }

                return Json(new { results = select2Data });
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "POLICY-DATA", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        public async Task<IActionResult> AllPolicies([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("POLICY DATA ERROR: Failed to get user");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_RETRIEVE_POLICY.GetDescription();

                var result = await _policyService.GetPagedDocumentsAsync(request);
                PagedResponse<PolicyDocumentResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<PolicyDocumentResponse>())
                    .Select(p => new {
                        id = p.Id,
                        documentName = p.DocumentName,
                        comments = p.Comments,
                        documentTypeId = p.DocumentTypeId,
                        documentType = p.DocumentTypeName,
                        ownerId = p.ResponsibilityId,
                        documentOwner = p.ResponsibilityName,
                        departmentId = p.DepartmentId,
                        department = p.DepartmentName,
                        isDeleted = p.IsDeleted,
                        lastReview = p.LastRevisionDate,
                        nextReview = p.NextRevisionDate,
                        frequencyId = p.FrequencyId,
                        documentStatus = p.Status ?? string.Empty,
                        isAligned = p.IsAligned,
                        isLocked = p.IsLocked,
                        isApproved = !string.IsNullOrWhiteSpace(p.ApprovedBy) && p.ApprovedBy.ToUpper() != "NONE",
                        approvalDate = p.ApprovalDate,
                        approvedBy = p.ApprovedBy ?? string.Empty
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);
                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving policies: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

        #region Policy Tasks

        public async Task<IActionResult> PoliciesTasks() {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null)
                    {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel
                    {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        Statistics = new()
                    };

                    return View(userDashboard);
                }
                else
                {
                    return RedirectToAction("Login", "Application");
                }
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error loading Policy Tasks view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-TASKS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }

        }

        [LogActivityResult("Retrieve Task", "User retrieved task", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_TASK, "Task")]
        public async Task<IActionResult> GetTask(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                {
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
                    Action = Activity.COMPLIANCE_RETRIEVE_TASK.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _policyTasksService.GetPolicyTaskAsync(request);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving policy";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var policyRecord = new {
                    id = response.Id,
                    documentName = response.PolicyDocument,
                    taskDescription = response.TaskDescription,
                    taskStatus = response.TaskStatus,
                    assigneeName = response.AssigneeName,
                    assigneeEmail = response.AssigneeEmail,
                    assigneeContact = response.AssigneeContact,
                    assigneeDepartment = response.AssigneeDepartment,
                    assigneePosition = response.AssigneePosition,
                    assignedBy = response.AssignedBy,
                    assignDate = response.AssignDate,
                    dueDate = response.DueDate,
                    lastReminder = response.LastReminder,
                    reminderInterval = response.ReminderIntervalDays,
                    nextReminder = response.NextReminder,
                    reminderSent = response.LastReminderSent
                };

                return Ok(new { success = true, data = policyRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error retrieving task: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-TASK", ex.StackTrace);
                return Redirect(Url.Action("PoliciesRegisters", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [LogActivityResult("Add Task", "User added task", ActivityTypeDefaults.COMPLIANCE_CREATE_TASK, "Task")]
        public async Task<IActionResult> CreateTask([FromBody] PolicyTaskViewModel request) {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    string combinedErrors = string.Join("; ", errors);
                    return Ok(new
                    {
                        success = false,
                        message = $"Please correct these errors: {combinedErrors}",
                        data = (object)null
                    });
                }

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_CREATE_TASK.GetDescription();

                if (request == null)
                {
                    return Ok(new { success = false, message = "Invalid task data" });
                }


                var result = await _policyTasksService.CreatePolicyTaskAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create task" });

                var created = result.Data;
                return Ok(new
                {
                    success = true,
                    message = "Task created successfully",
                    data = new
                    {
                        id = created.Id,
                        documentName = created.PolicyDocument,
                        taskDescription = created.TaskDescription,
                        taskStatus = created.TaskStatus,
                        assigneeName = created.AssigneeName,
                        assigneeEmail = created.AssigneeEmail,
                        assigneeContact = created.AssigneeContact,
                        assigneeDepartment = created.AssigneeDepartment,
                        assigneePosition = created.AssigneePosition,
                        assignedBy = created.AssignedBy,
                        assignDate = created.AssignDate,
                        dueDate = created.DueDate,
                        lastReminder = created.LastReminder,
                        reminderInterval = created.ReminderIntervalDays,
                        nextReminder = created.NextReminder,
                        reminderSent = created.LastReminderSent
                    }
                });

            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error creating task: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-TASK", ex.StackTrace);
                return Redirect(Url.Action("PoliciesRegisters", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [LogActivityResult("Update Task", "User updated task", ActivityTypeDefaults.COMPLIANCE_EDITED_TASK, "Task")]
        public async Task<IActionResult> UpdateTask([FromBody] PolicyTaskViewModel request) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_EDITED_TASK.GetDescription();

                var result = await _policyTasksService.UpdatePolicyTaskAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update task" });

                var updated = result.Data;
                return Ok(new
                {
                    success = true,
                    message = "Task updated successfully",
                    data = new
                    {
                        id = updated.Id,
                        documentName = updated.PolicyDocument,
                        taskDescription = updated.TaskDescription,
                        taskStatus = updated.TaskStatus,
                        assigneeName = updated.AssigneeName,
                        assigneeEmail = updated.AssigneeEmail,
                        assigneeContact = updated.AssigneeContact,
                        assigneeDepartment = updated.AssigneeDepartment,
                        assigneePosition = updated.AssigneePosition,
                        assignedBy = updated.AssignedBy,
                        assignDate = updated.AssignDate,
                        dueDate = updated.DueDate,
                        lastReminder = updated.LastReminder,
                        reminderInterval = updated.ReminderIntervalDays,
                        nextReminder = updated.NextReminder,
                        reminderSent = updated.LastReminderSent
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error updating task: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-TASK", ex.StackTrace);
                return Redirect(Url.Action("PoliciesRegisters", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [LogActivityResult("Delete Task", "User delete task", ActivityTypeDefaults.COMPLIANCE_DELETED_TASK, "Task")]
        public async Task<IActionResult> DeleteTask(long id) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Task Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new()
                {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_DELETED_TASK.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _policyTasksService.DeletePolicyTaskAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete task" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error deleting task: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-TASK", ex.StackTrace);
                return Redirect(Url.Action("PoliciesRegisters", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [LogActivityResult("Export Task", "User exported tasks to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_TASK, "Task")]
        public IActionResult ExcelExportTasks([FromBody] List<PolicyTaskResponse> data) {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Tasks");

            ws.Cell(1, 1).Value = "POLICY/PROCEDURE NAME";
            ws.Cell(1, 2).Value = "TASK DESCRIPTION";
            ws.Cell(1, 3).Value = "ASSIGNED TO";
            ws.Cell(1, 4).Value = "DEPARTMENT";
            ws.Cell(1, 5).Value = "ASSIGNED ON";
            ws.Cell(1, 6).Value = "DEADLINE";
            ws.Cell(1, 7).Value = "TASK STATUS";
            ws.Cell(1, 8).Value = "COMMENTS";

            int row = 2;
            foreach (var p in data)
            {
                ws.Cell(row, 1).Value = p.PolicyDocument;
                ws.Cell(row, 2).Value = p.TaskDescription;
                ws.Cell(row, 3).Value = p.AssigneeName;
                ws.Cell(row, 4).Value = p.AssigneeDepartment;
                ws.Cell(row, 5).Value = p.AssignDate;
                ws.Cell(row, 6).Value = p.DueDate;
                ws.Cell(row, 7).Value = p.TaskStatus;
                ws.Cell(row, 8).Value = p.Comments;

                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Tasks.xlsx");
        }

        [HttpPost]
        [LogActivityResult("Export Task", "User exported tasks to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_TASK, "Task")]
        public async Task<IActionResult> ExcelExportAlTasks() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new TableListRequest
            {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                PageIndex = 1,
                PageSize = int.MaxValue,
                Action = Activity.COMPLIANCE_EXPORT_TASK.GetDescription()
            };

            var result = await _policyTasksService.GetAllPolicyTasks(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = "Failed to retrieve tasks" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Tasks");

            ws.Cell(1, 1).Value = "POLICY/PROCEDURE NAME";
            ws.Cell(1, 2).Value = "TASK DESCRIPTION";
            ws.Cell(1, 3).Value = "ASSIGNED TO";
            ws.Cell(1, 4).Value = "DEPARTMENT";
            ws.Cell(1, 5).Value = "ASSIGNED ON";
            ws.Cell(1, 6).Value = "DEADLINE";
            ws.Cell(1, 7).Value = "TASK STATUS";
            ws.Cell(1, 8).Value = "COMMENTS";

            int row = 2;
            foreach (var p in result.Data.Entities)
            {
                ws.Cell(row, 1).Value = p.PolicyDocument;
                ws.Cell(row, 2).Value = p.TaskDescription;
                ws.Cell(row, 3).Value = p.AssigneeName;
                ws.Cell(row, 4).Value = p.AssigneeDepartment;
                ws.Cell(row, 5).Value = p.AssignDate;
                ws.Cell(row, 6).Value = p.DueDate;
                ws.Cell(row, 7).Value = p.TaskStatus;
                ws.Cell(row, 8).Value = p.Comments;

                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Tasks.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks() {
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    Logger.LogActivity($"TASK LIST ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { success = false, message = "Unable to resolve current user" });
                }

                var currentUser = grcResponse.Data;
                GrcRequest request = new()
                {
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_RETRIEVE_TASK.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                //..get list of all tasks
                var taskData = await _policyTasksService.GetAllAsync(request);

                List<PolicyTaskResponse> tasks;
                if (taskData.HasError)
                {
                    tasks = new();
                    Logger.LogActivity($"TASK DATA ERROR: Failed to retrieve type items - {JsonSerializer.Serialize(taskData)}");
                }
                else
                {
                    tasks = taskData.Data;
                    Logger.LogActivity($"TASK DATA - {JsonSerializer.Serialize(tasks)}");
                }

                //..get ajax data
                List<object> select2Data = new();
                if (tasks.Any())
                {
                    select2Data = tasks.Select(task => new {
                        id = task.Id,
                        documentName = task.PolicyDocument,
                        taskDescription = task.TaskDescription,
                        taskStatus = task.TaskStatus,
                        assigneeName = task.AssigneeName,
                        assigneeEmail = task.AssigneeEmail,
                        assigneeContact = task.AssigneeContact,
                        assigneeDepartment = task.AssigneeDepartment,
                        assigneePosition = task.AssigneePosition,
                        assignedBy = task.AssignedBy,
                        assignDate = task.AssignDate,
                        dueDate = task.DueDate,
                        lastReminder = task.LastReminder,
                        reminderInterval = task.ReminderIntervalDays,
                        nextReminder = task.NextReminder,
                        reminderSent = task.LastReminderSent
                    }).Cast<object>().ToList();
                }

                return Json(new { results = select2Data });
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "TASK-DATA", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AllTasks([FromBody] TableListRequest request) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("TASK DATA ERROR: Failed to get user");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_RETRIEVE_TASK.GetDescription();

                var result = await _policyTasksService.GetAllPolicyTasks(request);
                PagedResponse<PolicyTaskResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<PolicyTaskResponse>())
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(task => new {
                        id = task.Id,
                        documentName = task.PolicyDocument,
                        taskDescription = task.TaskDescription,
                        taskStatus = task.TaskStatus,
                        assigneeName = task.AssigneeName,
                        assigneeEmail = task.AssigneeEmail,
                        assigneeContact = task.AssigneeContact,
                        assigneeDepartment = task.AssigneeDepartment,
                        assigneePosition = task.AssigneePosition,
                        assignedBy = task.AssignedBy,
                        assignDate = task.AssignDate,
                        dueDate = task.DueDate,
                        lastReminder = task.LastReminder,
                        reminderInterval = task.ReminderIntervalDays,
                        nextReminder = task.NextReminder,
                        reminderSent = task.LastReminderSent
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);

                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving policies: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

        #region Policy Documents
        public async Task<IActionResult> PoliciesDocuments()
        {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null)
                    {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel
                    {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        Statistics = new()
                    };

                    return View(userDashboard);
                }
                else
                {
                    return RedirectToAction("Login", "Application");
                }
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error loading Policy Documents view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-DOCUMENTS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }

        }
        #endregion

    }

}
