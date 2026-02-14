using ClosedXML.Excel;
using Grc.ui.App.Defaults;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
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
                    var model = await _dashboardFactory.PreparePolicyMinModelAsync(grcResponse.Data, "TOTALS");
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
                    var model = await _dashboardFactory.PreparePolicyMinModelAsync(grcResponse.Data, "ONHOLD");
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
                    var model = await _dashboardFactory.PreparePolicyMinModelAsync(grcResponse.Data, "NEEDREVIEW");
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
                    var model = await _dashboardFactory.PreparePolicyMinModelAsync(grcResponse.Data, "PENDINGBOARD");
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
                    var model = await _dashboardFactory.PreparePolicyMinModelAsync(grcResponse.Data, "PENDINGDEPARTMENT");
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
                    var model = await _dashboardFactory.PreparePolicyMinModelAsync(grcResponse.Data, "UPTODATE");
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
                    var model = await _dashboardFactory.PreparePolicyMinModelAsync(grcResponse.Data, "STANDARD");
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

        [PermissionAuthorization(false, "ManageRegulationAndGuides", "ViewRegulationAndGuides")]
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
        [PermissionAuthorization(false, "ManageRegulationAndGuides", "ViewRegulationAndGuides")]
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
                    documentName = response.DocumentName ?? string.Empty,
                    documentTypeId = response.DocumentTypeId,
                    documentTypeName = response.DocumentTypeName ?? string.Empty,
                    ownerId = response.ResponsibilityId,
                    responsibilityName = response.ResponsibilityName ?? string.Empty,
                    departmentId = response.DepartmentId,
                    departmentName = response.DepartmentName ?? string.Empty,
                    isDeleted = response.IsDeleted,
                    lastReviewDate = response.LastRevisionDate,
                    lastReview = response.LastRevisionDate.ToString("yyyy-MM-dd"),
                    nextReviewDate = response.NextRevisionDate.HasValue ? 
                    ((DateTime)response.NextRevisionDate).ToString("yyyy-MM-dd"):
                    string.Empty,
                    nextReview = response.NextRevisionDate,
                    frequencyId = response.FrequencyId,
                    frequencyName = response.FrequencyName ?? string.Empty,
                    documentStatus = response.Status ?? string.Empty,
                    sendNotification = response.SendNotification,
                    interval = response.Interval ?? string.Empty,
                    intervalType = response.IntervalType ?? string.Empty,
                    sentMessages = response.SentMessages,
                    nextSendAt = response.NextSendAt ?? string.Empty,
                    reminderMessage = response.ReminderMessage ?? string.Empty,
                    comments = response.Comments ?? string.Empty,
                    isAligned = response.IsAligned,
                    isLocked = response.IsLocked,
                    mcrApproval = response.NeedMcrApproval,
                    boardApproval = response.NeedBoardApproval,
                    onIntranet = response.OnIntranet,
                    isApproved = string.IsNullOrWhiteSpace(response.ApprovedBy) ? 2 : 1,
                    approvalDate = response.ApprovalDate,
                    approver = response.ApprovedBy ?? string.Empty
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
        [PermissionAuthorization(true, "CreateRegulationAndGuides")]
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
        [PermissionAuthorization(true, "EditRegulationAndGuides")]
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
        [LogActivityResult("Lock Policy", "User locked policy", ActivityTypeDefaults.COMPLIANCE_LOCK_POLICY, "Policy")]
        [PermissionAuthorization(true, "CANLOCKPOLICYDOCUMENT")]
        public async Task<IActionResult> LockPolicy([FromBody] PolicyLockViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                var result = await _policyService.LockDocumentAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to lock/unlock policy" });

                var updated = result.Data;
                return Ok(new {
                    success = true,
                    message = "Policy updated successfully",
                    data = new { }
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating policy: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Redirect(Url.Action("PoliciesRegisters", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [LogActivityResult("Delete Policy", "User delete policy", ActivityTypeDefaults.COMPLIANCE_DELETED_POLICY, "Policy")]
        [PermissionAuthorization(true, "DeleteRegulationAndGuides")]
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
        [PermissionAuthorization(true, "CANVIEWCOMPLIANCEREPORTS", "CANCREATECOMPLIANCEREPORTS")]
        public IActionResult ExcelExportPolicies([FromBody] List<PolicyDocumentResponse> data) {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Policies");

            //..define headers
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

            //..header styling
            var header = ws.Range(1, 1, 1, 11);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            //..worksheet data
            int row = 2;
            foreach (var p in data) {
                ws.Cell(row, 1).Value = p.DocumentName;
                ws.Cell(row, 2).Value = p.DocumentTypeName;
                ws.Cell(row, 3).Value = p.Status;
                ws.Cell(row, 4).Value = p.ApprovedBy;

                //..approval Date
                SetSafeDate(ws.Cell(row, 5), p.ApprovalDate);

                //..last Revision Date
                ws.Cell(row, 6).Value = p.LastRevisionDate;
                ws.Cell(row, 6).Style.DateFormat.Format = "dd-MMM-yyyy";

                //..next Revision Date
                SetSafeDate(ws.Cell(row, 7), p.NextRevisionDate);

                ws.Cell(row, 8).Value = p.ResponsibilityName ?? string.Empty;
                ws.Cell(row, 9).Value = p.DepartmentName ?? string.Empty;
                ws.Cell(row, 10).Value = p.IsAligned ? "YES" : "NO";
                ws.Cell(row, 11).Value = p.Comments ?? string.Empty;

                row++;
            }

            //..status formating
            var statusRange = ws.Range(2, 3, row - 1, 3);
            statusRange.AddConditionalFormat().WhenEquals("DUE").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("PENDING-BOARD").Fill.SetBackgroundColor(XLColor.Orange);
            statusRange.AddConditionalFormat().WhenEquals("PENDING-SM").Fill.SetBackgroundColor(XLColor.Orange);
            statusRange.AddConditionalFormat() .WhenEquals("DPT-REVIEW").Fill.SetBackgroundColor(XLColor.LightGreen);
            statusRange.AddConditionalFormat().WhenEquals("UPTODATE").Fill.SetBackgroundColor(XLColor.Green);

            // Enable filters
            ws.Range(1, 1, 1, 11).SetAutoFilter();

            //..freeze header row
            ws.SheetView.FreezeRows(1);

            //..wrap COMMENTS column
            ws.Column(11).Style.Alignment.WrapText = true;
            ws.Column(11).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            //..auto-size columns based on content
            ws.Columns().AdjustToContents();

            //..cap COMMENTS width AFTER auto-fit
            ws.Column(11).Width = 40;


            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Policies.xlsx");
        }

        [HttpPost]
        [LogActivityResult("Export Policy", "User exported policies to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_POLICY, "Policy")]
        [PermissionAuthorization(true, "CANVIEWCOMPLIANCEREPORTS", "CANCREATECOMPLIANCEREPORTS")]
        public async Task<IActionResult> ExportAll() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcReportRequest
            {
                UserId = userResponse.Data.UserId,
                IpAddress = ipAddress,
                Filter = "ALL",
                Action = Activity.COMPLIANCE_EXPORT_POLICIES.GetDescription()
            };

            var result = await _policyService.GetPolicyReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Policies");

            //..define headers
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

            //..header styling
            var header = ws.Range(1, 1, 1, 11);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            //..resize header
            ws.Row(1).Height = 30;
            var countHeaderCell = ws.Cell(1, 3);

            //..worksheet data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.DocumentName;
                ws.Cell(row, 2).Value = p.DocumentTypeName;
                ws.Cell(row, 3).Value = p.Status;
                ws.Cell(row, 4).Value = p.ApprovedBy;

                //..approval Date
                SetSafeDate(ws.Cell(row, 5), p.ApprovalDate);

                //..last Revision Date
                ws.Cell(row, 6).Value = p.LastRevisionDate;
                ws.Cell(row, 6).Style.DateFormat.Format = "dd-MMM-yyyy";

                //..next Revision Date
                SetSafeDate(ws.Cell(row, 7), p.NextRevisionDate);

                ws.Cell(row, 8).Value = p.ResponsibilityName ?? string.Empty;
                ws.Cell(row, 9).Value = p.DepartmentName ?? string.Empty;
                ws.Cell(row, 10).Value = p.IsAligned ? "YES" : "NO";
                ws.Cell(row, 11).Value = p.Comments ?? string.Empty;

                row++;
            }

            //..status formating 
            var statusRange = ws.Range(2, 3, row - 1, 3);
            statusRange.AddConditionalFormat().WhenEquals("DUE").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("PENDING-BOARD").Fill.SetBackgroundColor(XLColor.Orange); 
            statusRange.AddConditionalFormat().WhenEquals("PENDING-SM").Fill.SetBackgroundColor(XLColor.Orange);
            statusRange.AddConditionalFormat() .WhenEquals("DEPT-REVIEW").Fill.SetBackgroundColor(XLColor.LightGreen);
            statusRange.AddConditionalFormat().WhenEquals("UPTODATE").Fill.SetBackgroundColor(XLColor.Green);
            statusRange.AddConditionalFormat().WhenEquals("ON-HOLD").Fill.SetBackgroundColor(XLColor.Amber);

            var aligRange = ws.Range(2, 10, row - 1, 10);
            aligRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Red);
            aligRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Green);

            // Enable filters
            ws.Range(1, 1, 1, 11).SetAutoFilter();

            //..freeze header row
            ws.SheetView.FreezeRows(1);

            //..wrap COMMENTS column
            ws.Column(11).Style.Alignment.WrapText = true;
            ws.Column(11).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            //..auto-size columns based on content
            ws.Columns().AdjustToContents();

            //..cap COMMENTS width AFTER auto-fit
            ws.Column(11).Width = 40;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","All_Policies.xlsx");
        }

        [HttpPost]
        [PermissionAuthorization(true, "CANVIEWCOMPLIANCEREPORTS", "CANCREATECOMPLIANCEREPORTS")]
        public async Task<IActionResult> ExportAllSummery() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcReportRequest {
                UserId = userResponse.Data.UserId,
                IpAddress = ipAddress,
                Filter = "ALLSUMMERY",
                Action = Activity.COMPLIANCE_EXPORT_POLICIES.GetDescription()
            };

            var result = await _policyService.GetPolicySummeryAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Summery");

            var data = result.Data;

            var summaryRows = new[] {
                "Uptodate",
                "Pending SMT signoff",
                "Under departmental review",
                "Pending Board signoff",
                "To be presented to MRC",
                "Due for review",
                "Not Applicable",
                "On Hold"
            };
            //..define headers
            ws.Cell(1, 1).Value = "NO";
            ws.Cell(1, 2).Value = "DETAILS";
            ws.Cell(1, 3).Value = "COUNT";
            ws.Cell(1, 4).Value = "PERCENTAGE";

            //..header styling
            var header = ws.Range(1, 1, 1, 4);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            //..increase header height
            ws.Row(1).Height = 30;
            var countHeaderCell = ws.Cell(1, 3);

            countHeaderCell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            countHeaderCell.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            int row = 2;
            int no = 1;

            int totalCount = 0;
            decimal totalPercentage = 0;

            foreach (var label in summaryRows) {

                var count = data.Count.TryGetValue(GetKey(label), out var c) ? c : 0;
                var percentage = data.Percentage.TryGetValue(GetKey(label), out var p) ? p : 0;
                ws.Cell(row, 1).Value = no;
                ws.Cell(row, 2).Value = label;
                ws.Cell(row, 3).Value = count;
                ws.Cell(row, 4).Value = percentage / 100; 
                ws.Cell(row, 4).Style.NumberFormat.Format = "0%";
                totalCount += count;
                totalPercentage += percentage;
                row++;
                no++;
            }

            //..add totals
            ws.Cell(row, 2).Value = "Total";
            ws.Cell(row, 3).Value = totalCount;
            ws.Cell(row, 4).Value = 1; 

            ws.Range(row, 2, row, 4).Style.Font.Bold = true;
            ws.Cell(row, 4).Style.NumberFormat.Format = "0%";

            //..adjust details column
            ws.Columns().AdjustToContents();
            foreach (var col in ws.ColumnsUsed()) {
                if (col.Width > 40)
                    col.Width = 40;
                else col.Width = 40;
            }

            // Cap row height
            foreach (var rowUsed in ws.RowsUsed()) {
                if (rowUsed.Height > 20)
                    rowUsed.Height = 20;
                else rowUsed.Height = 20;
            }

            ws.SheetView.FreezeRows(1);

            //..exclude headers and totals
            var dataRange = ws.Range(2, 3, row - 2, 4);

            //..count column
            //ws.Range(2, 3, row - 2, 3).Style.Font.FontColor = XLColor.Gray;
            ws.Range(2, 3, row - 2, 3).Style.Fill.BackgroundColor = XLColor.Gray;

            //..percentagelight gray
            //ws.Range(2, 4, row - 2, 4).Style.Font.FontColor = XLColor.LightGray;
            ws.Range(2, 4, row - 2, 4).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range(2, 3, row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(row, 4).Style.NumberFormat.Format = "0%";

            //..total row styling
            var totalRow = ws.Range(row, 1, row, 4);
            totalRow.Style.Font.Bold = true;
            totalRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            //..header height
            ws.Row(1).Height = 30;   

            //..totals height
            int totalRowNumber = row;
            ws.Row(totalRowNumber).Height = 28;
            ws.Range(1, 3, totalRowNumber, 3).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range(1, 3, totalRowNumber, 3).Style.Border.RightBorder = XLBorderStyleValues.Thin;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 $"POLICIES-SUMMERY-{DateTime.Today:MM-yyyy}.xlsx");
        }
        
        [HttpPost]
        [PermissionAuthorization(true, "CANVIEWCOMPLIANCEREPORTS", "CANCREATECOMPLIANCEREPORTS")]
        public async Task<IActionResult> ExportReview() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcReportRequest
            {
                UserId = userResponse.Data.UserId,
                IpAddress = ipAddress,
                Filter = "REVIEW",
                Action = Activity.COMPLIANCE_EXPORT_POLICIES.GetDescription()
            };

            var result = await _policyService.GetPolicyReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Policies");

            //..resize header
            ws.Row(1).Height = 30;
            var countHeaderCell = ws.Cell(1, 3);

            //..define headers
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

            //..header styling
            var header = ws.Range(1, 1, 1, 11);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            //..worksheet data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.DocumentName;
                ws.Cell(row, 2).Value = p.DocumentTypeName;
                ws.Cell(row, 3).Value = p.Status;
                ws.Cell(row, 4).Value = p.ApprovedBy;

                //..approval Date
                SetSafeDate(ws.Cell(row, 5), p.ApprovalDate);

                //..last Revision Date
                ws.Cell(row, 6).Value = p.LastRevisionDate;
                ws.Cell(row, 6).Style.DateFormat.Format = "dd-MMM-yyyy";

                //..next Revision Date
                SetSafeDate(ws.Cell(row, 7), p.NextRevisionDate);

                ws.Cell(row, 8).Value = p.ResponsibilityName ?? string.Empty;
                ws.Cell(row, 9).Value = p.DepartmentName ?? string.Empty;
                ws.Cell(row, 10).Value = p.IsAligned ? "YES" : "NO";
                ws.Cell(row, 11).Value = p.Comments ?? string.Empty;

                row++;
            }

            //..status formating
            var statusRange = ws.Range(2, 3, row - 1, 3);
            statusRange.AddConditionalFormat().WhenEquals("DUE").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("PENDING-BOARD").Fill.SetBackgroundColor(XLColor.Orange);
            statusRange.AddConditionalFormat().WhenEquals("PENDING-SM").Fill.SetBackgroundColor(XLColor.Orange);
            statusRange.AddConditionalFormat() .WhenEquals("DPT-REVIEW").Fill.SetBackgroundColor(XLColor.LightGreen);
            statusRange.AddConditionalFormat().WhenEquals("UPTODATE").Fill.SetBackgroundColor(XLColor.Green);

            var aligRange = ws.Range(2, 10, row - 1, 10);
            aligRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Red);
            aligRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Green);

            // Enable filters
            ws.Range(1, 1, 1, 11).SetAutoFilter();

            //..freeze header row
            ws.SheetView.FreezeRows(1);

            //..wrap COMMENTS column
            ws.Column(11).Style.Alignment.WrapText = true;
            ws.Column(11).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            //..auto-size columns based on content
            ws.Columns().AdjustToContents();

            //..cap COMMENTS width AFTER auto-fit
            ws.Column(11).Width = 40;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 $"POLICIES-UNDER-REVIEW-{DateTime.Today:MM-yyyy}.xlsx");
        }
        
        [HttpPost]
        [PermissionAuthorization(true, "CANVIEWCOMPLIANCEREPORTS", "CANCREATECOMPLIANCEREPORTS")]
        public async Task<IActionResult> ExportUpdated() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcReportRequest
            {
                UserId = userResponse.Data.UserId,
                IpAddress = ipAddress,
                Filter = "UPTODATE",
                Action = Activity.COMPLIANCE_EXPORT_POLICIES.GetDescription()
            };

            var result = await _policyService.GetPolicyReportAsync(request);

            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Policies");

            //..define headers
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

            //..header styling
            var header = ws.Range(1, 1, 1, 11);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            //..resize header
            ws.Row(1).Height = 30;
            var countHeaderCell = ws.Cell(1, 3);

            //..worksheet data
            //..worksheet data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.DocumentName;
                ws.Cell(row, 2).Value = p.DocumentTypeName;
                ws.Cell(row, 3).Value = p.Status;
                ws.Cell(row, 4).Value = p.ApprovedBy;

                //..approval Date
                SetSafeDate(ws.Cell(row, 5), p.ApprovalDate);

                //..last Revision Date
                ws.Cell(row, 6).Value = p.LastRevisionDate;
                ws.Cell(row, 6).Style.DateFormat.Format = "dd-MMM-yyyy";

                //..next Revision Date
                SetSafeDate(ws.Cell(row, 7), p.NextRevisionDate);

                ws.Cell(row, 8).Value = p.ResponsibilityName ?? string.Empty;
                ws.Cell(row, 9).Value = p.DepartmentName ?? string.Empty;
                ws.Cell(row, 10).Value = p.IsAligned ? "YES" : "NO";
                ws.Cell(row, 11).Value = p.Comments ?? string.Empty;

                row++;
            }

            //..status formating
            var statusRange = ws.Range(2, 3, row - 1, 3);
            statusRange.AddConditionalFormat().WhenEquals("DUE").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("PENDING-BOARD").Fill.SetBackgroundColor(XLColor.Orange);
            statusRange.AddConditionalFormat().WhenEquals("PENDING-SM").Fill.SetBackgroundColor(XLColor.Orange);
            statusRange.AddConditionalFormat() .WhenEquals("DPT-REVIEW").Fill.SetBackgroundColor(XLColor.LightGreen);
            statusRange.AddConditionalFormat().WhenEquals("UPTODATE").Fill.SetBackgroundColor(XLColor.Green);

            var aligRange = ws.Range(2, 10, row - 1, 10);
            aligRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Red);
            aligRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Green);

            //..format data
            // Enable filters
            ws.Range(1, 1, 1, 11).SetAutoFilter();

            //..freeze header row
            ws.SheetView.FreezeRows(1);

            //..wrap COMMENTS column
            ws.Column(11).Style.Alignment.WrapText = true;
            ws.Column(11).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            //..auto-size columns based on content
            ws.Columns().AdjustToContents();

            //..cap COMMENTS width AFTER auto-fit
            ws.Column(11).Width = 40;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 $"POLICIES-UPTODATE-{DateTime.Today:MM-yyyy}.xlsx");
        }
        
        [HttpPost]
        [PermissionAuthorization(true, "CANVIEWCOMPLIANCEREPORTS", "CANCREATECOMPLIANCEREPORTS")]
        public async Task<IActionResult> ExportDue() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcReportRequest
            {
                UserId = userResponse.Data.UserId,
                IpAddress = ipAddress,
                Filter = "DUE",
                Action = Activity.COMPLIANCE_EXPORT_POLICIES.GetDescription()
            };

            var result = await _policyService.GetPolicyReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Policies");

            //..define headers
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

            //..header styling
            var header = ws.Range(1, 1, 1, 11);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            //..resize header
            ws.Row(1).Height = 30;
            var countHeaderCell = ws.Cell(1, 3);

            //..worksheet data
            //..worksheet data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.DocumentName;
                ws.Cell(row, 2).Value = p.DocumentTypeName;
                ws.Cell(row, 3).Value = p.Status;
                ws.Cell(row, 4).Value = p.ApprovedBy;

                //..approval Date
                SetSafeDate(ws.Cell(row, 5), p.ApprovalDate);

                //..last Revision Date
                ws.Cell(row, 6).Value = p.LastRevisionDate;
                ws.Cell(row, 6).Style.DateFormat.Format = "dd-MMM-yyyy";

                //..next Revision Date
                SetSafeDate(ws.Cell(row, 7), p.NextRevisionDate);

                ws.Cell(row, 8).Value = p.ResponsibilityName ?? string.Empty;
                ws.Cell(row, 9).Value = p.DepartmentName ?? string.Empty;
                ws.Cell(row, 10).Value = p.IsAligned ? "YES" : "NO";
                ws.Cell(row, 11).Value = p.Comments ?? string.Empty;

                row++;
            }

            //..status formating
            var statusRange = ws.Range(2, 3, row - 1, 3);
            statusRange.AddConditionalFormat().WhenEquals("DUE").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("PENDING-BOARD").Fill.SetBackgroundColor(XLColor.Orange);
            statusRange.AddConditionalFormat().WhenEquals("PENDING-SM").Fill.SetBackgroundColor(XLColor.Orange);
            statusRange.AddConditionalFormat() .WhenEquals("DPT-REVIEW").Fill.SetBackgroundColor(XLColor.LightGreen);
            statusRange.AddConditionalFormat().WhenEquals("UPTODATE").Fill.SetBackgroundColor(XLColor.Green);

            var aligRange = ws.Range(2, 10, row - 1, 10);
            aligRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Red);
            aligRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Green);

            //..format data
            // Enable filters
            ws.Range(1, 1, 1, 11).SetAutoFilter();

            //..freeze header row
            ws.SheetView.FreezeRows(1);

            //..wrap COMMENTS column
            ws.Column(11).Style.Alignment.WrapText = true;
            ws.Column(11).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            //..auto-size columns based on content
            ws.Columns().AdjustToContents();

            //..cap COMMENTS width AFTER auto-fit
            ws.Column(11).Width = 40;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 $"DUE-UPDATE-{DateTime.Today:MM-yyyy}.xlsx");
        }

        [HttpPost]
        [PermissionAuthorization(true, "CANVIEWCOMPLIANCEREPORTS", "CANCREATECOMPLIANCEREPORTS")]
        public async Task<IActionResult> ExportSmt() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcReportRequest
            {
                UserId = userResponse.Data.UserId,
                IpAddress = ipAddress,
                Filter = "SMT",
                Action = Activity.COMPLIANCE_EXPORT_POLICIES.GetDescription()
            };

            var result = await _policyService.GetPolicyReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Policies");

            //..define headers
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

            //..header styling
            var header = ws.Range(1, 1, 1, 11);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            //..resize header
            ws.Row(1).Height = 30;
            var countHeaderCell = ws.Cell(1, 3);

            //..worksheet data
            //..worksheet data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.DocumentName;
                ws.Cell(row, 2).Value = p.DocumentTypeName;
                ws.Cell(row, 3).Value = p.Status;
                ws.Cell(row, 4).Value = p.ApprovedBy;

                //..approval Date
                SetSafeDate(ws.Cell(row, 5), p.ApprovalDate);

                //..last Revision Date
                ws.Cell(row, 6).Value = p.LastRevisionDate;
                ws.Cell(row, 6).Style.DateFormat.Format = "dd-MMM-yyyy";

                //..next Revision Date
                SetSafeDate(ws.Cell(row, 7), p.NextRevisionDate);

                ws.Cell(row, 8).Value = p.ResponsibilityName ?? string.Empty;
                ws.Cell(row, 9).Value = p.DepartmentName ?? string.Empty;
                ws.Cell(row, 10).Value = p.IsAligned ? "YES" : "NO";
                ws.Cell(row, 11).Value = p.Comments ?? string.Empty;

                row++;
            }

            //..status formating
            var statusRange = ws.Range(2, 3, row - 1, 3);
            statusRange.AddConditionalFormat().WhenEquals("DUE").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("PENDING-BOARD").Fill.SetBackgroundColor(XLColor.Orange);
            statusRange.AddConditionalFormat().WhenEquals("PENDING-SM").Fill.SetBackgroundColor(XLColor.Orange);
            statusRange.AddConditionalFormat() .WhenEquals("DPT-REVIEW").Fill.SetBackgroundColor(XLColor.LightGreen);
            statusRange.AddConditionalFormat().WhenEquals("UPTODATE").Fill.SetBackgroundColor(XLColor.Green);

            var aligRange = ws.Range(2, 10, row - 1, 10);
            aligRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Red);
            aligRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Green);

            //..format data
            // Enable filters
            ws.Range(1, 1, 1, 11).SetAutoFilter();

            //..freeze header row
            ws.SheetView.FreezeRows(1);

            //..wrap COMMENTS column
            ws.Column(11).Style.Alignment.WrapText = true;
            ws.Column(11).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            //..auto-size columns based on content
            ws.Columns().AdjustToContents();

            //..cap COMMENTS width AFTER auto-fit
            ws.Column(11).Width = 40;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 $"SMT-SUMMERY-{DateTime.Today:MM-yyyy}.xlsx");
        }
        
        [HttpPost]
        [PermissionAuthorization(true, "CANVIEWCOMPLIANCEREPORTS", "CANCREATECOMPLIANCEREPORTS")]
        public async Task<IActionResult> ExportSmtSummery() {
           var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcReportRequest
            {
                UserId = userResponse.Data.UserId,
                IpAddress = ipAddress,
                Filter = "SMTSUMMERY",
                Action = Activity.COMPLIANCE_EXPORT_POLICIES.GetDescription()
            };

            var result = await _policyService.GetSmtSummeryAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Summery");

            var data = result.Data;

            var summaryRows = new[] {
                "Uptodate",
                "Pending SMT signoff",
                "Under departmental review",
                "Pending Board signoff",
                "To be presented to MRC",
                "Due for review",
                "Not Applicable",
                "On Hold"
            };
            //..define headers
            ws.Cell(1, 1).Value = "NO";
            ws.Cell(1, 2).Value = "DETAILS";
            ws.Cell(1, 3).Value = "COUNT";
            ws.Cell(1, 4).Value = "PERCENTAGE";

            // Header styling (optional but recommended)
            var header = ws.Range(1, 1, 1, 4);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Increase header height
            ws.Row(1).Height = 30;
            var countHeaderCell = ws.Cell(1, 3);

            countHeaderCell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            countHeaderCell.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            int row = 2;
            int no = 1;

            int totalCount = 0;
            decimal totalPercentage = 0;

            foreach (var label in summaryRows) {

                var count = data.Count.TryGetValue(GetKey(label), out var c) ? c : 0;
                var percentage = data.Percentage.TryGetValue(GetKey(label), out var p) ? p : 0;
                ws.Cell(row, 1).Value = no;
                ws.Cell(row, 2).Value = label;
                ws.Cell(row, 3).Value = count;
                ws.Cell(row, 4).Value = percentage / 100;
                ws.Cell(row, 4).Style.NumberFormat.Format = "0%";
                totalCount += count;
                totalPercentage += percentage;
                row++;
                no++;
            }

            //..add totals
            ws.Cell(row, 2).Value = "Total";
            ws.Cell(row, 3).Value = totalCount;
            ws.Cell(row, 4).Value = 1;

            ws.Range(row, 2, row, 4).Style.Font.Bold = true;
            ws.Cell(row, 4).Style.NumberFormat.Format = "0%";

            //..adjust details column
            ws.Columns().AdjustToContents();
            foreach (var col in ws.ColumnsUsed()) {
                if (col.Width > 40)
                    col.Width = 40;
                else col.Width = 40;
            }

            // Cap row height (~100px ≈ 75 points)
            foreach (var rowUsed in ws.RowsUsed()) {
                if (rowUsed.Height > 20)
                    rowUsed.Height = 20;
                else rowUsed.Height = 20;
            }

            ws.SheetView.FreezeRows(1);

            //..exclude headers and totals
            var dataRange = ws.Range(2, 3, row - 2, 4);

            //..count column
            //ws.Range(2, 3, row - 2, 3).Style.Font.FontColor = XLColor.Gray;
            ws.Range(2, 3, row - 2, 3).Style.Fill.BackgroundColor = XLColor.Gray;

            //..percentagelight gray
            //ws.Range(2, 4, row - 2, 4).Style.Font.FontColor = XLColor.LightGray;
            ws.Range(2, 4, row - 2, 4).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range(2, 3, row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(row, 4).Style.NumberFormat.Format = "0%";

            //..total row styling
            var totalRow = ws.Range(row, 1, row, 4);
            totalRow.Style.Font.Bold = true;
            totalRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            //..header height
            ws.Row(1).Height = 30;

            //..totals height
            int totalRowNumber = row;
            ws.Row(totalRowNumber).Height = 28;
            ws.Range(1, 3, totalRowNumber, 3).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range(1, 3, totalRowNumber, 3).Style.Border.RightBorder = XLBorderStyleValues.Thin;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 $"SMT-SUMMERY-{DateTime.Today:MM-yyyy}.xlsx");
        }
        
        [HttpPost]
        [PermissionAuthorization(true, "CANVIEWCOMPLIANCEREPORTS", "CANCREATECOMPLIANCEREPORTS")]
        public async Task<IActionResult> ExportBod() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcReportRequest
            {
                UserId = userResponse.Data.UserId,
                IpAddress = ipAddress,
                Filter = "BOD",
                Action = Activity.COMPLIANCE_EXPORT_POLICIES.GetDescription()
            };

            var result = await _policyService.GetPolicyReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Policies");

            //..define headers
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

            //..header styling
            var header = ws.Range(1, 1, 1, 11);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            //..resize header
            ws.Row(1).Height = 30;
            var countHeaderCell = ws.Cell(1, 3);

            //..worksheet data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.DocumentName;
                ws.Cell(row, 2).Value = p.DocumentTypeName;
                ws.Cell(row, 3).Value = p.Status;
                ws.Cell(row, 4).Value = p.ApprovedBy;

                //..approval Date
                SetSafeDate(ws.Cell(row, 5), p.ApprovalDate);

                //..last Revision Date
                ws.Cell(row, 6).Value = p.LastRevisionDate;
                ws.Cell(row, 6).Style.DateFormat.Format = "dd-MMM-yyyy";

                //..next Revision Date
                SetSafeDate(ws.Cell(row, 7), p.NextRevisionDate);

                ws.Cell(row, 8).Value = p.ResponsibilityName ?? string.Empty;
                ws.Cell(row, 9).Value = p.DepartmentName ?? string.Empty;
                ws.Cell(row, 10).Value = p.IsAligned ? "YES" : "NO";
                ws.Cell(row, 11).Value = p.Comments ?? string.Empty;

                row++;
            }

            //..status formating
            var statusRange = ws.Range(2, 3, row - 1, 3);
            statusRange.AddConditionalFormat().WhenEquals("DUE").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("PENDING-BOARD").Fill.SetBackgroundColor(XLColor.Orange);
            statusRange.AddConditionalFormat().WhenEquals("PENDING-SM").Fill.SetBackgroundColor(XLColor.Orange);
            statusRange.AddConditionalFormat() .WhenEquals("DPT-REVIEW").Fill.SetBackgroundColor(XLColor.LightGreen);
            statusRange.AddConditionalFormat().WhenEquals("UPTODATE").Fill.SetBackgroundColor(XLColor.Green);

            var aligRange = ws.Range(2, 10, row - 1, 10);
            aligRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Red);
            aligRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Green);

            //..format data
            // Enable filters
            ws.Range(1, 1, 1, 11).SetAutoFilter();

            //..freeze header row
            ws.SheetView.FreezeRows(1);

            //..wrap COMMENTS column
            ws.Column(11).Style.Alignment.WrapText = true;
            ws.Column(11).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            //..auto-size columns based on content
            ws.Columns().AdjustToContents();

            //..cap COMMENTS width AFTER auto-fit
            ws.Column(11).Width = 40;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 $"BOD-REPORT-{DateTime.Today:MM-yyyy}.xlsx");
        }
        
        [HttpPost]
        [PermissionAuthorization(true, "CANVIEWCOMPLIANCEREPORTS", "CANCREATECOMPLIANCEREPORTS")]
        public async Task<IActionResult> ExportBodSummery() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcReportRequest {
                UserId = userResponse.Data.UserId,
                IpAddress = ipAddress,
                Filter = "BODSUMMERY",
                Action = Activity.COMPLIANCE_EXPORT_POLICIES.GetDescription()
            };

            var result = await _policyService.GetBodSummeryAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Summery");

            var data = result.Data;
            var summaryRows = new[] {
                "Uptodate",
                "Pending SMT signoff",
                "Under departmental review",
                "Pending Board signoff",
                "To be presented to MRC",
                "Due for review",
                "Not Applicable",
                "On Hold"
            };

            //..define headers
            ws.Cell(1, 1).Value = "NO";
            ws.Cell(1, 2).Value = "DETAILS";
            ws.Cell(1, 3).Value = "COUNT";
            ws.Cell(1, 4).Value = "PERCENTAGE";

            // Header styling (optional but recommended)
            var header = ws.Range(1, 1, 1, 4);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Increase header height
            ws.Row(1).Height = 30;
            var countHeaderCell = ws.Cell(1, 3);

            countHeaderCell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            countHeaderCell.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            int row = 2;
            int no = 1;

            int totalCount = 0;
            decimal totalPercentage = 0;

            foreach (var label in summaryRows) {

                var count = data.Count.TryGetValue(GetKey(label), out var c) ? c : 0;
                var percentage = data.Percentage.TryGetValue(GetKey(label), out var p) ? p : 0;
                ws.Cell(row, 1).Value = no;
                ws.Cell(row, 2).Value = label;
                ws.Cell(row, 3).Value = count;
                ws.Cell(row, 4).Value = percentage / 100;
                ws.Cell(row, 4).Style.NumberFormat.Format = "0%";
                totalCount += count;
                totalPercentage += percentage;
                row++;
                no++;
            }

            //..add totals
            ws.Cell(row, 2).Value = "Total";
            ws.Cell(row, 3).Value = totalCount;
            ws.Cell(row, 4).Value = 1;

            ws.Range(row, 2, row, 4).Style.Font.Bold = true;
            ws.Cell(row, 4).Style.NumberFormat.Format = "0%";

            //..adjust details column
            ws.Columns().AdjustToContents();
            foreach (var col in ws.ColumnsUsed()) {
                if (col.Width > 40)
                    col.Width = 40;
                else col.Width = 40;
            }

            // Cap row height (~100px ≈ 75 points)
            foreach (var rowUsed in ws.RowsUsed()) {
                if (rowUsed.Height > 20)
                    rowUsed.Height = 20;
                else rowUsed.Height = 20;
            }

            ws.SheetView.FreezeRows(1);

            //..exclude headers and totals
            var dataRange = ws.Range(2, 3, row - 2, 4);

            //..count column
            //ws.Range(2, 3, row - 2, 3).Style.Font.FontColor = XLColor.Gray;
            ws.Range(2, 3, row - 2, 3).Style.Fill.BackgroundColor = XLColor.Gray;

            //..percentagelight gray
            //ws.Range(2, 4, row - 2, 4).Style.Font.FontColor = XLColor.LightGray;
            ws.Range(2, 4, row - 2, 4).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range(2, 3, row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(row, 4).Style.NumberFormat.Format = "0%";

            //..total row styling
            var totalRow = ws.Range(row, 1, row, 4);
            totalRow.Style.Font.Bold = true;
            totalRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            //..header height
            ws.Row(1).Height = 30;

            //..totals height
            int totalRowNumber = row;
            ws.Row(totalRowNumber).Height = 28;
            ws.Range(1, 3, totalRowNumber, 3).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range(1, 3, totalRowNumber, 3).Style.Border.RightBorder = XLBorderStyleValues.Thin;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"BOD-SUMMERY-{DateTime.Today:MM-yyyy}.xlsx");
        }

        [HttpGet]
        [PermissionAuthorization(false, "ManageRegulationAndGuides", "ViewRegulationAndGuides")]
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

        [PermissionAuthorization(false, "ManageRegulationAndGuides", "ViewRegulationAndGuides")]
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
                        mcrApproval = p.NeedMcrApproval,
                        boardApproval = p.NeedBoardApproval,
                        onIntranet = p.OnIntranet,
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
        [PermissionAuthorization(false, "ManageRegulationAndGuides", "ViewRegulationAndGuides")]
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

        #region Private Methods
        private static void SetSafeDate(IXLCell cell, DateTime? date) {
            if (!date.HasValue) {
                cell.Value = string.Empty;
                return;
            }

            var value = date.Value;
            //..excel-safe date range
            if (value.Year < 1900 || value.Year > 9999) {
                cell.Value = string.Empty;
                return;
            }

            cell.Value = value;
            cell.Style.DateFormat.Format = "dd-MMM-yyyy";
        }

        private static string GetKey(string label) {
            return label switch {
                "On Hold" => "On Hold",
                "Under departmental review" => "Department Review",
                "Due for review" => "Not Uptodate",
                "Pending Board signoff" => "Board Review",
                "Uptodate" => "Uptodate",
                "To be presented to MRC" => "MRC Review",
                "Pending SMT signoff" => "SMT Review",
                _ => "Standard"
            };
        }
            #endregion
    }

}
