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
using OpenXmlPowerTools;

namespace Grc.ui.App.Controllers {
    public class CompliancePolicyController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly IPolicyService _policyService;
        public CompliancePolicyController(IApplicationLoggerFactory loggerFactory, 
            IEnvironmentProvider environment, 
            IWebHelper webHelper, 
            ILocalizationService localizationService, 
            IErrorService errorService, 
            IAuthenticationService authService,
            IPolicyService policyService,
            IGrcErrorFactory errorFactory, 
            SessionManager sessionManager) 
            : base(loggerFactory, environment, webHelper, 
                  localizationService, errorService,
                  errorFactory, sessionManager) {

            Logger.Channel = $"POLICY-{DateTime.Now:yyyyMMddHHmmss}";
             _authService = authService;
            _policyService = policyService;
        }

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
                        DashboardStatistics = new()
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

        [LogActivityResult("Retrieve Policy", "User retrieved policy", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_POLICY, "Compliance")]
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
                GrcIdRequst request = new()
                {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_GET_POLICY.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _policyService.GetPolicyAsync(request);
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
                    policyName = response.DocumentName,
                    policyCode = response.DocumentStatus,
                    //owner = response.DocumentOwner,
                    lastRevisionDate = response.LastRevisionDate,
                    nextRevisionDate = response.NextRevisionDate,
                    status = response.IsDeleted ? "Inactive" : "Active",
                    //addedOn = response.CreatedAt.ToString("dd-MM-yyyy")
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
        [LogActivityResult("Add Policy", "User added policy", ActivityTypeDefaults.COMPLIANCE_CREATE_POLICY, "Compliance")]
        public async Task<IActionResult> CreatePolicy([FromBody] PolicyViewModel request)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_CREATE_POLICY.GetDescription();

                var result = await _policyService.CreatePolicyAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create policy" });

                var created = result.Data;
                return Ok(new
                {
                    success = true,
                    message = "Policy created successfully",
                    data = new
                    {
                        id = created.Id,
                        documentName = created.DocumentName,
                        documentType = created.DocumentType,
                        aligned = created.IsAligned,
                        locked = created.IsLocked,
                        reviewStatus = created.ReviewStatus,
                        lastReview = created.LastRevisionDate,
                        nextReview = created.NextRevisionDate,
                    }
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
        [LogActivityResult("Update Policy", "User updated policy", ActivityTypeDefaults.COMPLIANCE_EDITED_POLICY, "Compliance")]
        public async Task<IActionResult> UpdatePolicy([FromBody] PolicyViewModel request)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_EDITED_POLICY.GetDescription();

                var result = await _policyService.UpdatePolicyAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update policy" });

                var updated = result.Data;
                return Ok(new
                {
                    success = true,
                    message = "Policy updated successfully",
                    data = new
                    {
                        id = updated.Id,
                        documentName = updated.DocumentName,
                        documentType = updated.DocumentType,
                        aligned = updated.IsAligned,
                        locked = updated.IsLocked,
                        reviewStatus = updated.ReviewStatus,
                        lastReview = updated.LastRevisionDate,
                        nextReview = updated.NextRevisionDate,
                    }
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
        [LogActivityResult("Delete Policy", "User delete policy", ActivityTypeDefaults.COMPLIANCE_DELETED_POLICY, "Compliance")]
        public async Task<IActionResult> DeletePolicy(long id)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Policy Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequst request = new()
                {
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
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error deleting policy: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-REGISTER", ex.StackTrace);
                return Redirect(Url.Action("PoliciesRegisters", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [LogActivityResult("Export Policy", "User exported policies to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_POLICY, "Compliance")]
        public IActionResult ExcelExportPolicies([FromBody] List<PolicyRegisterResponse> data)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Policies");

            ws.Cell(1, 1).Value = "POLICY/PROCEDURE NAME";
            ws.Cell(1, 2).Value = "DOCUMENT TYPE";
            ws.Cell(1, 3).Value = "REVIEW STATUS";
            ws.Cell(1, 4).Value = "LAST REVISION";
            ws.Cell(1, 5).Value = "NEXT REVISION";
            ws.Cell(1, 6).Value = "ALIGNED";

            int row = 2;
            foreach (var p in data)
            {
                ws.Cell(row, 1).Value = p.DocumentName;
                ws.Cell(row, 2).Value = p.DocumentType;
                ws.Cell(row, 3).Value = p.ReviewStatus == "OVERDUE" ? "PASSED DUE" : (p.ReviewStatus == "DUE" ? "DUE" : "UPTODATE");
                ws.Cell(row, 4).Value = p.LastRevisionDate;
                ws.Cell(row, 5).Value = p.NextRevisionDate;
                ws.Cell(row, 6).Value = p.IsAligned ? "YES" : "NO";
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
        [LogActivityResult("Export Policy", "User exported policies to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_POLICY, "Compliance")]
        public async Task<IActionResult> ExcelExportAllPolicies()
        {
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

            var result = await _policyService.GetAllPolicies(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = "Failed to retrieve policies" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Policies");

            ws.Cell(1, 1).Value = "POLICY/PROCEDURE NAME";
            ws.Cell(1, 2).Value = "DOCUMENT TYPE";
            ws.Cell(1, 3).Value = "REVIEW STATUS";
            ws.Cell(1, 4).Value = "LAST REVISION";
            ws.Cell(1, 5).Value = "NEXT REVISION";
            ws.Cell(1, 6).Value = "ALIGNED";

            int row = 2;
            foreach (var p in result.Data.Entities)
            {
                ws.Cell(row, 1).Value = p.DocumentName;
                ws.Cell(row, 2).Value = p.DocumentType;
                ws.Cell(row, 3).Value = p.ReviewStatus == "OVERDUE" ? "PASSED DUE" : (p.ReviewStatus == "DUE" ? "DUE" : "UPTODATE");
                ws.Cell(row, 4).Value = p.LastRevisionDate;
                ws.Cell(row, 5).Value = p.NextRevisionDate;
                ws.Cell(row, 6).Value = p.IsAligned ? "YES" : "NO";
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Policies.xlsx");
        }
        
        public async Task<IActionResult> AllPolicies([FromBody] TableListRequest request)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("POLICY DATA ERROR: Failed to get user");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_RETRIEVE_POLICY.GetDescription();

                var result = await _policyService.GetAllPolicies(request);
                PagedResponse<PolicyRegisterResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<PolicyRegisterResponse>())
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(p => new
                    {
                        id = p.Id,
                        documentName = p.DocumentName,
                        documentType = p.DocumentType,
                        aligned = p.IsAligned,
                        locked = p.IsLocked,
                        reviewStatus = p.ReviewStatus,
                        lastReview = p.LastRevisionDate,
                        nextReview = p.NextRevisionDate,
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
                        DashboardStatistics = new()
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

        #endregion

        #region Policy Documnets
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
                        DashboardStatistics = new()
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
