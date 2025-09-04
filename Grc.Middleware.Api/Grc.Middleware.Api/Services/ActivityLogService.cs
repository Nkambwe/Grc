using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;
using Grc.Middleware.Api.Enums;
using Microsoft.EntityFrameworkCore;
using Grc.Middleware.Api.Helpers;

namespace Grc.Middleware.Api.Services {

    public class ActivityLogService : BaseService, IActivityLogService {

        private readonly IActivityLogSettingService _activityLogSettings;

        public ActivityLogService(IServiceLoggerFactory loggerFactory, 
                                  IUnitOfWorkFactory uowFactory, 
                                  IMapper mapper,
                                  IActivityLogSettingService activityLogSettings) 
                                : base(loggerFactory, uowFactory, mapper) {
            _activityLogSettings = activityLogSettings;
        }
        
        public async Task<ActivityLog> GetActivityByIdAsync(int activityId, bool includeMarkedAsDeleted=false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve activity log record with '{activityId}' ID", "INFO");
    
            try {;
                var activity = await uow.ActivityLogRepository.GetAsync(t => t.Id == activityId, includeMarkedAsDeleted);
                if(activity != null) {
                    //..log the activity log data being saved
                    var typeJson = JsonSerializer.Serialize(activity, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Activity log data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"Activity log with ID '{activityId}' not found", "DEBUG");
                }

                return activity;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve activity log: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            };
        }
        
