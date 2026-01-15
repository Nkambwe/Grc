using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {
    public class AuditTypesListViewComponent : ViewComponent {
        public async Task<IViewComponentResult> InvokeAsync() {
            return View();
        }
    }
}
