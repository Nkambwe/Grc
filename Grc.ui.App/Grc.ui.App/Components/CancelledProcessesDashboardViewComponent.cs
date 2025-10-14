using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components
{
    public class CancelledProcessesDashboardViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
