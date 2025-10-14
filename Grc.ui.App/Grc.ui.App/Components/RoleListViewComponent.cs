using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {

    public class RoleListViewComponent : ViewComponent {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }

    }

}
