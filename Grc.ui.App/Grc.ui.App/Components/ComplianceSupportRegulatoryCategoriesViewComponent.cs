using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components
{
    public class ComplianceSupportRegulatoryCategoriesViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
