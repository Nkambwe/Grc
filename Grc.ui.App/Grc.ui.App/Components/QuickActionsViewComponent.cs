using Grc.ui.App.Factories;
using Grc.ui.App.Models;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {
    public class QuickActionsViewComponent : ViewComponent  {
        private readonly IQuickActionModelFactory FactoryModel;
        public QuickActionsViewComponent(IQuickActionModelFactory modelFactory) {
            FactoryModel = modelFactory;
        }
        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<QuickActionModel> model){ 
            FactoryModel.QuickActionItems = model;
           
            return View(FactoryModel);
        }
    }
}
