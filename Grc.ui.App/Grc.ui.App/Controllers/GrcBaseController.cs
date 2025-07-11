using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Controllers {

    public class GrcBaseController : Controller  {

        protected readonly IApplicationLogger Logger;
        protected readonly IEnvironmentProvider Environment;

        public GrcBaseController(IApplicationLoggerFactory loggerFactory, IEnvironmentProvider environment) {
            Logger = loggerFactory.CreateLogger("app_controllers");
            Environment = environment;
        }
    }

}
