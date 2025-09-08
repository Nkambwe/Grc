using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {

    public class PopupPageViewComponent : ViewComponent {
        public async Task<IViewComponentResult> InvokeAsync(string title, Task<IHtmlContent> content, int level = 0) {
            ViewData["Content"] = await content;
            ViewData["Level"] = level; 
            ViewData["Title"] = title;
            return View("Default");
        }

    }
}
