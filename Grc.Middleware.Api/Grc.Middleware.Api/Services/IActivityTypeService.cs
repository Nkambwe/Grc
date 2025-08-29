using Grc.Middleware.Api.Data.Entities.Logging;

namespace Grc.Middleware.Api.Services {

    public interface IActivityTypeService:IBaseService {
        Task<IList<ActivityType>> GetAllActivityTypesAsync(bool includeMarkedAsDeleted = false);
        
        Task<ActivityType> GetActivityTypeByIdAsync(int activityTypeId, bool includeMarkedAsDeleted=false);
        
        Task<ActivityType> GetActivityTypeBySystemKeywordAsync(string systemKeyword, bool includeMarkedAsDeleted = false);
        
        Task<bool> InsertActivityTypeAsync(ActivityType activityType);
        
        Task<bool> UpdateActivityTypeAsync(ActivityType activityType);
        
        Task<bool> DeleteActivityTypeAsync(ActivityType activityType, bool markAsDeleted=false);
    }
}
