using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class PermissionSetRepository : Repository<SystemPermissionSet>, IPermissionSetRepository
    {
        public PermissionSetRepository(IServiceLoggerFactory loggerFactory, GrcContext _context) : base(loggerFactory, _context)
        {
        }


    }
}


