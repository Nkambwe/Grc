using Grc.ui.App.Dtos;
using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {
    public class AnnualReturnsViewComponent : ViewComponent {
        private readonly IDashboardFactory _dashboardFactory;
        private readonly IWebHelper _webHelper;
        private readonly IAuthenticationService _authService;

        public AnnualReturnsViewComponent(IDashboardFactory dashboardFactory, IWebHelper webHelper, IAuthenticationService authService) {
            _dashboardFactory = dashboardFactory;
            _webHelper = webHelper;
            _authService = authService;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            //..get user IP address
            var ipAddress = _webHelper.GetCurrentIpAddress();

            //..get current authenticated user record
            var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (grcResponse.HasError) {
                return View(new ComplianceExtensionReturnStatisticViewModel());
            }

            var currentUser = grcResponse.Data;
            currentUser.IPAddress = ipAddress;
            return View(await _dashboardFactory.PrepareReturnExtensionDashboardModelAsync(grcResponse.Data, "ANNUAL"));
        }
    }
}
