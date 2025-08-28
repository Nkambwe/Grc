using Grc.Middleware.Api.Data.Entities.Logging;

namespace Grc.Middleware.Api.Services {

    public interface IActivityLogSettingService : IBaseService {
        Task<ActivityLogSetting> GetDefaultAsync();

        Task<bool> AddDisabledActivityTypeAsync(List<string> activities);
    }
}
