using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {

    public class DepartmentListViewComponent : ViewComponent {
        public async Task<IViewComponentResult> InvokeAsync() {
            var content = await Task.FromResult(new { name = "Mark"});
            return View();
        }
    }

}
