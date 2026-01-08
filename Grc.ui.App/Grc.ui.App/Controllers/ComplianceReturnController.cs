using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> GetPagedReturnsList([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("RETURNS DATA ERROR: Failed to get user");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_RETURNS_RETRIEVE.GetDescription();

                var result = await _returnService.GetPagedReturnsAsync(request);
                PagedResponse<GrcReturnsResponse> returns = result.Data ?? new();

                var pagedEntities = (returns.Entities ?? new List<GrcReturnsResponse>())
                    .Select(r => new {
                        id = r.Id,
                        reportName = r.ReportName ?? string.Empty,
                        frequency = r.Frequency ?? string.Empty,
                        department = r.Department ?? string.Empty,
                        authority = r.Authority ?? string.Empty,
                        article = r.Article ?? string.Empty,
                        isDeleted = r.IsDeleted,
                        comments = r.Comments ?? string.Empty
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)returns.TotalCount / returns.Size);
                return Ok(new { last_page = totalPages, total_records = returns.TotalCount, data = pagedEntities });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving returns: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "RETURNS-REGISTER", ex.StackTrace);
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
