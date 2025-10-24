using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class SystemConfigurationRepository : Repository<SystemConfiguration>, ISystemConfigurationRepository
    {

        public SystemConfigurationRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context)
        {
        }

    }


}


