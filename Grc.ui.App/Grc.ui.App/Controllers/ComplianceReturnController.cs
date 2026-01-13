using Grc.ui.App.Defaults;
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
using OpenXmlPowerTools;
using System.Text.Json;
using Activity = Grc.ui.App.Enums.Activity;

namespace Grc.ui.App.Controllers {
    public class ComplianceReturnController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly ISystemAccessService _accessService;
        private readonly IDashboardFactory _dashboardFactory;
        private readonly IReturnsService _returnService;
        public ComplianceReturnController(IApplicationLoggerFactory loggerFactory,
                                    IDashboardFactory dashboardFactory,
                                    IEnvironmentProvider environment,
                                    IWebHelper webHelper,
                                    ILocalizationService localizationService,
                                    IReturnsService returnService,
                                    IErrorService errorService,
                                    IAuthenticationService authService,
                                    IGrcErrorFactory errorFactory,
                                    SessionManager sessionManager)
                                    : base(loggerFactory, environment, webHelper,
                                          localizationService, errorService,
                                          errorFactory, sessionManager) {

            Logger.Channel = $"REGULATION-{DateTime.Now:yyyyMMddHHmmss}";
            _authService = authService;
            _dashboardFactory = dashboardFactory;
            _returnService = returnService;
        }

        #region Regulatory Returns