        public async Task<IList<ActivityLog>> GetAllActivitiesAsync(int pageIndex, int pageSize, bool includeMarkedAsDeleted) {
            
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all activity settings", "INFO");
    
            try {;

                int page = pageIndex > 0 ? pageIndex : 1;
                int size = pageSize > 0 ? pageSize : 10;

               var pagedResult = await uow.ActivityLogRepository.PageAllAsync(page, size, includeMarkedAsDeleted, x => x.User, x => x.ActivityType);
                if(pagedResult != null) {
                    var result = pagedResult.Entities;

                    if(result != null && result.Any()) {
                        //..log the activity log data being saved
                        var typeJson = JsonSerializer.Serialize(result, new JsonSerializerOptions { 
                            WriteIndented = true,
                            ReferenceHandler = ReferenceHandler.IgnoreCycles 
                        });
                        Logger.LogActivity($"Activity types data: {typeJson}", "DEBUG");
                    } else {
                        result = new List<ActivityLog>();
                    }

                    return result;
                } else {
                    Logger.LogActivity($"No Activity logs found", "DEBUG");
                    return new List<ActivityLog>();
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve activity types: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            };
        }
        
        public async Task<IList<ActivityLog>> GetAllActivitiesAsync(DateTime? createdFrom = null, DateTime? createdTo = null,
            long? userId = null, long? activityTypeId = null,
            string ipAddress = null, string entityName = null, int pageIndex = 0, 
            int pageSize = 6, bool includeDeleted = false) {
            
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all log settings", "INFO");
    
            try {

                int page = pageIndex > 0 ? pageIndex : 1;
                var query = uow.ActivityLogRepository.GetAll(
                    includeDeleted, 
                    t => t.ActivityType, 
                    t => t.User);

                if (createdFrom.HasValue)
                query = query.Where(x => x.CreatedOn >= createdFrom.Value);

                if (createdTo.HasValue)
                    query = query.Where(x => x.CreatedOn <= createdTo.Value);

                if (userId.HasValue)
                    query = query.Where(x => x.UserId == userId.Value);

                if (activityTypeId.HasValue)
                    query = query.Where(x => x.TypeId == activityTypeId.Value);

                if (!string.IsNullOrEmpty(ipAddress))
                    query = query.Where(x => x.IpAddress == ipAddress);

                if (!string.IsNullOrEmpty(entityName))
                    query = query.Where(x => x.EntityName == entityName);

                query = query.OrderByDescending(x => x.CreatedOn);

                return await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve activity log: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            };
        }

        public async Task<PagedResult<ActivityLog>> GetPagedActivitiesAsync(DateTime? createdFrom = null, DateTime? createdTo = null, long? userId = null, long? activityTypeId = null,
            string ipAddress = null, string entityName = null, int pageIndex = 1, int pageSize = 7, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all log settings", "INFO");
    
            try {
                //...build query first
                var query = uow.ActivityLogRepository.GetAll(includeDeleted, a => a.ActivityType, a => a.User)
                    .AsQueryable(); 
        
                //..apply filters
                var filteredQuery = query.Where(a => 
                    (!createdFrom.HasValue || a.CreatedOn >= createdFrom.Value) &&
                    (!createdTo.HasValue || a.CreatedOn <= createdTo.Value) &&
                    (!userId.HasValue || a.UserId == userId.Value) &&
                    (!activityTypeId.HasValue || a.TypeId == activityTypeId.Value) &&
                    (string.IsNullOrEmpty(ipAddress) || a.IpAddress == ipAddress) &&
                    (string.IsNullOrEmpty(entityName) || a.EntityName == entityName)
                );
        
                var orderedQuery = filteredQuery.OrderByDescending(x => x.CreatedOn);
        
                //..execute query
                var totalRecords = await orderedQuery.CountAsync();
                var entities = await orderedQuery
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
        
                return new PagedResult<ActivityLog>  {
                    Entities = entities,
                    Count = totalRecords,
                    Page = pageIndex,
                    Size = pageSize
                };
            } catch (Exception ex)  {
                Logger.LogActivity($"Failed to retrieve activity logs: {ex.Message}", "ERROR");
                throw;
            }
        }

        public async Task<bool> InsertActivityAsync(string systemKeyword, string comment, long entityId, string entityName, long userId, string ipAddress) {
             using var uow = UowFactory.Create();
             Logger.LogActivity("Check if activity can be logged", "INFO");

             try {
                //..check if logging is enabled
                var setting = await _activityLogSettings.GetActivitySettingByKeyAsync(ActivityTypeKey.ENABLELOGGING.ToString());
                if (setting == null) { 
                    return false;
                }

                if(!bool.TryParse(setting.ParameterValue, out bool enableLogging)) {
                    return false;
                }

                if (!enableLogging)
                    return false;
                
                //..check if type is excluded from logging
                var excluded = await _activityLogSettings.GetExcludedActivityTypeAsync(ActivityTypeKey.EXCLUDEDACTIVITITIES.ToString());
                if (excluded.Contains(systemKeyword))
                    return false;
                
                //..check if type can be logged
                var activityType = await uow.ActivityTypeRepository.GetAsync(t => t.SystemKeyword == systemKeyword);
                if (activityType == null || !activityType.Enabled)
                    return false;

                //..create activity record
                var activity = new ActivityLog {
                    TypeId = activityType.Id,
                    UserId = userId,
                    Comment = comment,
                    EntityName = entityName,
                    EntityId = entityId,
                    CreatedOn = DateTime.UtcNow,
                    IpAddress = ipAddress
                };
                //..log the activity data being saved
                var activityJson = JsonSerializer.Serialize(activity, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Activity log data: {activityJson}", "DEBUG");
        
                await uow.ActivityLogRepository.InsertAsync(activity);

                //..check entity state
                var entityState = ((UnitOfWork)uow).Context.Entry(activity).State;
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

        public async Task<bool> InsertActivityAsync(string systemKeyword, string comment, long userId, string ipAddress,object entity = null) {

            using var uow = UowFactory.Create();
            Logger.LogActivity("Check if activity can be logged", "INFO");

             try {
                //..check if logging is enabled
                var setting = await _activityLogSettings.GetActivitySettingByKeyAsync(ActivityTypeKey.ENABLELOGGING.ToString());
                if (setting == null) { 
                    return false;
                }

                if(!bool.TryParse(setting.ParameterValue, out bool enableLogging)) {
                    return false;
                }

                if (!enableLogging)
                    return false;
                
                //..check if type is excluded from logging
                var excluded = await _activityLogSettings.GetExcludedActivityTypeAsync(ActivityTypeKey.EXCLUDEDACTIVITITIES.ToString());
                if (excluded.Contains(systemKeyword))
                    return false;
                
                //..check if type can be logged
                var activityType = await uow.ActivityTypeRepository.GetAsync(t => t.SystemKeyword == systemKeyword);
                if (activityType == null || !activityType.Enabled)
                    return false;

                return await InsertActivityAsync(activityType, comment, userId, ipAddress, entity);
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

        public async Task<bool> InsertActivityAsync(ActivityType activityType, string comment, long userId, string ipAddress,object entity = null) {
            using var uow = UowFactory.Create();
             Logger.LogActivity("Check if activity can be logged", "INFO");

             try {
                //..check if logging is enabled
                var setting = await _activityLogSettings.GetActivitySettingByKeyAsync(ActivityTypeKey.ENABLELOGGING.ToString());
                if (setting == null) { 
                    return false;
                }

                if(!bool.TryParse(setting.ParameterValue, out bool enableLogging)) {
                    return false;
                }

                if (!enableLogging)
                    return false;
                
                //..check if type can be logged
                if (activityType == null || !activityType.Enabled)
                    return false;

                //..create activity record
                var activity = new ActivityLog {
                    TypeId = activityType.Id,
                    UserId = userId,
                    Comment = comment,
                    IsDeleted = false,
                    EntityName = entity?.ToString(),
                    CreatedOn = DateTime.Now,
                    CreatedBy = $"{userId}",
                    IpAddress = ipAddress,
                    LastModifiedOn = DateTime.Now,
                    LastModifiedBy= $"{userId}"
                };

                //..log the activity data being saved
                var activityJson = JsonSerializer.Serialize(activity, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Activity log data: {activityJson}", "DEBUG");
        
                await uow.ActivityLogRepository.InsertAsync(activity);

                //..check entity state
                var entityState = ((UnitOfWork)uow).Context.Entry(activity).State;
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
        
        public async Task<bool> DeleteActivityAsync(ActivityLog activityLog, bool markAsDeleted = false) {
            using var uow = UowFactory.Create();

            try {

                var activityJson = JsonSerializer.Serialize(activityLog, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Activity log data: {activityJson}", "DEBUG");

                var activity = await uow.ActivityLogRepository.GetAsync(t => t.Id == activityLog.Id);
                if(activity != null){ 
                    //..mark as delete activity log
                    _= await uow.ActivityLogRepository.DeleteAsync(activity, markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(activity).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");
                   
                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update activity log: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            }
        }
        
        public async Task<bool> DeleteActivitiesAsync(IList<ActivityLog> activityLogs, bool markAsDeleted = false) {
            if (activityLogs == null || activityLogs.Count == 0)
                return false;

            using var uow = UowFactory.Create();

            try {
                //..log all activities being deleted
                var activitiesJson = JsonSerializer.Serialize(activityLogs, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Activity logs data (batch delete): {activitiesJson}", "DEBUG");

                //..fetch the actual entities from DB
                var ids = activityLogs.Select(a => a.Id).ToList();
                var activities = await uow.ActivityLogRepository.GetAllAsync(a => ids.Contains(a.Id), markAsDeleted);
                if (activities == null || !activities.Any())
                    return false;

                //..perform delete (soft or hard)
                _ = await uow.ActivityLogRepository.DeleteAllAsync(activities.ToList(), markAsDeleted);

                //..debug: check entity states after deletion
                foreach (var activity in activities) {
                    var entityState = ((UnitOfWork)uow).Context.Entry(activity).State;
                    Logger.LogActivity($"Entity state after deletion (Id={activity.Id}): {entityState}", "DEBUG");
                }

                // Save once
                var result = await uow.SaveChangesAsync();
                Logger.LogActivity($"SaveChanges result (batch delete): {result}", "DEBUG");

                return result > 0;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to batch delete activity logs: {ex.Message}", "ERROR");

                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                throw;
            }
        }

        public async Task ClearAllActivitiesAsync() {
            using var uow = UowFactory.Create();

            try {
                Logger.LogActivity($"Delete all activities", "DEBUG");
                await uow.ActivityLogRepository.ClearAllActivitiesAsync();
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to clear all activities: {ex.Message}", "ERROR");
        
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
