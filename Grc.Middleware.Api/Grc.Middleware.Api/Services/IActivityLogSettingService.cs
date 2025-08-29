using Grc.Middleware.Api.Data.Entities.Logging;

namespace Grc.Middleware.Api.Services {

    public interface IActivityLogSettingService : IBaseService {
        Task<ActivityLogSetting> GetActivitySettingByKeyAsync(string settingsKey, bool includeMarkedAsDeleted = false);
        Task<List<string>> GetExcludedActivityTypeAsync(string settingKey, bool includeMarkedAsDeleted = false);
        Task<bool> UpdateActivitySettingAsync(ActivityLogSetting activityLogSetting);
        Task<bool> UpdateExcludedActivitiesAsync(List<string> activities, string settingsKey, long userId);
    }
}
