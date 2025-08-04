using Grc.ui.App.Models;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {
    public class QuickActionsViewComponent : ViewComponent  {

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<QuickActionModel> model){ 
            return View(model);
        }
    }
}
