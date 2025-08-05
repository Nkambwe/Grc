using Grc.ui.App.Factories;
using Grc.ui.App.Models;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {
    public class PinnedViewComponent : ViewComponent  {
        private readonly IPinnedModelFactory FactoryModel;
        public PinnedViewComponent(IPinnedModelFactory modelFactory) {
            FactoryModel = modelFactory;
        }
    
        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<PinnedModel> model){ 
             FactoryModel.PinnedItems = model;
           
            return View(FactoryModel);
        }

    }
}
