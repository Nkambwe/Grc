using Grc.ui.App.Models;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {
    public class RecentViewComponent : ViewComponent  {

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<RecentModel> model){ 
            return View(model);
        }

    }
}
