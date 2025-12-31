using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {
    public class ComplianceObligationRegisterViewComponent : ViewComponent {
        private readonly IControlFactory _controlactory;
        private readonly IWebHelper _webHelper;
        private readonly IAuthenticationService _authService;

        public ComplianceObligationRegisterViewComponent(IControlFactory controlFactory, IWebHelper webHelper, IAuthenticationService authService) {
            _controlactory = controlFactory;
            _webHelper = webHelper;
            _authService = authService;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            var model = new ControlViewModel();
            //..get user IP address
            var ipAddress = _webHelper.GetCurrentIpAddress();

            //..get current authenticated user record
            var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (grcResponse.HasError) {
                return View(model);
            }

            var currentUser = grcResponse.Data;
            currentUser.IPAddress = ipAddress;
            model = await _controlactory.PrepareControlViewModelAsync(currentUser);
            return View(model);
        }
    }
}
