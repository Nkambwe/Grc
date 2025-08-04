using Grc.ui.App.Models;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {
    public class SidebarViewComponent : ViewComponent  {

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<MenuItemModel> model){ 
            return View(model);
        }
    }
}
