using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.Middleware.Api.Controllers {

    [ApiController]
    [Route("grc")]
    public class GrcController : GrcControllerBase {

        public GrcController(IServiceLoggerFactory loggerFactory, IEnvironmentProvider environment) 
            : base(loggerFactory, environment) {
        }

        [HttpGet]
        public IActionResult Index() { 
            return Ok("Welcome to GRC Suite! ");    
        }
    }
}
