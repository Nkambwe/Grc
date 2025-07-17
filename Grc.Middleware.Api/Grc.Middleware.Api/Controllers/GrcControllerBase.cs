using AutoMapper;
using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.Middleware.Api.Controllers {

    public class GrcControllerBase : ControllerBase  {
        protected readonly IObjectCypher Cypher;
        protected readonly IServiceLogger Logger;
        protected readonly IMapper Mapper;
        protected readonly IEnvironmentProvider Environment;

        public GrcControllerBase(IObjectCypher cypher,
            IServiceLoggerFactory loggerFactory, 
                                 IMapper mapper,
                                 IEnvironmentProvider environment) { 
            Logger = loggerFactory.CreateLogger();
            Mapper = mapper;
            Cypher = cypher;
            Environment = environment;
        }
    }
}
