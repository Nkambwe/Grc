using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class UserRepository : Repository<SystemUser>, IUserRepository {
        public UserRepository(IServiceLoggerFactory loggerFactory, 
            GrcContext _context) : base(loggerFactory, _context) {
        }
    }
}
