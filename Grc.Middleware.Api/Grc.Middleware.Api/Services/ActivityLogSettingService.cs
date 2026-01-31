using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Utils;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services {
    public class ActivityLogSettingService : BaseService, IActivityLogSettingService {

        public ActivityLogSettingService(IServiceLoggerFactory loggerFactory, 
                                         IUnitOfWorkFactory uowFactory, 
                                         IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
        }

        public async Task<ActivityLogSetting> GetActivitySettingByKeyAsync(string settingsKey, bool includeMarkedAsDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve activity settings record with '{settingsKey}' Param_key", "INFO");
    
            try {;
                var settings = await uow.ActivityLogSettingRepository.GetAsync(t => t.ParameterName == settingsKey, includeMarkedAsDeleted);
                if(settings != null) {
                    //..log the activity settings data being saved
                    var typeJson = JsonSerializer.Serialize(settings, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Activity settings data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"Activity settings with Param_key '{settingsKey}' not found", "DEBUG");
                }

                return settings;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve activity type: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            };
        }

        public async Task<List<string>> GetExcludedActivityTypeAsync(string settingKey, bool includeMarkedAsDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve activity settings record with '{settingKey}' Param_KEY", "INFO");
    
            try {;
                var setting = await uow.ActivityLogSettingRepository.GetAsync(t => t.ParameterName == settingKey, includeMarkedAsDeleted);
                if(setting != null) {
                    //..log the activity settings data being saved
                    var settingJson = JsonSerializer.Serialize(setting, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Activity settings data: {settingJson}", "DEBUG");

                    return JsonSerializer.Deserialize<List<string>>(setting.ParameterValue) ?? new List<string>();
                } else {
                    Logger.LogActivity($"Activity settings with Param_Key '{settingKey}' not found", "DEBUG");
                    return new List<string>();
                }
            
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve activity type: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            };
            
        }

        public async Task<bool> UpdateActivitySettingAsync(ActivityLogSetting activityLogSetting) {
            using var uow = UowFactory.Create();

            try {

                var typeJson = JsonSerializer.Serialize(activityLogSetting, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Activity settings data: {typeJson}", "DEBUG");

                var settings = await uow.ActivityLogSettingRepository.GetAsync(t => t.Id == activityLogSetting.Id);
                if(settings != null){ 
                    //..update activity settings
                    settings.ParameterValue = activityLogSetting.ParameterValue;
                    settings.LastModifiedBy = $"{activityLogSetting.LastModifiedBy}";
                    settings.LastModifiedOn = activityLogSetting.LastModifiedOn;
                    _= await uow.ActivityLogSettingRepository.UpdateAsync(settings);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(settings).State;
                    Logger.LogActivity($"Entity state after Update: {entityState}", "DEBUG");
                   
                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update activity type: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            }
        }

        public async Task<bool> UpdateExcludedActivitiesAsync(List<string> activities, string settingsKey, long userId) {
            using var uow = UowFactory.Create();

            try {

                var valueJson = JsonSerializer.Serialize(activities, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Excluded Activity settings data: {valueJson}", "DEBUG");

                var settings = await uow.ActivityLogSettingRepository.GetAsync(t => t.ParameterName == settingsKey);
                if(settings != null){ 
                    //..update activity settings
                    settings.ParameterValue = valueJson;
                    settings.LastModifiedBy = $"{userId}";
                    settings.LastModifiedOn = DateTime.Now;
                    _= await uow.ActivityLogSettingRepository.UpdateAsync(settings);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(settings).State;
                    Logger.LogActivity($"Entity state after Update: {entityState}", "DEBUG");
                   
                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update activity type: {ex.Message}", "ERROR");
        
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
