using Grc.Middleware.Api.Data.Entities.Logging;

namespace Grc.Middleware.Api.Services {

    public interface IActivityLogService: IBaseService {
        Task<bool> InsertActivityAsync(string systemKeyword, string comment, object entity = null);
        Task<bool> InsertActivityAsync(string systemKeyword, string comment, int entityId, string entityName);
        Task<bool> InsertActivityAsync(ActivityType activityType, string comment, object entity = null);
        
        Task<IList<ActivityLog>> GetAllActivitiesAsync(DateTime? createdFrom = null, DateTime? createdTo = null, int? 
                            userId = null, int? activityTypeId = null, string ipAddress = null, string entityName = null, 
                            int pageIndex = 0, int pageSize = int.MaxValue);
            
        Task<ActivityLog> GetActivityByIdAsync(int activityId);
        
        Task<bool> DeleteActivityAsync(ActivityLog activityLog);
        
        Task<bool> DeleteActivitiesAsync(IList<ActivityLog> activityLogs);
        
        Task<bool> ClearAllActivitiesAsync();
        
        Task<IList<ActivityType>> GetAllActivityTypesAsync();
        
        Task<ActivityType> GetActivityTypeByIdAsync(int activityTypeId);
        
        Task<ActivityType> GetActivityTypeBySystemKeywordAsync(string systemKeyword);
        
        Task<bool> InsertActivityTypeAsync(ActivityType activityType);
        
        Task<bool> UpdateActivityTypeAsync(ActivityType activityType);
        
        Task<bool> DeleteActivityTypeAsync(ActivityType activityType);
    }
}