        public async Task<IActionResult> ReturnsHome() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnsDashboardModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> ReturnAnnualStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnPeriodDashboardModelAsync(grcResponse.Data, "ANNUAL"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ReturnDailyStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnPeriodDashboardModelAsync(grcResponse.Data, "DAILY"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ReturnWeeklyStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnPeriodDashboardModelAsync(grcResponse.Data, "WEEKLY"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ReturnMonthlyStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnPeriodDashboardModelAsync(grcResponse.Data, "MONTHLY"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ReturnQuaterlyStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnPeriodDashboardModelAsync(grcResponse.Data, "QUARTERLY"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ReturnBiennailStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard;
                return View(await _dashboardFactory.PrepareReturnPeriodDashboardModelAsync(grcResponse.Data, "BIENNIAL"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ReturnBiannaulStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnPeriodDashboardModelAsync(grcResponse.Data, "BIANNUAL"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ReturnOneOffStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnPeriodDashboardModelAsync(grcResponse.Data, "ONE-OFF"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ReturnOnOccurrenceStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnPeriodDashboardModelAsync(grcResponse.Data, "ON OCCURRENCE"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ReturnPeriodicStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnPeriodDashboardModelAsync(grcResponse.Data, "PERIODIC"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ReturnNaStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnPeriodDashboardModelAsync(grcResponse.Data, "NA"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ReturnTriennialStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnPeriodDashboardModelAsync(grcResponse.Data, "TRIENNIAL"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ReturnsRegister() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            return Redirect(Url.Action("Dashboard", "Application"));

        }

        [LogActivityResult("Retrieve Return", "User retrieved return/report", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_RETURN, "StatutoryReturn")]
        public async Task<IActionResult> GetReturn(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Return/Report Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_GET_RETURN.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _returnService.GetReturnAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving return/report";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var report = new {
                    id = response.Id,
                    sectionId = response.StatuteId,
                    returnName = response.ReturnName ?? string.Empty,
                    returnTypeId = response.TypeId,
                    authorityId = response.AuthorityId,
                    departmentId = response.DepartmentId,
                    isDeleted = response.IsDeleted,
                    frequencyId = response.FrequencyId,
                    documentTypeId = response.DepartmentId,
                    riskAttached = response.Risk ?? string.Empty,
                    comments = response.Comments ?? string.Empty
                };

                return Ok(new { success = true, data = report });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving return/report: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "RETURNS-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to retrieve return/report.Something went wrong" });
            }
        }

        [HttpPost]
        [LogActivityResult("Add Return", "User added return/repor", ActivityTypeDefaults.COMPLIANCE_CREATE_RETURN, "StatutoryReturn")]
        public async Task<IActionResult> CreateReturn([FromBody] StatutoryReturnViewModel request) {
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
                    return Ok(new { success = false, message = "Invalid Return/Report data" });
                }


                var result = await _returnService.CreateReturnAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create return/report" });

                var created = result.Data;
                return Ok(new { success = true, message = "Return/Report created successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving return/report: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "RETURNS-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to create return/report.Something went wrong" });
            }
        }

        [HttpPost]
        [LogActivityResult("Update Return", "User updated return/repor", ActivityTypeDefaults.COMPLIANCE_EDITED_RETURN, "StatutoryReturn")]
        public async Task<IActionResult> UpdateReturn([FromBody] StatutoryReturnViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                var result = await _returnService.UpdateReturnAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update return/report" });

                var updated = result.Data;
                return Ok(new {
                    success = true,
                    message = "Return/report updated successfully",
                    data = new { }
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating return/report: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "RETURNS-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to updating return/report.Something went wrong" });
            }
        }

        [HttpPost]
        [LogActivityResult("Delete Return", "User delete return/report", ActivityTypeDefaults.COMPLIANCE_DELETED_RETURN, "StatutoryReturn")]
        public async Task<IActionResult> DeleteReturn(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Return/Report Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_DELETED_POLCIY.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _returnService.DeleteReturnAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete return/report" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating return/report: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "RETURNS-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to updating return/report.Something went wrong" });
            }
        }

        [HttpPost]
        public IActionResult ExportReturnsFiltered([FromBody] List<GrcStatutoryReturnReportResponse> data) {
            return Ok(new { data = data });
        }

        [HttpPost]
        public async Task<IActionResult> ExportReturnsAll() {
            return Ok(new { message = "success" });
        }

        [HttpPost]
        public async Task<IActionResult> GetFrequencyReturns([FromBody] TableListRequest request) {
            try {

                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"FREQUENCY RETURNS DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.RETRIEVEREGULATORYCATEGORIES.GetDescription();

                //..get frequency data
                var categoryData = await _returnService.GetFrequencyReturnsAsync(request);
                PagedResponse<GrcFrequencyResponse> frequencyList = new();

                if (categoryData.HasError) {
                    Logger.LogActivity($"FREQUENCY RETURNS DATA ERROR: Failed to retrieve frquencu returns - {JsonSerializer.Serialize(categoryData)}");
                } else {
                    frequencyList = categoryData.Data;
                    Logger.LogActivity($"FREQUENCY RETURNS DATA - {JsonSerializer.Serialize(frequencyList)}");
                }

                //..map to ajax object
                var frequencies = frequencyList.Entities ??= new();
                if (frequencies.Any()) {
                    var tree = frequencies.Select(l => new {
                        id = $"C_{l.Id}",
                        text = l.FrequencyName,
                        type = "frequency",
                        children = l.Returns.Select(s => new { id = $"L_{s.Id}", text = s.Title, type = "report" }).ToArray()
                    }).ToArray();

                    return Ok(tree);
                }

                return Ok(Array.Empty<object>());
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving frequency returns items: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RETURNS-CONTROLLER", ex.StackTrace);
                return Ok(Array.Empty<object>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPagedReturnsList([FromBody] TableListRequest request) {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"RETURNS DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_RETURNS_RETRIEVE.GetDescription();

                //..map to ajax object
                var returnsData = await _returnService.GetReturnsListAsync(request);
                PagedResponse<GrcReturnsResponse> returnsList = new();

                if (returnsData.HasError) {
                    Logger.LogActivity($"RETURNS DATA ERROR: Failed to retrieve returns - {JsonSerializer.Serialize(returnsData)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                } else {
                    returnsList = returnsData.Data;
                    Logger.LogActivity($"RETURNS DATA - {JsonSerializer.Serialize(returnsList)}");
                }

                var pagedEntities = returnsList.Entities
                    .Select(report => new {
                        id = report.Id,
                        reportName = report.ReportName ?? string.Empty,
                        article = report.Article ?? string.Empty,
                        authority = report.Authority ?? string.Empty,
                        department = report.Department ?? string.Empty,
                        status = report.IsDeleted ? "INACTIVE" : "ACTIVE",
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)returnsList.TotalCount / returnsList.Size);
                return Ok(new {
                    last_page = totalPages,
                    total_records = returnsList.TotalCount,
                    data = pagedEntities
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving regulatory categories: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-SETTINGS-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPagedReturnSubmissions([FromBody] TableListRequest request) {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"RETURNS DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_RETURNS_RETRIEVE.GetDescription();

                //..map to ajax object
                var returnsData = await _returnService.GetReturnSubmissionsAsync(request);
                PagedResponse<GrcReturnSubmissionResponse> submissionList = new();

                if (returnsData.HasError) {
                    Logger.LogActivity($"RETURNS DATA ERROR: Failed to retrieve returns - {JsonSerializer.Serialize(returnsData)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                } else {
                    submissionList = returnsData.Data;
                    Logger.LogActivity($"RETURNS DATA - {JsonSerializer.Serialize(submissionList)}");
                }

                var pagedEntities = submissionList.Entities
                    .Select(submission => new {
                        id = submission.Id,
                        article = submission.Article ?? string.Empty,
                        reportName = submission.Report ?? string.Empty,
                        reportTitle = submission.Title ?? string.Empty,
                        periodStart = submission.PeriodStart,
                        periodEnd = submission.PeriodEnd,
                        status = submission.Status ?? string.Empty,
                        risk = submission.Risk ?? string.Empty,
                        isBreached = submission.IsBreached,
                        reason= submission.BreachReason,
                        submittedBy = submission.SubmittedBy ?? string.Empty,
                        submittedOn = submission.SubmittedOn,
                        departmentId = submission.DepartmentId,
                        department = submission.Department ?? string.Empty
                    }).OrderBy(s => s.status).ToList();

                var totalPages = (int)Math.Ceiling((double)submissionList.TotalCount / submissionList.Size);
                return Ok(new {
                    last_page = totalPages,
                    total_records = submissionList.TotalCount,
                    data = pagedEntities
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving return submissions: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-SETTINGS-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

        #region Returns Dashboards

        public async Task<IActionResult> NaInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("ReturnsHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> OneInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("ReturnsHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> OccurrenceInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("ReturnsHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> PeriodicInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("ReturnsHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> DailyInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("ReturnsHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> WeeklyInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                //return ;
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("ReturnsHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> MonthlyInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("ReturnsHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> QuarterlyInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("ReturnsHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> BiannualInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("ReturnsHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> AnnualInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("ReturnsHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> BiennialInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("ReturnsHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> TriennialInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("ReturnsHome", "ComplianceReturn"));
        }

        #endregion

        #region Return Submissions
        public async Task<IActionResult> GetSubmission(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Policy Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_GET_POLICY.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _returnService.GetSubmissionAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving policy";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var policyRecord = new {
                    id = response.Id,
                    report = response.Report ?? string.Empty,
                    title = response.Title ?? string.Empty,
                    period = $"{response.PeriodStart:yyyy-MM-dd} TO {response.PeriodEnd:yyyy-MM-dd}",
                    status = response.Status ?? string.Empty,
                    isDeleted = response.IsDeleted,
                    isBreached = response.IsBreached,
                    ownerId = response.DepartmentId,
                    riskAttached = response.Risk ?? string.Empty,
                    department = response.Department ?? string.Empty,
                    comments = response.Comment ?? string.Empty,
                    submittedBy = response.SubmittedBy,
                    reason = response.BreachReason ?? string.Empty
                };

                return Ok(new { success = true, data = policyRecord });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving submission: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "RETURNS-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to retrieving submission. An error occurred" });
            }
        }

        public async Task<IActionResult> UpdateSubmission([FromBody] SubmissionViewModel submission) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                var result = await _returnService.UpdateSubmissionAsync(submission, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update policy" });

                var updated = result.Data;
                return Ok(new { success = true, message = "Submission updated successfully", data = new { }});
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating submission: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "RETURNS-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to update submission. An error occurred" });
            }
        }

        #endregion

        #region Circular Submissions
        public async Task<IActionResult> GetCircular(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Policy Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_GET_POLICY.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _returnService.GetCircularSubmissionAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving policy";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var circularRecord = new {
                    id = response.Id,
                    reference = response.Reference ?? string.Empty,
                    circularTitle = response.CircularTitle ?? string.Empty,
                    circularRequirement = response.CircularRequirement ?? string.Empty,
                    recievedOn = $"{response.RecievedOn:yyyy-MM-dd}",
                    deadline = response.Deadline.HasValue? response.Deadline.Value.ToString("yyyy-MM-dd"): "N/A",
                    circularStatus = response.CircularStatus ?? string.Empty,
                    isDeleted = response.IsDeleted,
                    submittedOn = response.SubmittedOn,
                    isBreached = response.IsBreached,
                    submissionBreach = response.IsBreached ? "YES": "NO",
                    breachReason = response.BreachReason ?? string.Empty,
                    breachRisk = response.BreachRisk ?? string.Empty,
                    ownerId = response.OwnerId,
                    department = response.Department ?? string.Empty,
                    frequency = string.IsNullOrWhiteSpace(response.Frequency) ? "N/A": response.Frequency,
                    authority = response.Authority ?? string.Empty,
                    comments = response.Comments ?? string.Empty,
                    submittedBy = response.SubmittedBy,
                    filePath = response.FilePath ?? string.Empty,
                    issues = response.Issues != null && response.Issues.Count > 0
                        ? response.Issues.Select(issue => new {
                            id = issue.Id,
                            issueDescription = issue.IssueDescription,
                            resolution = issue.Resolution,
                            status = issue.Status,
                            resolvedOn = issue.ResolvedOn?.ToString("yyyy-MM-dd"),
                            receivedOn = issue.ReceivedOn.ToString("yyyy-MM-dd")
                        }).ToList()
                        : Enumerable.Empty<object>()
                };

                return Ok(new { success = true, data = circularRecord });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving submission: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "RETURNS-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to retrieving submission. An error occurred" });
            }
        }

        public async Task<IActionResult> UpdateCircular([FromBody] CircularSubmissionViewModel submission) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                var result = await _returnService.UpdateCircularSubmissionAsync(submission, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update policy" });

                var updated = result.Data;
                return Ok(new { success = true, message = "Submission updated successfully", data = new { } });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating submission: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "RETURNS-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to update submission. An error occurred" });
            }
        }

        #endregion

        #region Circular Returns

        public async Task<IActionResult> CircularHome() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularDashboardModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> CircularBouStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularAuthorityDashboardModelAsync(grcResponse.Data, "BOU"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> CircularFiaStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularAuthorityDashboardModelAsync(grcResponse.Data, "FIA"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> CircularPpdaStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularAuthorityDashboardModelAsync(grcResponse.Data, "PPDA"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> CircularMofedStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularAuthorityDashboardModelAsync(grcResponse.Data, "MoFED"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> CircularUraStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularAuthorityDashboardModelAsync(grcResponse.Data, "URA"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> CircularPdpoStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularAuthorityDashboardModelAsync(grcResponse.Data, "PDPO"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> CircularAgStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularAuthorityDashboardModelAsync(grcResponse.Data, "AG"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> CircularUibStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularAuthorityDashboardModelAsync(grcResponse.Data, "UIB"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> CircularNiraStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularAuthorityDashboardModelAsync(grcResponse.Data, "NIRA"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> CircularDpfStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularAuthorityDashboardModelAsync(grcResponse.Data, "DPF"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> CircularOtherStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularAuthorityDashboardModelAsync(grcResponse.Data, "OTHERS"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> CircularCmaStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularAuthorityDashboardModelAsync(grcResponse.Data, "CMA"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> CircularUmraStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularAuthorityDashboardModelAsync(grcResponse.Data, "UMRA"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> CircularUrbraStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularAuthorityDashboardModelAsync(grcResponse.Data, "URBRA"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> CircularIrauStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularAuthorityDashboardModelAsync(grcResponse.Data, "IRAU"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> CircularRegister() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> GetPagedCircularList([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("CIRCULAR DATA ERROR: Failed to get user");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_CIRCULAR_RETRIEVE.GetDescription();

                var result = await _returnService.GetPagedCircularAsync(request);
                PagedResponse<GrcCircularsResponse> circulars = result.Data ?? new();

                var pagedEntities = (circulars.Entities ?? new List<GrcCircularsResponse>())
                    .Select(circular => new {
                        id = circular.Id,
                        circularTitle = circular.CircularTitle ?? string.Empty,
                        authority = circular.Authority ?? string.Empty,
                        frequency = circular.Frequency ?? string.Empty,
                        department = circular.Department ?? string.Empty,
                        recievedOn = circular.RecievedOn,
                        status = circular.Status ?? string.Empty,
                        submissionDate = circular.SubmissionDate,
                        deadlineOn = circular.DeadlineOn,
                        hasIssues = circular.Issues.Count > 0,
                        file = circular.FilePath ?? string.Empty,
                        submittedBy = circular.SubmittedBy ?? string.Empty,
                        reference = circular.Reference ?? string.Empty,
                        comments = circular.Comments ?? string.Empty,
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)circulars.TotalCount / circulars.Size);
                return Ok(new { last_page = totalPages, total_records = circulars.TotalCount, data = pagedEntities });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving circulars: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RETURNS-REGISTER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

        #region Circular Dashboards

        public async Task<IActionResult> BouInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("CircularHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> CmaInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("CircularHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> UmraInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("CircularHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> IrauInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                //return View(await _dashboardFactory.PrepareCircularExtensionDashboardModelAsync(grcResponse.Data, ""));
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("CircularHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> FiaInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("CircularHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> PpdaInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("CircularHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> UrbraInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("CircularHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> MofedInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("CircularHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> UraInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                //return View(await _dashboardFactory.PrepareCircularExtensionDashboardModelAsync(grcResponse.Data, "URA"));
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("CircularHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> PdpoInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("CircularHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> AgInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("CircularHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> UibInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("CircularHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> NiraInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("CircularHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> DpfInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("CircularHome", "ComplianceReturn"));
        }

        public async Task<IActionResult> OtherInnerDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("CircularHome", "ComplianceReturn"));
        }

        #endregion

        #region Return Tasks

        public async Task<IActionResult> ComplianceTasksHome() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareTasksDashboardModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> TotalTaskStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareTasksDashboardModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> TaskOpenStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareMinTaskDashboardStatisticModelAsync(grcResponse.Data, "OPEN"));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> TaskClosedStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareMinTaskDashboardStatisticModelAsync(grcResponse.Data, "CLOSED"));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> FailedTaskStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareMinTaskDashboardStatisticModelAsync(grcResponse.Data, "BREACHED"));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> ComplianceTasksList() {
            if (User.Identity?.IsAuthenticated == true) {
                var userDashboard = new UserDashboardModel() {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }

        #endregion

    }
}
