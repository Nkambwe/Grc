using Grc.Middleware.Api.Data.Entities.Logging;

namespace Grc.Middleware.Api.Services.Operations {

    public interface IActivityTypeService:IBaseService {
        Task<IList<KeyValuePair<string, string>>> GetSystemKeyWordsAsync();

        Task<IList<ActivityType>> GetAllActivityTypesAsync(bool includeMarkedAsDeleted = false);
        
        Task<ActivityType> GetActivityTypeByIdAsync(long activityTypeId, bool includeMarkedAsDeleted=false);

        Task<ActivityType> GetActivityTypeByNameAsync(string typeName, bool includeMarkedAsDeleted=false);
        
        Task<ActivityType> GetActivityTypeBySystemKeywordAsync(string systemKeyword, bool includeMarkedAsDeleted = false);
        
        Task<bool> InsertActivityTypeAsync(ActivityType activityType);
        
        Task<bool> UpdateActivityTypeAsync(ActivityType activityType);
        
        Task<bool> DeleteActivityTypeAsync(ActivityType activityType, bool markAsDeleted=false);
    }
}
