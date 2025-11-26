using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Utils;
using Microsoft.EntityFrameworkCore;

namespace Grc.Middleware.Api.Data.Repositories {

    public class ActivityLogRepository : Repository<ActivityLog>, IActivityLogRepository {

        public ActivityLogRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context) {
        }

        public async Task ClearAllActivitiesAsync(){
            await context.Database.ExecuteSqlRawAsync("DELETE FROM TBL_GRC_ACTIVITY_LOG");
        }
    }

}


