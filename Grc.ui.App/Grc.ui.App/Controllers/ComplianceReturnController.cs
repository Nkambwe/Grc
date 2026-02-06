using ClosedXML.Excel;
using Grc.ui.App.Defaults;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
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
        private readonly IDashboardFactory _dashboardFactory;
        private readonly IReturnsService _returnService;
        private readonly IRegulatonCategoryService _regulatoryCategoryService;
        public ComplianceReturnController(IApplicationLoggerFactory loggerFactory,
                                    IDashboardFactory dashboardFactory,
                                    IEnvironmentProvider environment,
                                    IWebHelper webHelper,
                                    ILocalizationService localizationService,
                                    IReturnsService returnService,
                                    IErrorService errorService,
                                    IAuthenticationService authService,
                                    IRegulatonCategoryService regulatoryCategoryService,
                                    IGrcErrorFactory errorFactory,
                                    SessionManager sessionManager)
                                    : base(loggerFactory, environment, webHelper,
                                          localizationService, errorService,
                                          errorFactory, sessionManager) {

            Logger.Channel = $"REGULATION-{DateTime.Now:yyyyMMddHHmmss}";
            _authService = authService;
            _dashboardFactory = dashboardFactory;
            _returnService = returnService;
            _regulatoryCategoryService = regulatoryCategoryService;
        }


        [HttpPost]
        public async Task<IActionResult> AllRegulatoryCategories([FromBody] TableListRequest request) {
            try {

                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"REGULATORY CATEGORY DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.RETRIEVEREGULATORYCATEGORIES.GetDescription();

                //..get regulatory category data
                var categoryData = await _regulatoryCategoryService.GetPagedCategoriesAsync(request);
                PagedResponse<GrcRegulatoryCategoryResponse> categoryList = new();

                if (categoryData.HasError) {
                    Logger.LogActivity($"REGULATORY CATEGORY DATA ERROR: Failed to retrieve category items - {JsonSerializer.Serialize(categoryData)}");
                } else {
                    categoryList = categoryData.Data;
                    Logger.LogActivity($"REGULATORY CATEGORY DATA - {JsonSerializer.Serialize(categoryList)}");
                }

                //..map to ajax object
                var categories = categoryList.Entities ??= new();
                if (categories.Any()) {
                    var tree = categories.Select(l => new {
                        id = $"C_{l.Id}",
                        text = l.CategoryName,
                        type = "category",
                        children = l.Statutes.Select(s => new { id = $"L_{s.Id}", text = s.StatutoryLawName, type = "law" }).ToArray()
                    }).ToArray();

                    return Ok(tree);
                }

                return Ok(Array.Empty<object>());
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving regulatory category items: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-CATEGORY-CONTROLLER", ex.StackTrace);
                return Ok(Array.Empty<object>());
            }
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
                    statuteId = response.StatuteId,
                    returnName = response.ReturnName ?? string.Empty,
                    returnTypeId = response.TypeId,
                    returnType = response.Type ?? string.Empty,
                    authorityId = response.AuthorityId,
                    authority = response.Authority ?? string.Empty,
                    departmentId = response.DepartmentId,
                    department = response.Department ?? string.Empty,
                    frequencyId = response.FrequencyId,
                    frequency = response.Frequency ?? string.Empty,
                    riskAttached = response.Risk ?? string.Empty,
                    sendReminder = response.SendReminder,
                    interval = response.Interval ?? string.Empty,
                    intervalType = response.IntervalType ?? string.Empty,
                    reminderMessage = response.Reminder ?? string.Empty,
                    requiredSubmissionDate = response.RequiredSubmissionDate,
                    requiredSubmissionDay = response.RequiredSubmissionDay,
                    isDeleted = response.IsDeleted,
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
        [LogActivityResult("Add Return", "User added return/report", ActivityTypeDefaults.COMPLIANCE_CREATE_RETURN, "StatutoryReturn")]
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
                        type = report.Type ?? string.Empty,
                        reportName = report.ReportName ?? string.Empty,
                        article = report.Article ?? string.Empty,
                        authority = report.Authority ?? string.Empty,
                        department = report.Department ?? string.Empty,
                        sendReminder = report.SendReminder ? "YES" : "NO",
                        interval = report.Interval?? string.Empty,
                        intervalType = report.IntervalType ?? string.Empty,
                        reminder = report.Reminder ?? string.Empty,
                        submissionDate = report.RequiredSubmissionDate.HasValue ? report.RequiredSubmissionDate.Value.ToString("MMM dd"): string.Empty,
                        status = report.IsDeleted ? "INACTIVE" : "ACTIVE",
                        comments = report.Comments ?? string.Empty,
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
                        requiredDate = submission.RequiredSubmissionDate,
                        requiredDay = submission.RequiredSubmissionDay,
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
                    requiredDate = $"{response.RequiredSubmissionDate:yyyy-MM-dd}",
                    status = response.Status ?? string.Empty,
                    isDeleted = response.IsDeleted,
                    isBreached = response.IsBreached,
                    ownerId = response.DepartmentId,
                    department = response.Department ?? string.Empty,
                    riskAttached = response.Risk ?? string.Empty,
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
                    return BadRequest(new { success = false, message = "Circular Id is required", data = new { } });
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
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving circular";
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
                Logger.LogActivity($"Unexpected error retrieving circular: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "RETURNS-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to retrieving circular. An error occurred" });
            }
        }

        public async Task<IActionResult> GetCircularInfo(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Circular Id is required", data = new { } });
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
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving circular";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var circularRecord = new {
                    id = response.Id,
                    reference = response.Reference ?? string.Empty,
                    circularTitle = response.CircularTitle ?? string.Empty,
                    circularRequirement = response.CircularRequirement ?? string.Empty,
                    recievedOn = response.RecievedOn,
                    deadline = response.Deadline,
                    circularStatus = response.CircularStatus ?? string.Empty,
                    isDeleted = response.IsDeleted,
                    breachRisk = response.BreachRisk ?? string.Empty,
                    ownerId = response.OwnerId,
                    frequencyId = response.FrequencyId,
                    authorityId = response.AuthorityId,
                    comments = response.Comments ?? string.Empty,
                    issues = response.Issues != null && response.Issues.Count > 0
                        ? response.Issues.Select(issue => new {
                            id = issue.Id,
                            issueDescription = issue.IssueDescription,
                            resolution = issue.Resolution,
                            status = issue.Status,
                            resolvedOn = issue.ResolvedOn?.ToString("yyyy-MM-dd") ?? string.Empty,
                            receivedOn = issue.ReceivedOn.ToString("yyyy-MM-dd") ?? string.Empty
                        }).ToList()
                        : Enumerable.Empty<object>()
                };

                return Ok(new { success = true, data = circularRecord });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving circular: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "RETURNS-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to retrieving circular. An error occurred" });
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

        #endregion

        #region Circular

       public async Task<IActionResult> GetCircularRecord(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Circular Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_CIRCULAR_RETRIEVE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _returnService.GetCircularAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving circular";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var circular = result.Data;
                var circularRecord = new {
                    id = circular.Id,
                    reference = circular.Reference ?? string.Empty,
                    circularTitle = circular.CircularTitle ?? string.Empty,
                    circularRequirement = circular.Requirement ?? string.Empty,
                    ownerId = circular.DepartmentId,
                    owner = circular.Department,
                    authorityId = circular.AuthorityId,
                    authority = circular.Authority,
                    frequencyId = circular.FrequencyId,
                    recievedOn = circular.RecievedOn,
                    received = circular.RecievedOn.ToString("yyyy-MM-dd"),
                    sendReminder = circular.SendReminder,
                    requiredSubmissionDate = circular.RequiredSubmissionDate.HasValue ? ((DateTime)circular.RequiredSubmissionDate).ToString("yyyy-MM-dd") : "",
                    requiredSubmissionDay = circular.RequiredSubmissionDay,
                    reminder = circular.Reminder ?? string.Empty,
                    interval = circular.Interval ?? string.Empty,
                    intervalType = circular.IntervalType ?? string.Empty,
                    deadline = circular.DeadlineOn ?? DateTime.Now.AddYears(30),
                    lastDate = circular.DeadlineOn.HasValue ?
                    circular.RecievedOn.ToString("yyyy-MM-dd") :
                    "NO DEADLINE",
                    status = circular.Status ?? string.Empty,
                    breachRisk = circular.BreachRisk ?? string.Empty,
                    comments = circular.Comments ?? string.Empty,
                    isDeleted = circular.IsDeleted,
                    issueCount = circular.Issues == null ? 0 : circular.Issues.Count,
                    issues = circular.Issues != null && circular.Issues.Any()?
                             circular.Issues.Select( issue => new { 
                                 id = issue.Id,
                                 circularId = issue.CircularId,
                                 issueDescription = (issue.IssueDescription ?? string.Empty).Trim(),
                                 issueResolution = issue.Resolution ?? string.Empty,
                                 issueDeleted = issue.IsDeleted,
                                 issueStatus = issue.Status ?? "OPEN",
                                 issueRecieved = issue.ReceivedOn,
                                 issueResolved = issue.ResolvedOn ?? DateTime.Now.AddYears(30),
                             }).ToArray():
                             Array.Empty<object>()
                };

                return Ok(new { success = true, data = circularRecord });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving circular: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RETURN-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Failed to retrieve circular issue, an Unexpected error occurred", data = new { } });
            }
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

                var pagedEntities = (circulars.Entities ?? new())
                    .Select(circular => new {
                        id = circular.Id,
                        circularTitle = circular.CircularTitle ?? string.Empty,
                        requirement = circular.Requirement ?? string.Empty,
                        authority = circular.Authority ?? string.Empty,
                        frequency = circular.Frequency ?? string.Empty,
                        department = circular.Department ?? string.Empty,
                        recievedOn = circular.RecievedOn,
                        status = circular.Status ?? string.Empty,
                        submissionDate = circular.SubmissionDate,
                        sendReminder = circular.SendReminder,
                        requiredSubmissionDate = circular.RequiredSubmissionDate.HasValue ? ((DateTime)circular.RequiredSubmissionDate).ToString("yyyy-MM-dd") : "",
                        requiredSubmissionDay = circular.RequiredSubmissionDay,
                        reminder = circular.Reminder ?? string.Empty,
                        interval = circular.Interval ?? string.Empty,
                        intervalType = circular.IntervalType ?? string.Empty,
                        deadlineOn = circular.DeadlineOn,
                        hasIssues = circular.Issues.Count > 0,
                        file = circular.FilePath ?? string.Empty,
                        submittedBy = circular.SubmittedBy ?? string.Empty,
                        breachRisk = circular.BreachRisk ?? string.Empty,
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

        public async Task<IActionResult> CreateCircularRecord([FromBody] CircularViewModel request) {
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
                var result = await _returnService.CreateCircularRecordAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create circular" });

                var created = result.Data;
                if (!created.Status)
                    return Ok(new { success = created.Status, message = created.Message ?? "Failed to create circular", data = new { } });

                return Ok(new { success = true, message = "Circular created successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating circular: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RETURN-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Failed to create circular, an Unexpected error occurred", data = new { } });
            }
        }

        public async Task<IActionResult> UpdateCircularRecord([FromBody] CircularViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                var result = await _returnService.UpdateCircularAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update circular" });

                var updated = result.Data;
                return Ok(new { success = true, message = "Circular updated successfully", data = new { } });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating circular: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "RETURNS-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to update circular. An error occurred" });
            }
        }

        public async Task<IActionResult> DeleteCircularRecord(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Circular Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_ISSUE_DELETE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _returnService.DeleteCircularAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete circular" });

                var resultData = result.Data;
                if (!resultData.Status)
                    return Ok(new { success = resultData.Status, message = resultData.Message ?? "Failed to delete circular", data = new { } });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error deleting circular: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RETURNS-CONTROLLER", ex.StackTrace);
                return Ok(new { success = true, data = new { } });
            }
        }

        #endregion

        #region Circular Issues

        [LogActivityResult("Retrieve Circular Issue", "User retrieved circular issue", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_ISSUE, "CircularIssue")]
        public async Task<IActionResult> GetCircularIssue(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Issue Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_ISSUE_RETRIEVE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = false
                };

                var result = await _returnService.GetIssueAsyncAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving circular issue";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var actRecord = new {
                    id = response.Id,
                    circularId = response.CircularId,
                    issueDescription = (response.IssueDescription ?? string.Empty).Trim(),
                    issueResolution = response.Resolution ?? string.Empty,
                    issueDeleted = response.IsDeleted,
                    issueStatus = response.Status ?? "OPEN",
                    issueRecieved = response.ReceivedOn,
                    issueResolved = response.ResolvedOn
                };

                return Ok(new { success = true, data = actRecord });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving circular issuet: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Failed to retrieve circular issue, an Unexpected error occurred", data = new { } });
            }
        }

        [LogActivityResult("Retrieve list of circular Issue", "User retrieved list of circular issues", ActivityTypeDefaults.COMPLIANCE_RETRIEVE_ISSUE, "CircularIssue")]
        public async Task<IActionResult> GetAllCircularIssues(GrcCircularIssueListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (request == null) {
                    return BadRequest(new { success = false, message = "Issue Id is required", data = new { } });
                }

                var result = await _returnService.GetCircularIssuesAsyncAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving circular issues";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var issues = response
                    .Select(issue => new {
                        circularId = issue.CircularId,
                        issueDescription = (issue.IssueDescription ?? string.Empty).Trim(),
                        issueResolution = issue.Resolution ?? string.Empty,
                        issueDeleted = issue.IsDeleted,
                        issueStatus = issue.Status ?? "OPEN",
                        issueRecieved = issue.ReceivedOn,
                        issueResolved = issue.ResolvedOn
                    }).OrderBy(s => s.issueStatus).ToList();

                return Ok(new { success = true, message = "Success", data = issues});
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving circular issues: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RIGISTER-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Failed to retrieve circular issues, an Unexpected error occurred", data = new { } });
            }
        }

        [LogActivityResult("Create Circular Issue", "User added new circular issue", ActivityTypeDefaults.COMPLIANCE_CREATE_ISSUE, "CircularIssue")]
        public async Task<IActionResult> CreateCircularIssue([FromBody] CircularIssueViewModel request) {
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
                var result = await _returnService.CreateIssueAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create issue" });

                var created = result.Data;
                if (!created.Status)
                    return Ok(new { success = created.Status, message = created.Message ?? "Failed to update issue", data = new { } });

                return Ok(new { success = true, message = "Issue created successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating issue: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RETURN-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Failed to create issue, an Unexpected error occurred", data = new { } });
            }
        }

        [LogActivityResult("Update Circular issue", "User updated circular issue", ActivityTypeDefaults.COMPLIANCE_EDITED_ISSUE, "CircularIssue")]
        public async Task<IActionResult> UpdateCircularIssue([FromBody] CircularIssueViewModel request) {
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
                var result = await _returnService.UpdateIssueAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update issue" });

                var updated = result.Data;
                if (!updated.Status)
                    return Ok(new { success = updated.Status, message = updated.Message ?? "Failed to update issue", data = new { } });

                return Ok(new { success = true, message = "Issue created successfully", data = new { } });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating issue: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RETURN-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, data = new { } });
            }
        }

        [LogActivityResult("Delete Circular Issue", "User deleted circular issue", ActivityTypeDefaults.COMPLIANCE_DELETED_ISSUE, "CircularIssue")]
        public async Task<IActionResult> DeleteCircularIssue(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Issue Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_ISSUE_DELETE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _returnService.DeleteIssueAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete issue" });

                var resultData = result.Data;
                if (!resultData.Status)
                    return Ok(new { success = resultData.Status, message = resultData.Message ?? "Failed to delete issue", data = new { } });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error deleting issue: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RETURNS-CONTROLLER", ex.StackTrace);
                return Ok(new { success = true, data = new { } });
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

        #region Return Reports

        [HttpPost]
        public async Task<IActionResult> DailyReturnsReport() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcPeriodStatisticRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Period = "DAILY",
                Action = Activity.RETURNS_REPORT_DATA.GetDescription()
            };

            var result = await _returnService.GetPeriodReturnReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Daily Return For the Month");


            //..headers
            string[] headers =
            {
                "RETURN TITLE",
                "RETURN TYPE",
                "AUTHORITY",
                "DUE DATE",
                "STATUS",
                "BREACHED",
                "DEPARTMENT",
                "EXECUTIONER"
            };

            for (int col = 0; col < headers.Length; col++) {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            // Header styling
            var header = ws.Range(1, 1, 1, headers.Length);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Header height
            ws.Row(1).Height = 30;


            //..add data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.Title;
                ws.Cell(row, 2).Value = p.Type;
                ws.Cell(row, 3).Value = p.Authority;

                ws.Cell(row, 4).Value = p.PeriodEnd;
                ws.Cell(row, 4).Style.DateFormat.Format = "dd-MMM-yyyy";

                ws.Cell(row, 5).Value = p.Status;
                ws.Cell(row, 6).Value = p.IsBreached ? "YES" : "NO";
                ws.Cell(row, 7).Value = p.Department;
                ws.Cell(row, 8).Value = p.Executioner;

                row++;
            }

            int lastDataRow = row - 1;


            //..style columns
            ws.Column(1).Width = 70; //..title
            ws.Column(5).Width = 15; //..status

            for (int col = 2; col <= 8; col++) {
                if (col != 5)
                    ws.Column(col).Width = 20;
            }

            ws.Range(2, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range(2, 4, lastDataRow, 4).Style.Fill.BackgroundColor = XLColor.Gray;
            ws.Range(1, 4, lastDataRow, 4).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            //..status
            var statusRange = ws.Range(2, 5, lastDataRow, 5);
            statusRange.AddConditionalFormat().WhenEquals("BREACHED").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("CLOSED").Fill.SetBackgroundColor(XLColor.Green);
            statusRange.AddConditionalFormat().WhenEquals("OPEN").Fill.SetBackgroundColor(XLColor.Orange);

            //..status
            var breachRange = ws.Range(2, 6, lastDataRow, 6);
            breachRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Red);
            breachRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Green);

            //..add header filtersfilters
            ws.Range(1, 1, 1, headers.Length).SetAutoFilter();

            //..freeze header
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                $"DailyReturns-{DateTime.Today:yyyy-MM}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> WeeklyReturnsReport() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcPeriodStatisticRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Period = "WEEKLY",
                Action = Activity.RETURNS_REPORT_DATA.GetDescription()
            };

            var result = await _returnService.GetPeriodReturnReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Daily Return For the Month");


            //..headers
            string[] headers =
            {
                "RETURN TITLE",
                "RETURN TYPE",
                "AUTHORITY",
                "START DATE",
                "DUE DATE",
                "STATUS",
                "BREACHED",
                "DEPARTMENT",
                "EXECUTIONER"
            };

            for (int col = 0; col < headers.Length; col++) {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            // Header styling
            var header = ws.Range(1, 1, 1, headers.Length);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Header height
            ws.Row(1).Height = 30;


            //..add data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.Title;
                ws.Cell(row, 2).Value = p.Type;
                ws.Cell(row, 3).Value = p.Authority;

                ws.Cell(row, 4).Value = p.PeriodStart;
                ws.Cell(row, 4).Style.DateFormat.Format = "dd-MMM-yyyy";
                SetSafeDate(ws.Cell(row, 5), p.PeriodEnd);

                ws.Cell(row, 6).Value = p.Status;
                ws.Cell(row, 7).Value = p.IsBreached ? "YES" : "NO";
                ws.Cell(row, 8).Value = p.Department;
                ws.Cell(row, 9).Value = p.Executioner;

                row++;
            }

            int lastDataRow = row - 1;


            //..style columns
            ws.Column(1).Width = 70; //..title
            ws.Column(5).Width = 15; //..status

            for (int col = 2; col <= 8; col++) {
                if (col != 5)
                    ws.Column(col).Width = 20;
            }

            ws.Range(2, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range(2, 4, lastDataRow, 5).Style.Fill.BackgroundColor = XLColor.Gray;
            ws.Range(1, 5, lastDataRow, 5).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            //..status
            var statusRange = ws.Range(2, 6, lastDataRow, 6);
            statusRange.AddConditionalFormat().WhenEquals("BREACHED").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("CLOSED").Fill.SetBackgroundColor(XLColor.Green);
            statusRange.AddConditionalFormat().WhenEquals("OPEN").Fill.SetBackgroundColor(XLColor.Orange);

            //..status
            var breachRange = ws.Range(2, 7, lastDataRow, 7);
            breachRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Red);
            breachRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Green);

            //..add header filtersfilters
            ws.Range(1, 1, 1, headers.Length).SetAutoFilter();

            //..freeze header
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"WeeklyReturns-{DateTime.Today:yyyy-MM}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> QuarterlyReturnsReport() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcPeriodStatisticRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Period = "QUARTERLY",
                Action = Activity.RETURNS_REPORT_DATA.GetDescription()
            };

            var result = await _returnService.GetPeriodReturnReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Quarterly Return For the Month");


            //..headers
            string[] headers =
            {
                "RETURN TITLE",
                "RETURN TYPE",
                "FREQUENCY",
                "AUTHORITY",
                "DUE DATE",
                "STATUS",
                "BREACHED",
                "DEPARTMENT",
                "EXECUTIONER"
            };

            for (int col = 0; col < headers.Length; col++) {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            // Header styling
            var header = ws.Range(1, 1, 1, headers.Length);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Header height
            ws.Row(1).Height = 30;

            //..add data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.Title;
                ws.Cell(row, 2).Value = p.Type;
                ws.Cell(row, 3).Value = p.Authority;
                ws.Cell(row, 4).Value = p.Frequency;

                SetSafeDate(ws.Cell(row, 5), p.PeriodEnd);

                ws.Cell(row, 6).Value = p.Status;
                ws.Cell(row, 7).Value = p.IsBreached ? "YES" : "NO";
                ws.Cell(row, 8).Value = p.Department;
                ws.Cell(row, 9).Value = p.Executioner;

                row++;
            }

            int lastDataRow = row - 1;


            //..style columns
            ws.Column(1).Width = 70; //..title
            ws.Column(5).Width = 15; //..status

            for (int col = 2; col <= 8; col++) {
                if (col != 5)
                    ws.Column(col).Width = 20;
            }

            ws.Range(2, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range(2, 5, lastDataRow, 5).Style.Fill.BackgroundColor = XLColor.Gray;
            ws.Range(1, 5, lastDataRow, 5).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            //..status
            var statusRange = ws.Range(2, 6, lastDataRow, 6);
            statusRange.AddConditionalFormat().WhenEquals("BREACHED").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("CLOSED").Fill.SetBackgroundColor(XLColor.Green);
            statusRange.AddConditionalFormat().WhenEquals("OPEN").Fill.SetBackgroundColor(XLColor.Orange);

            //..status
            var breachRange = ws.Range(2, 7, lastDataRow, 7);
            breachRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Red);
            breachRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Green);

            //..add header filtersfilters
            ws.Range(1, 1, 1, headers.Length).SetAutoFilter();

            //..freeze header
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"QuarterlyReturns-{DateTime.Today:yyyy-MM}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> MonthlyReturnsReport() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcPeriodStatisticRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Period = "MONTHLY",
                Action = Activity.RETURNS_REPORT_DATA.GetDescription()
            };

            var result = await _returnService.GetPeriodReturnReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Monthly Return For the Month");


            //..headers
            string[] headers =
            {
                "RETURN TITLE",
                "RETURN TYPE",
                "FREQUENCY",
                "AUTHORITY",
                "START DATE",
                "DUE DATE",
                "STATUS",
                "BREACHED",
                "DEPARTMENT",
                "EXECUTIONER"
            };

            for (int col = 0; col < headers.Length; col++) {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            // Header styling
            var header = ws.Range(1, 1, 1, headers.Length);
            header.Style.Font.Bold = true;
            header.Style.Font.FontSize = 11;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.FromHtml("#E5E7EB"); 
            header.Style.Border.BottomBorder = XLBorderStyleValues.Thick;

            // Header height
            ws.Row(1).Height = 30;


            //..add data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.Title;
                ws.Cell(row, 2).Value = p.Type;
                ws.Cell(row, 3).Value = p.Frequency;
                ws.Cell(row, 4).Value = p.Authority;

                ws.Cell(row, 5).Value = p.PeriodStart;
                ws.Cell(row, 5).Style.DateFormat.Format = "dd-MMM-yyyy";
                SetSafeDate(ws.Cell(row, 6), p.PeriodEnd);

                ws.Cell(row, 7).Value = p.Status;
                ws.Cell(row, 8).Value = p.IsBreached ? "YES" : "NO";
                ws.Cell(row, 9).Value = p.Department;
                ws.Cell(row, 10).Value = p.Executioner;

                row++;
            }

            int lastDataRow = row - 1;
            var dataRange = ws.Range(2, 1, lastDataRow, headers.Length);
            dataRange.AddConditionalFormat().WhenIsTrue("MOD(ROW(),2)=0").Fill.SetBackgroundColor(XLColor.FromHtml("#F9FAFB"));

            //..style columns
            ws.Column(1).Width = 70; //..title
            ws.Column(7).Width = 15; //..status

            for (int col = 2; col <= 8; col++) {
                if (col < 20)
                    ws.Column(col).Width = 20;
            }

            ws.Range(2, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#F3F4F6");
            ws.Column(1).Style.Font.Bold = true;
            ws.Column(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Range(2, 5, lastDataRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
            ws.Range(1, 6, lastDataRow, 6).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            //..status
            var statusRange = ws.Range(2, 7, lastDataRow, 7);
            statusRange.AddConditionalFormat().WhenEquals("BREACHED").Fill.SetBackgroundColor(XLColor.FromHtml("#FCA5A5")); // soft red
            statusRange.AddConditionalFormat().WhenEquals("CLOSED").Fill.SetBackgroundColor(XLColor.FromHtml("#86EFAC")); // soft green
            statusRange.AddConditionalFormat().WhenEquals("OPEN").Fill.SetBackgroundColor(XLColor.FromHtml("#FDBA74")); // soft orange

            statusRange.Style.Font.Bold = true;
            statusRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //..status
            var breachRange = ws.Range(2, 8, lastDataRow, 8);
            breachRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.FromHtml("#EF4444"));
            breachRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.FromHtml("#22C55E"));

            breachRange.Style.Font.Bold = true;
            breachRange.Style.Font.FontColor = XLColor.White;
            breachRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Columns(5, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Columns(9, 10).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Hair;

            ws.Row(lastDataRow).Style.Border.BottomBorder = XLBorderStyleValues.Thick;


            //..add header filtersfilters
            ws.Range(1, 1, 1, headers.Length).SetAutoFilter();

            //..freeze header
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"MonthlyReturns-{DateTime.Today:yyyy-MM}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> AnnuallyReturnsReport() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcPeriodStatisticRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Period = "ANNUAL",
                Action = Activity.RETURNS_REPORT_DATA.GetDescription()
            };

            var result = await _returnService.GetPeriodReturnReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Annual Returns");
            //..headers
            string[] headers =
            {
                "RETURN TITLE",
                "RETURN TYPE",
                "AUTHORITY",
                "START DATE",
                "DUE DATE",
                "STATUS",
                "BREACHED",
                "DEPARTMENT",
                "EXECUTIONER"
            };

            for (int col = 0; col < headers.Length; col++) {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            // Header styling
            var header = ws.Range(1, 1, 1, headers.Length);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Header height
            ws.Row(1).Height = 30;


            //..add data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.Title;
                ws.Cell(row, 2).Value = p.Type;
                ws.Cell(row, 3).Value = p.Authority;

                SetSafeDate(ws.Cell(row, 4), p.PeriodEnd);
                SetSafeDate(ws.Cell(row, 5), p.PeriodEnd);

                ws.Cell(row, 6).Value = p.Status;
                ws.Cell(row, 7).Value = p.IsBreached ? "YES" : "NO";
                ws.Cell(row, 8).Value = p.Department;
                ws.Cell(row, 9).Value = p.Executioner;

                row++;
            }

            int lastDataRow = row - 1;


            //..style columns
            ws.Column(1).Width = 70; //..title
            ws.Column(5).Width = 15; //..status

            for (int col = 2; col <= 8; col++) {
                if (col != 5)
                    ws.Column(col).Width = 20;
            }

            ws.Range(2, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range(2, 4, lastDataRow, 5).Style.Fill.BackgroundColor = XLColor.Gray;
            ws.Range(1, 5, lastDataRow, 5).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            //..status
            var statusRange = ws.Range(2, 6, lastDataRow, 6);
            statusRange.AddConditionalFormat().WhenEquals("BREACHED").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("CLOSED").Fill.SetBackgroundColor(XLColor.Green);
            statusRange.AddConditionalFormat().WhenEquals("OPEN").Fill.SetBackgroundColor(XLColor.Orange);

            //..status
            var breachRange = ws.Range(2, 7, lastDataRow, 7);
            breachRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Red);
            breachRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Green);

            //..add header filtersfilters
            ws.Range(1, 1, 1, headers.Length).SetAutoFilter();

            //..freeze header
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"AnnuallyReturns-{DateTime.Today:yyyy-MM}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> BreachedReport() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Action = Activity.RETURNS_REPORT_DATA.GetDescription()
            };

            var result = await _returnService.GetBreachedReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Breaches Reports");


            //..headers
            string[] headers = {
                "RETURN TITLE",
                "FREQUENCY",
                "DUE DATE",
                "SUBMISSION DATE",
                "STATUS",
                "DEPARTMENT",
                "ASSOCIATED RISK"
            };

            for (int col = 0; col < headers.Length; col++) {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            // Header styling
            var header = ws.Range(1, 1, 1, headers.Length);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Header height
            ws.Row(1).Height = 30;


            //..add data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.ReportName;
                ws.Cell(row, 2).Value = p.Frequency;

                ws.Cell(row, 3).Value = p.DueDate;
                ws.Cell(row, 3).Style.DateFormat.Format = "dd-MMM-yyyy";
                SetSafeDate(ws.Cell(row, 4), p.SubmissionDate);

                ws.Cell(row, 5).Value = p.Status;
                ws.Cell(row, 6).Value = p.Department;
                ws.Cell(row, 7).Value = p.AssociatedRisk;
                row++;
            }

            int lastDataRow = row - 1;

            //..style columns
            ws.Column(1).Width = 70; //..title
            ws.Column(5).Width = 15; //..status

            for (int col = 2; col <= 8; col++) {
                if (col != 5)
                    ws.Column(col).Width = 20;
            }

            ws.Range(2, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range(2, 3, lastDataRow, 4).Style.Fill.BackgroundColor = XLColor.Gray;
            ws.Range(2, 3, lastDataRow, 4).Style.Font.FontColor = XLColor.WhiteSmoke;
            ws.Range(1, 4, lastDataRow, 4).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            //..status
            var statusRange = ws.Range(2, 5, lastDataRow, 5);
            statusRange.AddConditionalFormat().WhenEquals("CLOSED").Fill.SetBackgroundColor(XLColor.Green);
            statusRange.AddConditionalFormat().WhenEquals("OPEN").Fill.SetBackgroundColor(XLColor.Orange);

            //..add header filtersfilters
            ws.Range(1, 1, 1, headers.Length).SetAutoFilter();

            //..freeze header
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"BreachReturns-{DateTime.Today:yyyy-MM}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> MonthlySummeryReport() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Action = Activity.RETURNS_REPORT_DATA.GetDescription()
            };

            var result = await _returnService.GetMonthlySummeryAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Monthly Summery Returns");

            var data = result.Data;

            // Row 1 (group headers)
            ws.Cell(1, 1).Value = "NO";
            ws.Cell(1, 2).Value = "PERIOD";
            ws.Range(1, 3, 1, 4).Merge().Value = "SUBMITTED";
            ws.Range(1, 5, 1, 6).Merge().Value = "PENDING";
            ws.Range(1, 7, 1, 8).Merge().Value = "ON TIME";
            ws.Range(1, 9, 1, 10).Merge().Value = "BREACHED";

            ws.Cell(1, 11).Value = "TOTAL";
            ws.Cell(1, 12).Value = "COMPLIANCE %";

            //..row 2 as sub headers
            ws.Cell(2, 3).Value = "COUNT";
            ws.Cell(2, 4).Value = "%";
            ws.Cell(2, 5).Value = "COUNT";
            ws.Cell(2, 6).Value = "%";
            ws.Cell(2, 7).Value = "COUNT";
            ws.Cell(2, 8).Value = "%";
            ws.Cell(2, 9).Value = "COUNT";
            ws.Cell(2, 10).Value = "%";

            //..header styling
            var headerRange = ws.Range(1, 1, 2, 12);

            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#E5E7EB");

            //..borders
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            //..increase header height
            ws.Row(1).Height = 30;
            ws.Row(2).Height = 20;

            //..freez header rows
            ws.SheetView.FreezeRows(2);

            int row = 3;
            int no = 1;

            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = no;
                ws.Cell(row, 2).Value = p.Period;
                ws.Cell(row, 3).Value = p.Submitted;
                ws.Cell(row, 4).Value = p.SubmittedPercentage/100D;
                ws.Cell(row, 5).Value = p.Pending;
                ws.Cell(row, 6).Value = p.PendingPercentage/100D;
                ws.Cell(row, 7).Value = p.OnTime;
                ws.Cell(row, 8).Value = p.OnTimePercentage/100D;
                ws.Cell(row, 9).Value = p.Breached;
                ws.Cell(row, 10).Value = p.BreachedPercentage/100D;
                ws.Cell(row, 11).Value = p.Total;
                ws.Cell(row, 12).Value = p.ComplianceRate / 100D;

                row++;
                no++;
            }

            ws.Range(3, 4, row - 1, 4).Style.NumberFormat.Format = "0%";
            ws.Range(3, 6, row - 1, 6).Style.NumberFormat.Format = "0%";
            ws.Range(3, 8, row - 1, 8).Style.NumberFormat.Format = "0%";
            ws.Range(3, 10, row - 1, 10).Style.NumberFormat.Format = "0%";
            ws.Range(3, 12, row - 1, 12).Style.NumberFormat.Format = "0%";

            ws.Range(3, 4, row - 1, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
            ws.Range(3, 6, row - 1, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
            ws.Range(3, 8, row - 1, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
            ws.Range(3, 10, row - 1, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
            ws.Range(3, 12, row - 1, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");

            int totalRow = row;
            ws.Cell(totalRow, 1).Value = "TOTAL";
            ws.Cell(totalRow, 3).FormulaA1 = $"=SUM(C2:C{row - 1})";
            ws.Cell(totalRow, 5).FormulaA1 = $"=SUM(E2:E{row - 1})";
            ws.Cell(totalRow, 7).FormulaA1 = $"=SUM(G2:G{row - 1})";
            ws.Cell(totalRow, 9).FormulaA1 = $"=SUM(I2:I{row - 1})";
            ws.Cell(totalRow, 11).FormulaA1 = $"=SUM(K2:K{row - 1})";

            ////..average percentages
            //ws.Cell(totalRow, 4).FormulaA1 = $"=AVERAGE(D2:D{row - 1})";
            //ws.Cell(totalRow, 6).FormulaA1 = $"=AVERAGE(F2:F{row - 1})";
            //ws.Cell(totalRow, 8).FormulaA1 = $"=AVERAGE(H2:H{row - 1})";
            //ws.Cell(totalRow, 10).FormulaA1 = $"=AVERAGE(J2:J{row - 1})";
            //ws.Cell(totalRow, 12).FormulaA1 = $"=AVERAGE(L2:L{row - 1})";

            //ws.Range(2, 4, totalRow, 4).Style.NumberFormat.Format = "0%";
            //ws.Range(2, 6, totalRow, 6).Style.NumberFormat.Format = "0%";
            //ws.Range(2, 8, totalRow, 8).Style.NumberFormat.Format = "0%";
            //ws.Range(2, 10, totalRow, 10).Style.NumberFormat.Format = "0%";
            //ws.Range(2, 12, totalRow, 12).Style.NumberFormat.Format = "0%";

            //..grand total
            var totalRange = ws.Range(totalRow, 1, totalRow, 6);
            totalRange.Style.Font.Bold = true;
            totalRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Row(totalRow).Height = 26;
            ws.Cell(totalRow, 7).FormulaA1 = $"=SUM(C{totalRow}:F{totalRow})";

            //..adjust details column
            ws.Columns().AdjustToContents();
            ws.Column(1).Width = 6;
            ws.Column(2).Width = 18;

            for (int col = 3; col <= 12; col++) {
                ws.Column(col).Width = (col % 2 == 0) ? 10 : 12;
            }

            ws.Column(12).Width = 25;

            //..cap row height
            ws.Rows().Height = 15;
            ws.Row(1).Height = 30;
            ws.Row(totalRow).Height = 20;

            //..totals height
            ws.Row(totalRow).Height = 28;
            ws.Range(1, 1, totalRow, 1).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range(3, 1, totalRow, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range(1, 3, totalRow, 3).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range(1, 5, totalRow, 5).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range(1, 7, totalRow, 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range(1, 9, totalRow, 9).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range(1, 11, totalRow, 11).Style.Border.RightBorder = XLBorderStyleValues.Thin;

            //..center content
            ws.Range(2, 3, totalRow, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //..thick left border
            ws.Range(1, 2, totalRow, 2).Style.Border.LeftBorder = XLBorderStyleValues.Medium;
            ws.Range(1, 3, totalRow, 3).Style.Border.LeftBorder = XLBorderStyleValues.Medium;
            ws.Range(1, 5, totalRow, 5).Style.Border.LeftBorder = XLBorderStyleValues.Medium;
            ws.Range(1, 7, totalRow, 7).Style.Border.LeftBorder = XLBorderStyleValues.Medium;
            ws.Range(1, 9, totalRow, 9).Style.Border.LeftBorder = XLBorderStyleValues.Medium;

            //..total column thick boards on both sides
            ws.Range(1, 11, totalRow, 11).Style.Border.LeftBorder = XLBorderStyleValues.Medium;
            ws.Range(1, 11, totalRow, 11).Style.Border.RightBorder = XLBorderStyleValues.Medium;
            ws.Range(1, 12, totalRow, 12).Style.Border.RightBorder = XLBorderStyleValues.Medium;

            ws.Range(2, 3, totalRow - 1, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
            ws.Range(2, 4, totalRow, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
            ws.Range(2, 6, totalRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
            ws.Range(1, 7, totalRow, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#E5E7EB");
            ws.Range(2, 8, totalRow, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
            ws.Range(2, 10, totalRow, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
            ws.Range(2, 12, totalRow, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");

            //..compliance column
            var complianceRange = ws.Range(3, 12, totalRow - 1, 12);
            complianceRange.AddConditionalFormat().WhenBetween(0.9, 1).Fill.SetBackgroundColor(XLColor.Green);
            complianceRange.AddConditionalFormat().WhenLessThan(0.9).Fill.SetBackgroundColor(XLColor.Orange);

            //..bottom totals
            var totalRange2 = ws.Range(totalRow, 1, totalRow, 12);
            totalRange2.Style.Font.Bold = true;
            totalRange2.Style.Fill.BackgroundColor = XLColor.FromHtml("#D1D5DB");
            totalRange2.Style.Border.TopBorder = XLBorderStyleValues.Thick;
            totalRange2.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            ws.Row(totalRow).Height = 20;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 $"MONTHLY-SUMMERY-{DateTime.Today:MM-yyyy}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> BreachedAgingReport() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Action = Activity.RETURNS_REPORT_DATA.GetDescription()
            };

            var result = await _returnService.GetAgingReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Aging Report");

            var data = result.Data;

            //..define headers
            ws.Cell(1, 1).Value = "NO";
            ws.Cell(1, 2).Value = "REPORT NAME";
            ws.Cell(1, 3).Value = "FREQUENCY";
            ws.Cell(1, 4).Value = "DEPARTMENT";
            ws.Cell(1, 5).Value = "DUE DATE";
            ws.Cell(1, 6).Value = "SUBMISSION DATE";
            ws.Cell(1, 7).Value = "STATUS";
            ws.Cell(1, 8).Value = "OVERDUE(IN DAYS)";
            ws.Cell(1, 9).Value = "AGE BRACKET";
            ws.Cell(1, 10).Value = "ASSOCIATED RISK";

            //..header styling
            var header = ws.Range(1, 1, 1, 10);
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

            foreach (var record in data) {
                ws.Cell(row, 1).Value = no;
                ws.Cell(row, 2).Value = record.ReportName;
                ws.Cell(row, 3).Value = record.Frequency;
                ws.Cell(row, 4).Value = record.Department;

                ws.Cell(row, 5).Value = record.DueDate;
                ws.Cell(row, 5).Style.DateFormat.Format = "dd-MMM-yyyy";
                SetSafeDate(ws.Cell(row, 6), record.SubmissionDate);

                ws.Cell(row, 7).Value = record.Status;
                ws.Cell(row, 8).Value = record.DaysOverdue;
                ws.Cell(row, 9).Value = record.AgingBucket;
                ws.Cell(row, 10).Value = record.AssociatedRisk;
                row++;
                no++;
            }

            int lastDataRow = row - 1;
            // Column shading
            ws.Range(2, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;   // NO
            ws.Range(2, 2, lastDataRow, 2).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;   // REPORT NAME
            ws.Range(2, 3, lastDataRow, 3).Style.Fill.BackgroundColor = XLColor.AliceBlue;    // FREQUENCY
            ws.Range(2, 4, lastDataRow, 4).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;   // DEPARTMENT
            ws.Range(2, 5, lastDataRow, 5).Style.Fill.BackgroundColor = XLColor.MistyRose;    // DUE DATE
            ws.Range(2, 6, lastDataRow, 6).Style.Fill.BackgroundColor = XLColor.Honeydew;     // SUBMISSION DATE
            ws.Range(2, 8, lastDataRow, 8).Style.Fill.BackgroundColor = XLColor.LemonChiffon; // OVERDUE
            ws.Range(2, 9, lastDataRow, 9).Style.Fill.BackgroundColor = XLColor.Lavender;     // AGE BRACKET
            ws.Range(2, 10, lastDataRow, 10).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;  // RISK

            //..zebra striping
            for (int r = 2; r <= lastDataRow; r++) {
                if(r % 2 == 0){
                    ws.Range(r, 1, r, 10).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
                }
            }

            //..status
            var statusRange = ws.Range(2, 7, lastDataRow, 7);
            statusRange.AddConditionalFormat().WhenEquals("CLOSED").Fill.SetBackgroundColor(XLColor.Green);
            statusRange.AddConditionalFormat().WhenEquals("OPEN").Fill.SetBackgroundColor(XLColor.Orange);

            statusRange.Style.Font.Bold = true;
            statusRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //..add header filtersfilters
            ws.Range(1, 2, 1, 9).SetAutoFilter();

            //..wrap text for risk column
            ws.Column(10).Style.Alignment.WrapText = true;
            ws.Column(10).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            //..center numeric columns
            ws.Range(2, 1, lastDataRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(2, 8, lastDataRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(1, 1, lastDataRow, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //..adjust details column
            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(1);

            //..set risk column width
            ws.Column(10).Width = 40;

            var overdueRange = ws.Range(2, 8, lastDataRow, 8);

            // > 30 days with red
            overdueRange.AddConditionalFormat().WhenGreaterThan(30).Fill.SetBackgroundColor(XLColor.Red);

            // 1–30 days with yellow
            overdueRange.AddConditionalFormat().WhenBetween(1, 30).Fill.SetBackgroundColor(XLColor.LightYellow);
            overdueRange.Style.Font.Bold = true;
            overdueRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 $"BREACH-AGING-{DateTime.Today:MM-yyyy}.xlsx");
        }

        #endregion

        #region Circular Reports

        [HttpPost]
        public async Task<IActionResult> MofReport() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcAuthorityStatisticRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Authority = "MoFED",
                Action = Activity.CIRCULAR_REPORT_DATA.GetDescription()
            };

            var result = await _returnService.GetCircularReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("MoFED Circulars");

            //..headers
            string[] headers =
            {
                "CIRCULAR TITLE",
                "AUTHORITY",
                "DUE DATE",
                "STATUS",
                "SUBMISSION DATE",
                "BREACHED",
                "DEPARTMENT"
            };

            for (int col = 0; col < headers.Length; col++) {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            // Header styling
            var header = ws.Range(1, 1, 1, headers.Length);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Header height
            ws.Row(1).Height = 30;

            //..add data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.Title;
                ws.Cell(row, 2).Value = p.AuthorityAlias;
                ws.Cell(row, 3).Value = p.DueDate;
                ws.Cell(row, 3).Style.DateFormat.Format = "dd-MMM-yyyy";

                ws.Cell(row, 4).Value = p.Status;

                SetSafeDate(ws.Cell(row, 5), p.SubmissionDate);

                ws.Cell(row, 5).Value = p.Department;
                ws.Cell(row, 6).Value = p.IsBreached ? "YES" : "NO";
                row++;
            }

            int lastDataRow = row - 1;

            //..style columns
            ws.Column(1).Width = 70; //..title
            ws.Column(5).Width = 15; //..status

            for (int col = 2; col <= 8; col++) {
                if (col != 5)
                    ws.Column(col).Width = 20;
            }

            ws.Range(2, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range(2, 4, lastDataRow, 4).Style.Fill.BackgroundColor = XLColor.Gray;
            ws.Range(1, 4, lastDataRow, 4).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            //..status
            var statusRange = ws.Range(2, 4, lastDataRow, 4);
            statusRange.AddConditionalFormat().WhenEquals("UNKNOWN").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("CLOSED").Fill.SetBackgroundColor(XLColor.Green);
            statusRange.AddConditionalFormat().WhenEquals("OPEN").Fill.SetBackgroundColor(XLColor.Orange);

            //..breached
            var breachRange = ws.Range(2, 6, lastDataRow, 6);
            breachRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Red);
            breachRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Green);

            //..add header filtersfilters
            ws.Range(1, 1, 1, headers.Length).SetAutoFilter();

            //..freeze header
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(),"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"MoFEDCirculars-{DateTime.Today:yyyy-MM}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> DpfReport() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcAuthorityStatisticRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Authority = "DPF",
                Action = Activity.CIRCULAR_REPORT_DATA.GetDescription()
            };

            var result = await _returnService.GetCircularReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("DPF Circulars");

            //..headers
            string[] headers =
            {
                "CIRCULAR TITLE",
                "AUTHORITY",
                "DUE DATE",
                "STATUS",
                "SUBMISSION DATE",
                "BREACHED",
                "DEPARTMENT"
            };

            for (int col = 0; col < headers.Length; col++) {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            // Header styling
            var header = ws.Range(1, 1, 1, headers.Length);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Header height
            ws.Row(1).Height = 30;

            //..add data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.Title;
                ws.Cell(row, 2).Value = p.AuthorityAlias;

                ws.Cell(row, 3).Value = p.DueDate;
                ws.Cell(row, 3).Style.DateFormat.Format = "dd-MMM-yyyy";

                ws.Cell(row, 4).Value = p.Status;

                SetSafeDate(ws.Cell(row, 5), p.SubmissionDate);

                ws.Cell(row, 5).Value = p.Department;

                ws.Cell(row, 6).Value = p.IsBreached ? "YES" : "NO";

                row++;
            }

            int lastDataRow = row - 1;

            //..style columns
            ws.Column(1).Width = 70; //..title
            ws.Column(5).Width = 15; //..status

            for (int col = 2; col <= 8; col++) {
                if (col != 5)
                    ws.Column(col).Width = 20;
            }

            ws.Range(2, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range(2, 4, lastDataRow, 4).Style.Fill.BackgroundColor = XLColor.Gray;
            ws.Range(1, 4, lastDataRow, 4).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            //..status
            var statusRange = ws.Range(2, 4, lastDataRow, 4);
            statusRange.AddConditionalFormat().WhenEquals("UNKNOWN").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("CLOSED").Fill.SetBackgroundColor(XLColor.Green);
            statusRange.AddConditionalFormat().WhenEquals("OPEN").Fill.SetBackgroundColor(XLColor.Orange);

            //..breached
            var breachRange = ws.Range(2, 6, lastDataRow, 6);
            breachRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Red);
            breachRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Green);

            //..add header filtersfilters
            ws.Range(1, 1, 1, headers.Length).SetAutoFilter();

            //..freeze header
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"DPFCirculars-{DateTime.Today:yyyy-MM}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> UraReport() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcAuthorityStatisticRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Authority = "URA",
                Action = Activity.CIRCULAR_REPORT_DATA.GetDescription()
            };

            var result = await _returnService.GetCircularReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("URA Circulars");

            //..headers
            string[] headers =
            {
                "CIRCULAR TITLE",
                "AUTHORITY",
                "DUE DATE",
                "STATUS",
                "SUBMISSION DATE",
                "BREACHED",
                "DEPARTMENT"
            };

            for (int col = 0; col < headers.Length; col++) {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            // Header styling
            var header = ws.Range(1, 1, 1, headers.Length);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Header height
            ws.Row(1).Height = 30;

            //..add data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.Title;
                ws.Cell(row, 2).Value = p.AuthorityAlias;

                ws.Cell(row, 3).Value = p.DueDate;
                ws.Cell(row, 3).Style.DateFormat.Format = "dd-MMM-yyyy";

                ws.Cell(row, 4).Value = p.Status;

                SetSafeDate(ws.Cell(row, 5), p.SubmissionDate);

                ws.Cell(row, 5).Value = p.Department;

                ws.Cell(row, 6).Value = p.IsBreached ? "YES" : "NO";

                row++;
            }

            int lastDataRow = row - 1;

            //..style columns
            ws.Column(1).Width = 70; //..title
            ws.Column(5).Width = 15; //..status

            for (int col = 2; col <= 8; col++) {
                if (col != 5)
                    ws.Column(col).Width = 20;
            }

            ws.Range(2, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range(2, 4, lastDataRow, 4).Style.Fill.BackgroundColor = XLColor.Gray;
            ws.Range(1, 4, lastDataRow, 4).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            //..status
            var statusRange = ws.Range(2, 4, lastDataRow, 4);
            statusRange.AddConditionalFormat().WhenEquals("UNKNOWN").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("CLOSED").Fill.SetBackgroundColor(XLColor.Green);
            statusRange.AddConditionalFormat().WhenEquals("OPEN").Fill.SetBackgroundColor(XLColor.Orange);

            //..breached
            var breachRange = ws.Range(2, 6, lastDataRow, 6);
            breachRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Red);
            breachRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Green);

            //..add header filtersfilters
            ws.Range(1, 1, 1, headers.Length).SetAutoFilter();

            //..freeze header
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"URACirculars-{DateTime.Today:yyyy-MM}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> PpdaReport() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcAuthorityStatisticRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Authority = "PPDA",
                Action = Activity.CIRCULAR_REPORT_DATA.GetDescription()
            };

            var result = await _returnService.GetCircularReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("PPDA Circulars");

            //..headers
            string[] headers =
            {
                "CIRCULAR TITLE",
                "AUTHORITY",
                "DUE DATE",
                "STATUS",
                "SUBMISSION DATE",
                "BREACHED",
                "DEPARTMENT"
            };

            for (int col = 0; col < headers.Length; col++) {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            // Header styling
            var header = ws.Range(1, 1, 1, headers.Length);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Header height
            ws.Row(1).Height = 30;

            //..add data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.Title;
                ws.Cell(row, 2).Value = p.AuthorityAlias;

                ws.Cell(row, 3).Value = p.DueDate;
                ws.Cell(row, 3).Style.DateFormat.Format = "dd-MMM-yyyy";

                ws.Cell(row, 4).Value = p.Status;

                SetSafeDate(ws.Cell(row, 5), p.SubmissionDate);

                ws.Cell(row, 5).Value = p.Department;

                ws.Cell(row, 6).Value = p.IsBreached ? "YES" : "NO";

                row++;
            }

            int lastDataRow = row - 1;

            //..style columns
            ws.Column(1).Width = 70; //..title
            ws.Column(5).Width = 15; //..status

            for (int col = 2; col <= 8; col++) {
                if (col != 5)
                    ws.Column(col).Width = 20;
            }

            ws.Range(2, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range(2, 4, lastDataRow, 4).Style.Fill.BackgroundColor = XLColor.Gray;
            ws.Range(1, 4, lastDataRow, 4).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            //..status
            var statusRange = ws.Range(2, 4, lastDataRow, 4);
            statusRange.AddConditionalFormat().WhenEquals("UNKNOWN").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("CLOSED").Fill.SetBackgroundColor(XLColor.Green);
            statusRange.AddConditionalFormat().WhenEquals("OPEN").Fill.SetBackgroundColor(XLColor.Orange);

            //..breached
            var breachRange = ws.Range(2, 6, lastDataRow, 6);
            breachRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Red);
            breachRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Green);

            //..add header filtersfilters
            ws.Range(1, 1, 1, headers.Length).SetAutoFilter();

            //..freeze header
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"PPDACirculars-{DateTime.Today:yyyy-MM}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> BouReport() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcAuthorityStatisticRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Authority = "BOU",
                Action = Activity.CIRCULAR_REPORT_DATA.GetDescription()
            };

            var result = await _returnService.GetCircularReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("BOU Circulars");

            //..headers
            string[] headers =
            {
                "CIRCULAR TITLE",
                "AUTHORITY",
                "DUE DATE",
                "STATUS",
                "SUBMISSION DATE",
                "BREACHED",
                "DEPARTMENT"
            };

            for (int col = 0; col < headers.Length; col++) {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            // Header styling
            var header = ws.Range(1, 1, 1, headers.Length);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Header height
            ws.Row(1).Height = 30;

            //..add data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.Title;
                ws.Cell(row, 2).Value = p.Authority;

                ws.Cell(row, 3).Value = p.DueDate;
                ws.Cell(row, 3).Style.DateFormat.Format = "dd-MMM-yyyy";

                ws.Cell(row, 4).Value = p.Status;

                SetSafeDate(ws.Cell(row, 5), p.SubmissionDate);

                ws.Cell(row, 5).Value = p.Department;

                ws.Cell(row, 6).Value = p.IsBreached ? "YES" : "NO";

                row++;
            }

            int lastDataRow = row - 1;

            //..style columns
            ws.Column(1).Width = 70; //..title
            ws.Column(5).Width = 15; //..status

            for (int col = 2; col <= 8; col++) {
                if (col != 5)
                    ws.Column(col).Width = 20;
            }

            ws.Range(2, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range(2, 4, lastDataRow, 4).Style.Fill.BackgroundColor = XLColor.Gray;
            ws.Range(1, 4, lastDataRow, 4).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            //..status
            var statusRange = ws.Range(2, 4, lastDataRow, 4);
            statusRange.AddConditionalFormat().WhenEquals("UNKNOWN").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("CLOSED").Fill.SetBackgroundColor(XLColor.Green);
            statusRange.AddConditionalFormat().WhenEquals("OPEN").Fill.SetBackgroundColor(XLColor.Orange);

            //..breached
            var breachRange = ws.Range(2, 6, lastDataRow, 6);
            breachRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Red);
            breachRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Green);

            //..add header filtersfilters
            ws.Range(1, 1, 1, headers.Length).SetAutoFilter();

            //..freeze header
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BOUCirculars-{DateTime.Today:yyyy-MM}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> OthersReport() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcAuthorityStatisticRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Authority = "OTHER",
                Action = Activity.CIRCULAR_REPORT_DATA.GetDescription()
            };

            var result = await _returnService.GetCircularReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve return data" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Other Authorites");

            //..headers
            string[] headers =
            {
                "CIRCULAR TITLE",
                "AUTHORITY",
                "DUE DATE",
                "STATUS",
                "SUBMISSION DATE",
                "BREACHED",
                "DEPARTMENT"
            };

            for (int col = 0; col < headers.Length; col++) {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            // Header styling
            var header = ws.Range(1, 1, 1, headers.Length);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Header height
            ws.Row(1).Height = 30;

            //..add data
            int row = 2;
            foreach (var p in result.Data) {
                ws.Cell(row, 1).Value = p.Title;
                ws.Cell(row, 2).Value = p.AuthorityAlias;

                ws.Cell(row, 3).Value = p.DueDate;
                ws.Cell(row, 3).Style.DateFormat.Format = "dd-MMM-yyyy";

                ws.Cell(row, 4).Value = p.Status;

                SetSafeDate(ws.Cell(row, 5), p.SubmissionDate);

                ws.Cell(row, 5).Value = p.Department;

                ws.Cell(row, 6).Value = p.IsBreached ? "YES" : "NO";

                row++;
            }

            int lastDataRow = row - 1;

            //..style columns
            ws.Column(1).Width = 70; //..title
            ws.Column(5).Width = 15; //..status

            for (int col = 2; col <= 8; col++) {
                if (col != 5)
                    ws.Column(col).Width = 20;
            }

            ws.Range(2, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range(2, 4, lastDataRow, 4).Style.Fill.BackgroundColor = XLColor.Gray;
            ws.Range(1, 4, lastDataRow, 4).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            //..status
            var statusRange = ws.Range(2, 4, lastDataRow, 4);
            statusRange.AddConditionalFormat().WhenEquals("UNKNOWN").Fill.SetBackgroundColor(XLColor.Red);
            statusRange.AddConditionalFormat().WhenEquals("CLOSED").Fill.SetBackgroundColor(XLColor.Green);
            statusRange.AddConditionalFormat().WhenEquals("OPEN").Fill.SetBackgroundColor(XLColor.Orange);

            //..breached
            var breachRange = ws.Range(2, 6, lastDataRow, 6);
            breachRange.AddConditionalFormat().WhenEquals("YES").Fill.SetBackgroundColor(XLColor.Red);
            breachRange.AddConditionalFormat().WhenEquals("NO").Fill.SetBackgroundColor(XLColor.Green);

            //..add header filtersfilters
            ws.Range(1, 1, 1, headers.Length).SetAutoFilter();

            //..freeze header
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"OtherCirculars-{DateTime.Today:yyyy-MM}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> CircularSummeryReport() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                Action = Activity.CIRCULAR_REPORT_DATA.GetDescription()
            };

            var result = await _returnService.GetCircularSummeryReportAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve circular data" });

            var data = result.Data;

            //..generate sheet
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Circular Summary");
            var kpiSheet = workbook.Worksheets.Add("Summary KPIs");

            // Row 1 – grouped headers
            ws.Cell(1, 1).Value = "NO";
            ws.Cell(1, 2).Value = "AUTHORITY";
            ws.Cell(1, 3).Value = "TOTAL RECEIVED";

            ws.Range(1, 4, 1, 5).Merge().Value = "CLOSED";
            ws.Range(1, 6, 1, 7).Merge().Value = "OUTSTANDING";
            ws.Range(1, 8, 1, 9).Merge().Value = "BREACHED";

            ws.Cell(1, 10).Value = "COMPLIANCE %";

            //..sub headers
            ws.Cell(2, 4).Value = "COUNT";
            ws.Cell(2, 5).Value = "%";

            ws.Cell(2, 6).Value = "COUNT";
            ws.Cell(2, 7).Value = "%";

            ws.Cell(2, 8).Value = "COUNT";
            ws.Cell(2, 9).Value = "%";

            //..header styling
            var headerRange = ws.Range(1, 1, 2, 10);

            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.Row(1).Height = 28;
            ws.Row(2).Height = 22;

            //..freez header
            ws.SheetView.FreezeRows(2);

            int row = 3;
            int no = 1;

            foreach (var a in data) {
                ws.Cell(row, 1).Value = no;
                ws.Cell(row, 2).Value = $"{a.AuthorityAlias} - {a.AuthorityName}";
                ws.Cell(row, 3).Value = a.TotalReceived;
                ws.Cell(row, 4).Value = a.Closed.Count;
                ws.Cell(row, 5).Value = a.Closed.Percentage / 100;
                ws.Cell(row, 6).Value = a.Outstanding.Count;
                ws.Cell(row, 7).Value = a.Outstanding.Percentage / 100;
                ws.Cell(row, 8).Value = a.Breached.Count;
                ws.Cell(row, 9).Value = a.Breached.Percentage / 100;
                ws.Cell(row, 10).Value = a.ComplianceRate / 100;
                row++;
                no++;
            }

            int lastDataRow = row - 1;
            ws.Range(3, 5, lastDataRow, 5).Style.NumberFormat.Format = "0%";
            ws.Range(3, 7, lastDataRow, 7).Style.NumberFormat.Format = "0%";
            ws.Range(3, 9, lastDataRow, 9).Style.NumberFormat.Format = "0%";
            ws.Range(3, 10, lastDataRow, 10).Style.NumberFormat.Format = "0%";

            //..column shading
            ws.Range(3, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.WhiteSmoke; // NO
            ws.Range(3, 2, lastDataRow, 2).Style.Fill.BackgroundColor = XLColor.WhiteSmoke; // AUTHORITY
            ws.Range(3, 3, lastDataRow, 3).Style.Fill.BackgroundColor = XLColor.AliceBlue;  // TOTAL
            ws.Range(3, 4, lastDataRow, 5).Style.Fill.BackgroundColor = XLColor.Honeydew;    // CLOSED
            ws.Range(3, 6, lastDataRow, 7).Style.Fill.BackgroundColor = XLColor.LightYellow; // OUTSTANDING
            ws.Range(3, 8, lastDataRow, 9).Style.Fill.BackgroundColor = XLColor.MistyRose;   // BREACHED

            //..zebra striping
            for (int r = 3; r <= lastDataRow; r++) {
                if (r % 2 == 0) {
                    ws.Range(r, 1, r, 10)
                      .Style.Fill.BackgroundColor = XLColor.FromHtml("#F9FAFB");
                }
            }

            var complianceRange = ws.Range(3, 10, lastDataRow, 10);
            complianceRange.AddConditionalFormat().WhenBetween(0.9, 1).Fill.SetBackgroundColor(XLColor.Green);
            complianceRange.AddConditionalFormat().WhenBetween(0.75, 0.8999).Fill.SetBackgroundColor(XLColor.Orange);
            complianceRange.AddConditionalFormat().WhenLessThan(0.75).Fill.SetBackgroundColor(XLColor.Red);
            complianceRange.Style.Font.Bold = true;

            //..boarders and resize
            ws.Range(3, 1, lastDataRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(3, 3, lastDataRow, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(1, 1, lastDataRow, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // Widths
            ws.Column(1).Width = 6;
            ws.Column(2).Width = 30;
            ws.Column(3).Width = 14;

            for (int c = 4; c <= 10; c++) {
                ws.Column(c).Width = 12;
            }

            int totalRow = lastDataRow + 1;

            //..label
            ws.Cell(totalRow, 1).Value = "TOTAL";
            ws.Range(totalRow, 1, totalRow, 2).Merge();

            //..totals (counts)
            ws.Cell(totalRow, 3).FormulaA1 = $"=SUM(C3:C{lastDataRow})";
            ws.Cell(totalRow, 4).FormulaA1 = $"=SUM(D3:D{lastDataRow})"; // Closed
            ws.Cell(totalRow, 6).FormulaA1 = $"=SUM(F3:F{lastDataRow})"; // Outstanding
            ws.Cell(totalRow, 8).FormulaA1 = $"=SUM(H3:H{lastDataRow})"; // Breached

            //..percentages
            ws.Cell(totalRow, 5).FormulaA1 = $"=IF(C{totalRow}=0,0,D{totalRow}/C{totalRow})";
            ws.Cell(totalRow, 7).FormulaA1 = $"=IF(C{totalRow}=0,0,F{totalRow}/C{totalRow})";
            ws.Cell(totalRow, 9).FormulaA1 = $"=IF(C{totalRow}=0,0,H{totalRow}/C{totalRow})";

            // Compliance rate
            ws.Cell(totalRow, 10).FormulaA1 = $"=IF(C{totalRow}=0,0,D{totalRow}/C{totalRow})";

            //..totals style
            var totalRange = ws.Range(totalRow, 1, totalRow, 10);
            totalRange.Style.Font.Bold = true;
            totalRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#D1D5DB");
            totalRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
            totalRange.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            ws.Row(totalRow).Height = 26;

            ws.Range(3, 5, totalRow, 5).Style.NumberFormat.Format = "0%";
            ws.Range(3, 7, totalRow, 7).Style.NumberFormat.Format = "0%";
            ws.Range(3, 9, totalRow, 9).Style.NumberFormat.Format = "0%";
            ws.Range(3, 10, totalRow, 10).Style.NumberFormat.Format = "0%";

            //..add KPI Sheet
            kpiSheet.Cell(2, 2).Value = "Total Circulars";
            kpiSheet.Cell(2, 3).FormulaA1 = $"='Circular Summary'!C{totalRow}";

            kpiSheet.Cell(3, 2).Value = "Closed";
            kpiSheet.Cell(3, 3).FormulaA1 = $"='Circular Summary'!D{totalRow}";

            kpiSheet.Cell(4, 2).Value = "Outstanding";
            kpiSheet.Cell(4, 3).FormulaA1 = $"='Circular Summary'!F{totalRow}";

            kpiSheet.Cell(5, 2).Value = "Breached";
            kpiSheet.Cell(5, 3).FormulaA1 = $"='Circular Summary'!H{totalRow}";

            kpiSheet.Cell(6, 2).Value = "Compliance Rate";
            kpiSheet.Cell(6, 3).FormulaA1 = $"='Circular Summary'!J{totalRow}";
            kpiSheet.Cell(6, 3).Style.NumberFormat.Format = "0%";

            //..styles
            var kpiRange = kpiSheet.Range(2, 2, 6, 3);
            kpiRange.Style.Font.Bold = true;
            kpiRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            kpiRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            kpiSheet.Column(2).Width = 30;
            kpiSheet.Column(3).Width = 16;
            kpiSheet.Range(2, 3, 5, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Center totals row
            ws.Range(totalRow, 1, totalRow, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(totalRow, 1, totalRow, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range(totalRow, 1, totalRow, 10).Style.Font.FontSize = 12;

            kpiSheet.Range(6, 3, 6, 3).AddConditionalFormat().WhenBetween(0.9, 1).Fill.SetBackgroundColor(XLColor.Green);
            kpiSheet.Range(6, 3, 6, 3).AddConditionalFormat().WhenLessThan(0.9).Fill.SetBackgroundColor(XLColor.Orange);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 $"CIRCULAR-SUMMERY-{DateTime.Today:MM-yyyy}.xlsx");
        }

        #endregion

        #region Private methods

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

        #endregion
    }
}
