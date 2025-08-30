using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Helpers;

namespace Grc.Middleware.Api.Services {

    public interface IActivityLogService: IBaseService {
        Task<ActivityLog> GetActivityByIdAsync(int activityId, bool includeMarkedAsDeleted=false);
        Task<bool> InsertActivityAsync(string systemKeyword, string comment, long userId, string ipAddress,object entity = null);
        Task<bool> InsertActivityAsync(string systemKeyword, string comment, long entityId, string entityName, long userId, string ipAddress);
        Task<bool> InsertActivityAsync(ActivityType activityType, string comment, long userId, string ipAddress, object entity = null);
        Task<IList<ActivityLog>> GetAllActivitiesAsync(int pageIndex, int pageSize, bool includeMarkedAsDeleted);

        Task<IList<ActivityLog>> GetAllActivitiesAsync(DateTime? createdFrom = null, DateTime? createdTo = null, long? userId = null, long? activityTypeId = null,
            string ipAddress = null, string entityName = null, int pageIndex = 0, int pageSize = 5, bool includeDeleted = false); 
            
        Task<PagedResult<ActivityLog>> GetPagedActivitiesAsync(DateTime? createdFrom = null, DateTime? createdTo = null, long? userId = null, long? activityTypeId = null,
            string ipAddress = null, string entityName = null, int pageIndex = 1, int pageSize = 5, bool includeDeleted = false); 
        Task<bool> DeleteActivityAsync(ActivityLog activityLog, bool includeMarkedAsDeleted);
        Task<bool> DeleteActivitiesAsync(IList<ActivityLog> activityLogs, bool includeMarkedAsDeleted);
        Task ClearAllActivitiesAsync();       
        
    }
}
