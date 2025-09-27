using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Controllers {

    public class RegulationsController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly ISystemAccessService _accessService;
        private readonly IDashboardFactory _dashboardFactory;

        public RegulationsController(IApplicationLoggerFactory loggerFactory,
                                    IDashboardFactory dashboardFactory,
                                    IEnvironmentProvider environment,
                                    IWebHelper webHelper,
                                    ILocalizationService localizationService,
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
        }

        public async Task<IActionResult> ReceivedRegulations() {
            try{
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    return RedirectToAction("Login");
                }

                return View(await _dashboardFactory.PrepareUserDashboardModelAsync(grcResponse.Data));
            } catch(Exception ex){
                Logger.LogActivity($"Error Loading recieved regulations: {ex.Message}", "ERROR");

                //..capture error to bug tracker
                _ = await ProcessErrorAsync(ex.Message, "REGULATION-DASHBORAD", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        public async Task<IActionResult> OpenRegulations()
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    return RedirectToAction("Login");
                }

                return View(await _dashboardFactory.PrepareUserDashboardModelAsync(grcResponse.Data));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error Loading open regulations: {ex.Message}", "ERROR");

                //..capture error to bug tracker
                _ = await ProcessErrorAsync(ex.Message, "REGULATION-DASHBORAD", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }
       
        public async Task<IActionResult> ClosedRegulations()
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    return RedirectToAction("Login");
                }

                return View(await _dashboardFactory.PrepareUserDashboardModelAsync(grcResponse.Data));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error Loading closed regulations: {ex.Message}", "ERROR");

                //..capture error to bug tracker
                _ = await ProcessErrorAsync(ex.Message, "REGULATION-DASHBORAD", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }
        
        public async Task<IActionResult> RegulatoryApplicable()
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    return RedirectToAction("Login");
                }

                return View(await _dashboardFactory.PrepareUserDashboardModelAsync(grcResponse.Data));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error Loading applicable regulations: {ex.Message}", "ERROR");

                //..capture error to bug tracker
                _ = await ProcessErrorAsync(ex.Message, "REGULATION-DASHBORAD", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }
       
        public async Task<IActionResult> RegulatoryGaps()
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    return RedirectToAction("Login");
                }

                return View(await _dashboardFactory.PrepareUserDashboardModelAsync(grcResponse.Data));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error Loading regulation gaps: {ex.Message}", "ERROR");

                //..capture error to bug tracker
                _ = await ProcessErrorAsync(ex.Message, "REGULATION-DASHBORAD", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }
        
        public async Task<IActionResult> RegulatoryCovered()
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    return RedirectToAction("Login");
                }

                return View(await _dashboardFactory.PrepareUserDashboardModelAsync(grcResponse.Data));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error Loading covered regulations: {ex.Message}", "ERROR");

                //..capture error to bug tracker
                _ = await ProcessErrorAsync(ex.Message, "REGULATION-DASHBORAD", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }
        
        public async Task<IActionResult> RegulatoryIssues()
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    return RedirectToAction("Login");
                }

                return View(await _dashboardFactory.PrepareUserDashboardModelAsync(grcResponse.Data));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error Loading issues on regulations: {ex.Message}", "ERROR");

                //..capture error to bug tracker
                _ = await ProcessErrorAsync(ex.Message, "REGULATION-DASHBORAD", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }
       
        public async Task<IActionResult> RegulatoryNotApplicable() {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    return RedirectToAction("Login");
                }

                return View(await _dashboardFactory.PrepareUserDashboardModelAsync(grcResponse.Data));
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error Loading uncovered regulations: {ex.Message}", "ERROR");

                //..capture error to bug tracker
                _ = await ProcessErrorAsync(ex.Message, "REGULATION-DASHBORAD", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }
     
    }
}
