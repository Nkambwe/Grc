using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components
{
    public class PasswordConfigViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
