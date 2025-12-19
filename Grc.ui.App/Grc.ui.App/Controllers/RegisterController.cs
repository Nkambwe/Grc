using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
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
using System.Text.Json;

namespace Grc.ui.App.Controllers {
    public class RegisterController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly ISystemAccessService _accessService;
        private readonly IStatutorySectionService _sectionService;
        private readonly IRegulatoryStatuteService _statuteService;
        public RegisterController(IApplicationLoggerFactory loggerFactory,
                                  IEnvironmentProvider environment,
                                  IWebHelper webHelper,
                                  ILocalizationService localizationService,
                                  IErrorService errorService,
                                  IAuthenticationService authService,
                                  IStatutorySectionService regulatoryActService,
                                  IRegulatoryStatuteService statuteService,
                                  IGrcErrorFactory errorFactory,
                                  SessionManager sessionManager)
                                : base(loggerFactory, environment, webHelper,
                                      localizationService, errorService,
                                      errorFactory, sessionManager) {
            Logger.Channel = $"REGISTER-{DateTime.Now:yyyyMMddHHmmss}";
            _authService = authService;
            _sectionService = regulatoryActService;
            _statuteService = statuteService;
        }

        #region Acts Registers
        public async Task<IActionResult> RegulationList() {
            try {
                if (User.Identity?.IsAuthenticated == true)  {
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
                Logger.LogActivity($"Error loading Acts and legal Register view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "REGISTER-ACTS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        [LogActivityResult("Retrieve Act", "User retrieved legal act", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_ACTS, "RegulatoryAct")]
        public async Task<IActionResult> GetRegulatoryAct(long id) {
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
                    Action = Activity.SECTION_RETRIVE_SECTION.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _sectionService.GetSectionAsyncAsync(request);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving policy";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var policyRecord = new {
                    id = response.Id,
                    statutoryId = response.StatutoryId,
                    section = response.Section ?? string.Empty,
                    summery = response.Summery ?? string.Empty,
                    obligation = response.Obligation ?? string.Empty,
                    frequencyId = response.FrequencyId,
                    isMandatory = response.IsMandatory,
                    exclude = response.ExcludeFromCompliance,
                    coverage = response.Coverage,
                    isCovered = response.IsCovered,
                    isActive = response.IsDeleted,
                    comments = response.Comments ?? string.Empty,
                    assurance = response.ComplianceAssurance
                };

                return Ok(new { success = true, data = policyRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error retrieving regulatory: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = true, data = new { }});
            }
        }

        [HttpPost]
        [LogActivityResult("Add Act", "User added legal act", ActivityTypeDefaults.COMPLIANCE_CREATE_ACT, "RegulatoryAct")]
        public async Task<IActionResult> CreateRegulatoryAct([FromBody] StatuteSectionViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {success = false,message = msg,data = new { }});
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _sectionService.CreateSectionAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create statutory section" });

                var created = result.Data;
                return Ok(new{success = true,message = "Category created successfully",data = new { }});

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating regulatory act: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = true, data = new { } });
            }
        }

        [HttpPost]
        [LogActivityResult("Update Act", "User updated legal act", ActivityTypeDefaults.COMPLIANCE_EDITED_ACT, "RegulatoryAct")]
        public async Task<IActionResult> UpdateRegulatoryAct([FromBody] StatuteSectionViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                var result = await _sectionService.UpdateSectionAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update Statute section" });

                var updated = result.Data;
                return Ok(new { success = true, message = "Statute section created successfully", data = new { } });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating task: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, data = new { } });
            }
        }

        [HttpPost]
        [LogActivityResult("Delete Act", "User delete legal act", ActivityTypeDefaults.COMPLIANCE_DELETED_ACT, "RegulatoryAct")]
        public async Task<IActionResult> DeleteRegulatory(long id) {
            try {
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
                    Action = Activity.COMPLIANCE_DELETED_ACTS.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _sectionService.DeleteSectionAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete Statute section" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error deleting Statute section: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = true, data = new { } });
            }
        }

        [HttpPost]
        [LogActivityResult("Export Act", "User exported legal acts to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_ACT, "RegulatoryAct")]
        public IActionResult ExcelExportRegulatory([FromBody] List<PolicyTaskResponse> data)
        {
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
        [LogActivityResult("Export Act", "User exported legal act to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_ACT, "RegulatoryAct")]
        public async Task<IActionResult> ExcelExportAllRegulatory()
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
                Action = Activity.COMPLIANCE_EXPORT_TASK.GetDescription()
            };

            var result = new List<object>();//await _regulatoryActService.GetPagedRegulatoryActAsync(request);
            //if (result.HasError || result.Data == null)
            //    return Ok(new { success = false, message = "Failed to retrieve regulatory acts" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Laws_acts");

            ws.Cell(1, 1).Value = "REGULATORY ACT/LAW";
            ws.Cell(1, 2).Value = "AUTHORITY";
            ws.Cell(1, 3).Value = "REVIEW FREQ";
            ws.Cell(1, 4).Value = "IS ACTIVE";
            ws.Cell(1, 5).Value = "LAST REVIEW";
            ws.Cell(1, 6).Value = "RESPONSIBILITY";
            ws.Cell(1, 7).Value = "COMMENTS";

            int row = 2;
            foreach (var p in result)
            {
            //    ws.Cell(row, 1).Value = p.RegulatoryName;
            //    ws.Cell(row, 2).Value = p.RegulatoryAuthority;
            //    ws.Cell(row, 3).Value = p.ReviewFrequency;
            //    ws.Cell(row, 4).Value = p.IsDeleted == true ? "YES": "NO";
            //    ws.Cell(row, 5).Value = p.LastReviewDate.ToString("MM-dd-yyyy");
            //    ws.Cell(row, 6).Value = p.ReviewResponsibility;
            //    ws.Cell(row, 7).Value = p.Comments;

                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Law_acts.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegulatoryActs() {
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    Logger.LogActivity($"REGULATORY ACT LIST ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { success = false, message = "Unable to resolve current user" });
                }

                var currentUser = grcResponse.Data;
                GrcRequest request = new()
                {
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_RETRIEVE_ACTS.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                //..get list of all regulary act
                var taskData = new List<GrcStatutorySectionResponse>();

                List<GrcStatutorySectionResponse> acts = taskData;
                //if (taskData.HasError) {
                //    acts = new();
                //    Logger.LogActivity($"REGUALTORY ACT DATA ERROR: Failed to retrieve type items - {JsonSerializer.Serialize(taskData)}");
                //} else {
                //    acts = taskData.Data;
                //    Logger.LogActivity($"REGUALTORY ACT DATA - {JsonSerializer.Serialize(acts)}");
                //}

                //..get ajax data
                List<object> select2Data = new();
                if (acts.Any())
                {
                    //select2Data = acts.Select(a => new {
                    //    id = a.Id,
                    //    regulatoryName = a.RegulatoryName,
                    //    authorityId = a.AuthorityId,
                    //    regulatoryAuthority = a.RegulatoryAuthority,
                    //    reviewFrequency = a.ReviewFrequency,
                    //    isActive = a.IsDeleted,
                    //    lastReviewDate = a.LastReviewDate,
                    //    reviewResponsibility = a.ReviewResponsibility,
                    //    comments = a.Comments
                    //}).Cast<object>().ToList();
                }

                return Json(new { results = select2Data });
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetRegulatoryLaws([FromBody] StatueListRequest request) {

            try {

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) {
                    Logger.LogActivity($"STATUTE DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(userResponse)}");
                }

                var currentUser = userResponse.Data;
                var result = await _statuteService.GetCategoryStatutes(request, currentUser.UserId, ipAddress);
                PagedResponse<GrcStatutoryLawResponse> list = new();
                if (result.HasError) {
                    Logger.LogActivity($"STATUTE DATA ERROR: Failed to retrieve category statutes - {JsonSerializer.Serialize(result)}");
                } else {
                    list = result.Data;
                    Logger.LogActivity($"CATEGORY STATUTES DATA - {JsonSerializer.Serialize(list)}");
                }

                var statutes = list.Entities ?? new();
                if (statutes.Any()) {
                    var laws = statutes.Select(l => new {
                        id = l.Id,
                        lawName = l.StatutoryLawName,
                        lawCode = l.StatutoryLawCode,
                        status = l.IsDeleted ? "Inactive" : "Active"
                    }).ToList();
                    return Ok(new { last_page = 1, total_records = laws.Count, data = laws });
                }

                return Ok(new { last_page = 0, total_records = 0, data = Array.Empty<object>() });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving regulatory acts: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> GetRegulatoryActs([FromBody] StatueListRequest request) {
            try {

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) {
                    Logger.LogActivity($"STATUTE DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(userResponse)}");
                }

                var currentUser = userResponse.Data;
                var result = await _sectionService.GetLawSectionsAsync(request, currentUser.UserId, ipAddress);
                PagedResponse<GrcStatutorySectionResponse> list = new();
                if (result.HasError) {
                    Logger.LogActivity($"SECTION DATA ERROR: Failed to retrieve category statutes - {JsonSerializer.Serialize(result)}");
                } else {
                    list = result.Data;
                    Logger.LogActivity($"SECTION STATUTES DATA - {JsonSerializer.Serialize(list)}");
                }

                var statutes = list.Entities ?? new();
                if (statutes.Any()) {
                    var laws = statutes.Select(l => new {
                        id = l.Id,
                        sectionNumber = l.Section,
                        title = l.Summery,
                        assurance = l.ComplianceAssurance,
                        isMandatory = l.IsMandatory,
                    }).ToList();
                    return Ok(new { last_page = 1, total_records = laws.Count, data = laws });
                }

                return Ok(new { last_page = 0, total_records = 0, data = Array.Empty<object>() });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving regulatory acts: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-ACTS", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

        #region Obligations
        public async Task<IActionResult> RegulationObligations()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));

        }
        #endregion

        #region Regulatory Returns
        public async Task<IActionResult> RegulationReturns() {
            if (User.Identity?.IsAuthenticated == true) {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }
        #endregion

        #region Manage Regulations
        public async Task<IActionResult> ManageRegulations()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }
        #endregion

        #region Circular Obligations
        public async Task<IActionResult> RegulationCirculars() {
            if (User.Identity?.IsAuthenticated == true) {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }
        #endregion

        #region Regulatory Maps
        public IActionResult RegulationMaps() {
            if (User.Identity?.IsAuthenticated == true) {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }
        #endregion

    }
}
