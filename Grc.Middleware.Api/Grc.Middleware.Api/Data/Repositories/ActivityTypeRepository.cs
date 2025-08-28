using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class ActivityTypeRepository : Repository<ActivityType>, IActivityTypeRepository {

        public ActivityTypeRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context) {
        }

    }

}
