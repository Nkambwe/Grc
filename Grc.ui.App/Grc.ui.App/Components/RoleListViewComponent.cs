using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {

    public class RoleListViewComponent : ViewComponent {

        private readonly ISupportDashboardFactory _dDashboardFactory;
        private readonly IWebHelper _webHelper;
        private readonly IAuthenticationService _authService;
        public RoleListViewComponent(ISupportDashboardFactory dDashboardFactory, IWebHelper webHelper, IAuthenticationService authService) {
            _dDashboardFactory = dDashboardFactory;
            _webHelper = webHelper;
            _authService = authService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            RoleGroupListModel model = new();
            //..get user IP address
            var ipAddress = _webHelper.GetCurrentIpAddress();

            //..get current authenticated user record
            var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (grcResponse.HasError) {
               
                return View(model);
            }

            var currentUser = grcResponse.Data;
            currentUser.IPAddress = ipAddress;
            model = await _dDashboardFactory.PrepareRoleGroupListModelAsync(currentUser);
            return View(model);
        }

    }

}
