using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class ActivityLogSettingRepository : Repository<ActivityLogSetting>, IActivityLogSettingRepository {

        public ActivityLogSettingRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context) {
        }

    }

}
