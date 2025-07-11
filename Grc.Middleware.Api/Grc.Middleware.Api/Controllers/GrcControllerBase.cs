using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.Middleware.Api.Controllers {

    public class GrcControllerBase : ControllerBase  {

        protected readonly IServiceLogger Logger;

        protected readonly IEnvironmentProvider Environment;

        public GrcControllerBase(IServiceLoggerFactory loggerFactory, IEnvironmentProvider environment) { 
            Logger = loggerFactory.CreateLogger();
            Environment = environment;
        }
    }
}
