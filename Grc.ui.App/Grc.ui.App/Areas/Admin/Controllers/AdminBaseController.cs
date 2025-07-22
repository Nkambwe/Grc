using Grc.ui.App.Infrastructure;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Areas.Admin.Controllers {

    public class AdminBaseController : Controller   {
        protected readonly IApplicationLogger Logger;
        protected readonly IEnvironmentProvider Environment;
        protected readonly IWebHelper WebHelper;
        protected readonly ILocalizationService LocalizationService;
        public AdminBaseController(IApplicationLoggerFactory loggerFactory, 
                                 IEnvironmentProvider environment,
                                 IWebHelper webHelper,
                                 ILocalizationService localizationService) {
            Logger = loggerFactory.CreateLogger("admin_controllers");
            Environment = environment;
            WebHelper = webHelper;
            LocalizationService = localizationService;
        }
    }
}
