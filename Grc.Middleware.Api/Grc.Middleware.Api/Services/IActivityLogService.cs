using Grc.Middleware.Api.Data.Entities.Logging;

namespace Grc.Middleware.Api.Services {

    public interface IActivityLogService: IBaseService {
        Task<ActivityLog> GetActivityByIdAsync(int activityId, bool includeMarkedAsDeleted=false);
        Task<bool> InsertActivityAsync(string systemKeyword, string comment, long userId, string ipAddress,object entity = null);
        Task<bool> InsertActivityAsync(string systemKeyword, string comment, long entityId, string entityName, long userId, string ipAddress);
        Task<bool> InsertActivityAsync(ActivityType activityType, string comment, long userId, string ipAddress, object entity = null);
        Task<IList<ActivityLog>> GetAllActivitiesAsync(int pageIndex, int pageSize, bool includeMarkedAsDeleted);
        Task<bool> DeleteActivityAsync(ActivityLog activityLog, bool includeMarkedAsDeleted);
        Task<bool> DeleteActivitiesAsync(IList<ActivityLog> activityLogs, bool includeMarkedAsDeleted);
        Task ClearAllActivitiesAsync();       
        
    }
}
