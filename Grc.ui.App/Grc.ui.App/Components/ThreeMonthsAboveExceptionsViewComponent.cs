using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {

    public class ThreeMonthsAboveExceptionsViewComponent : ViewComponent {
        public async Task<IViewComponentResult> InvokeAsync() {

            return View();
        }
    }
}
