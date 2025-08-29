using Grc.Middleware.Api.Data.Entities.Logging;

namespace Grc.Middleware.Api.Data.Repositories {

    public interface IActivityLogRepository : IRepository<ActivityLog> {
        Task ClearAllActivitiesAsync();
    }

}
