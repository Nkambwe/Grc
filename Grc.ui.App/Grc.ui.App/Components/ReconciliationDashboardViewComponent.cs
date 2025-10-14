using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components
{
    public class ReconciliationDashboardViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
