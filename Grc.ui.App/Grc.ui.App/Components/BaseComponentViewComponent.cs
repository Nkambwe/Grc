using Grc.ui.App.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {

    /// <summary>
    /// Component defines the base layout for different components
    /// </summary>
    public class BaseComponentViewComponent : ViewComponent {

         public async Task<IViewComponentResult> InvokeAsync(PageComponentViewModel model, Task<IHtmlContent> content){ 
             //..component content to pass-in
            ViewData["Content"] = await content;
            return View("Default", model);
         }
    }

}
