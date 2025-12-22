using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {

    public class ComplianceRegulatoryRegisterViewComponent : ViewComponent {
        private readonly IStatuteFactory _statuteFactory;
        private readonly IWebHelper _webHelper;
        private readonly IAuthenticationService _authService;

        public ComplianceRegulatoryRegisterViewComponent(
            IStatuteFactory statuteFactory,
            IWebHelper webHelper,
            IAuthenticationService authService) {
            _statuteFactory = statuteFactory;
            _webHelper = webHelper;
            _authService = authService;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            var model = new StatutoryViewModel();
            //..get user IP address
            var ipAddress = _webHelper.GetCurrentIpAddress();

            //..get current authenticated user record
            var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (grcResponse.HasError) {

                return View(model);
            }

            var currentUser = grcResponse.Data;
            currentUser.IPAddress = ipAddress;
            model = await _statuteFactory.PrepareStatuteViewModelAsync(currentUser);
            return View(model);
        }

    }

}
