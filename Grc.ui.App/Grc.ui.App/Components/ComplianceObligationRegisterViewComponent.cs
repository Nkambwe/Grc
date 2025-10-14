using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components
{
    public class ComplianceObligationRegisterViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
