using AutoMapper;
using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Services;
using Grc.Middleware.Api.Services.Organization;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.Middleware.Api.Controllers
{
    [ApiController]
    [Route("grc")]
    public class GrcSharedController : GrcControllerBase
    {
        public GrcSharedController(IObjectCypher cypher,
            IServiceLoggerFactory loggerFactory,
            IMapper mapper,
            ICompanyService companyService,
            IEnvironmentProvider environment,
            IErrorNotificationService errorService,
            ISystemErrorService systemErrorService)
            : base(cypher, loggerFactory, mapper, companyService, environment, errorService, systemErrorService)
        {
        }
    }
}
