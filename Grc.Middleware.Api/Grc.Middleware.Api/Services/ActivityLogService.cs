using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;
using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Services {

    public class ActivityLogService : BaseService, IActivityLogService {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActivityLogSettingService _activityLogSettings;

        public ActivityLogService(IServiceLoggerFactory loggerFactory, 
                                  IUnitOfWorkFactory uowFactory, 
                                  IMapper mapper,
                                  IActivityLogSettingService activityLogSettings) 
                                : base(loggerFactory, uowFactory, mapper) {
            _activityLogSettings = activityLogSettings;
        }

        public async Task<bool> ClearAllActivitiesAsync() {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteActivitiesAsync(IList<ActivityLog> activityLogs) {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteActivityAsync(ActivityLog activityLog) {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteActivityTypeAsync(ActivityType activityType) {
            throw new NotImplementedException();
        }

        public async Task<ActivityLog> GetActivityByIdAsync(int activityId) {
            throw new NotImplementedException();
        }

        public async Task<ActivityType> GetActivityTypeByIdAsync(int activityTypeId) {
            throw new NotImplementedException();
        }

        public async Task<ActivityType> GetActivityTypeBySystemKeywordAsync(string systemKeyword) {
            throw new NotImplementedException();
        }

        public async Task<IList<ActivityLog>> GetAllActivitiesAsync(DateTime? createdFrom = null, DateTime? createdTo = null, 
            int? userId = null, int? activityTypeId = null, string ipAddress = null, string entityName = null, int pageIndex = 0,
            int pageSize = int.MaxValue) {
            throw new NotImplementedException();
        }

        public async Task<IList<ActivityType>> GetAllActivityTypesAsync() {
            throw new NotImplementedException();
        }

        public async Task<bool> InsertActivityAsync(string systemKeyword, string comment, object entity = null) {

            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve user by email", "INFO");

             try {
                var settings = await _activityLogSettings.GetDefaultAsync();
                if (settings == null) { 
                    return false;
                }

                if (!settings.EnableLogging)
                    return false;

                if (settings.DisabledActivityTypes.Contains(systemKeyword))
                    return false;

                //..check if type can be logged
                var activityType = await GetActivityTypeBySystemKeywordAsync(systemKeyword);
                if (activityType == null || !activityType.Enabled)
                    return false;

                //..save activity log
                var activityLog = new ActivityLog();
                //TODO change parameters to activity object

                await InsertActivityAsync(activityType, comment, entity);

                //..check entity state
                var entityState = ((UnitOfWork)uow).Context.Entry(activityLog).State;
                Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");
        
                var result = await uow.SaveChangesAsync();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
        
                return result > 0;

             } catch (Exception ex) {
                Logger.LogActivity($"Failed to save activity log: {ex.Message}", "ERROR");
                //TODO -- save exception
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
             }

        }

        public async Task<bool> InsertActivityAsync(string systemKeyword, string comment, int entityId, string entityName) {
            throw new NotImplementedException();
        }

        public async Task<bool> InsertActivityAsync(ActivityType activityType, string comment, object entity = null) {
            throw new NotImplementedException();
        }

        public async Task<bool> InsertActivityTypeAsync(ActivityType activityType) {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateActivityTypeAsync(ActivityType activityType) {
            throw new NotImplementedException();
        }
    }
}
