using Grc.ui.App.Factories;
using Grc.ui.App.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {

    public class PopupPageViewComponent : ViewComponent {

        private readonly IPageModelFactory _modelFactory;
        public PopupPageViewComponent(IPageModelFactory modelFactory) {
            _modelFactory = modelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string title, string name, Task<IHtmlContent> content, int level = 0) {
            PopupModel model = await _modelFactory.PreparePoupModelAsync(title, name, level, await content);
            return View("Default",model);
        }

    }
}
