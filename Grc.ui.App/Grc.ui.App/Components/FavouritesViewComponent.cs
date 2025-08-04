using Grc.ui.App.Models;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Components {
    public class FavouritesViewComponent : ViewComponent  {

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<FavouriteModel> model){ 
            return View(model);
        }

    }
}
