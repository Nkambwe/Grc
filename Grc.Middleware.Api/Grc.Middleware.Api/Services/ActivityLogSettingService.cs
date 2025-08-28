using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Grc.Middleware.Api.Services {
    public class ActivityLogSettingService : BaseService, IActivityLogSettingService {

        public ActivityLogSettingService(IServiceLoggerFactory loggerFactory, 
                                         IUnitOfWorkFactory uowFactory, 
                                         IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
        }

        public async Task<bool> AddDisabledActivityTypeAsync(List<string> activities) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Add disabled activity type", "INFO");
    
            try {
                var activites = await uow.ActivityLogSettingRepository.GetAllAsync();

                //..get first record
                if(activites == null) {
                    return false;
                }
                ActivityLogSetting record = activites.FirstOrDefault();
                record = activites.FirstOrDefault();
                var activityJson = JsonSerializer.Serialize(record, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Settings Object: {activityJson}", "DEBUG");

                //..update activity
                record.DisabledActivityTypes = activities;
                await uow.ActivityLogSettingRepository.UpdateAsync(record, true);

                //..check entity state
                var entityState = ((UnitOfWork)uow).Context.Entry(record).State;
                Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");
        
                var result = await uow.SaveChangesAsync();
                Logger.LogActivity($"Updated result: {result}", "DEBUG");
        
                return result > 0;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update settings: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            }
        }

        public async Task<ActivityLogSetting> GetDefaultAsync() {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve activity settings record", "INFO");
    
            try {
                var activites = await uow.ActivityLogSettingRepository.GetAllAsync();

                //..get first record
                ActivityLogSetting record = null;
                if(activites != null && activites.Any()) {
                    record = activites.FirstOrDefault();
                    var activityJson = JsonSerializer.Serialize(record, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Settings Object: {activityJson}", "DEBUG");
                }
                

                return record;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve settings: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            }
        }
    }
}
