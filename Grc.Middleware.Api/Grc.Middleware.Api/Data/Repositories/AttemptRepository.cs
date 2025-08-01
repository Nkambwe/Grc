using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class AttemptRepository : Repository<LoginAttempt>, IAttemptRepository {
        public AttemptRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context) {
        }
    }
}
