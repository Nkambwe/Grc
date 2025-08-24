using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class UserPreferenceRepository : Repository<UserPrefference>, IUserPreferenceRepository {
        public UserPreferenceRepository(IServiceLoggerFactory loggerFactory, GrcContext _context) 
            : base(loggerFactory, _context) {
        }
    }
}
