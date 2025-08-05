using Grc.ui.App.Factories;
using Grc.ui.App.Models;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {

    public class RecentViewComponent : ViewComponent {
        private readonly IReceientModelFactory FactoryModel;
    
        public RecentViewComponent(IReceientModelFactory recentModelFactory) {
            FactoryModel = recentModelFactory;
        }
    
        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<RecentModel> model) { 
            FactoryModel.RecentItems = model;
           
            return View(FactoryModel);
        }
    }

}
