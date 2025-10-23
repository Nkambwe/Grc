using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;
using Grc.Middleware.Api.Defaults;

namespace Grc.Middleware.Api.Services {

    public class ActivityTypeService : BaseService, IActivityTypeService {

        public ActivityTypeService(IServiceLoggerFactory loggerFactory, 
                                   IUnitOfWorkFactory uowFactory, 
                                   IMapper mapper)
                                   : base(loggerFactory, uowFactory, mapper) {
        }

        public async Task<IList<KeyValuePair<string, string>>> GetSystemKeyWordsAsync() {
            var fields = typeof(ActivityTypeDefaults)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            var result = fields
                .Where(f => f.IsLiteral && !f.IsInitOnly) 
                .Select(f => new KeyValuePair<string, string>(
                    f.Name,
                    f.GetRawConstantValue()?.ToString() ?? string.Empty
                ))
                .ToList();

            return await Task.FromResult(result);
        }

        public async Task<ActivityType> GetActivityTypeByIdAsync(long activityTypeId, bool includeMarkedAsDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve activity type record with '{activityTypeId}' ID", "INFO");
    
            try {;
                var type = await uow.ActivityTypeRepository.GetAsync(t => t.Id == activityTypeId, includeMarkedAsDeleted);
                if(type != null) {
                    //..log the activity type data being saved
                    var typeJson = JsonSerializer.Serialize(type, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Activity type data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"Activity type with ID '{activityTypeId}' not found", "DEBUG");
                }

                return type;
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

        public async Task<ActivityType> GetActivityTypeByNameAsync(string typeName, bool includeMarkedAsDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve activity type record with '{typeName}' type name", "INFO");
    
            try {;
                var type = await uow.ActivityTypeRepository.GetAsync(t => t.Name == typeName, includeMarkedAsDeleted);
                if(type != null) {
                    //..log the activity type data being saved
                    var typeJson = JsonSerializer.Serialize(type, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Activity type data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"Activity type with name '{typeName}' not found", "DEBUG");
                }

                return type;
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

        public async Task<ActivityType> GetActivityTypeBySystemKeywordAsync(string systemKeyword, bool includeMarkedAsDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve activity type record with '{systemKeyword}' system key word", "INFO");
    
            try {;
                var type = await uow.ActivityTypeRepository.GetAsync(t => t.SystemKeyword == systemKeyword, includeMarkedAsDeleted);
                if(type != null) {
                    //..log the activity type data being saved
                    var typeJson = JsonSerializer.Serialize(type, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Activity type data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"Activity type with systemKeyword '{systemKeyword}' not found", "DEBUG");
                }

                return type;
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

        public async Task<IList<ActivityType>> GetAllActivityTypesAsync(bool includeMarkedAsDeleted = false) {
            
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all activity types", "INFO");
    
            try {;
                var types = await uow.ActivityTypeRepository.GetAllAsync( includeMarkedAsDeleted);
                if(types != null && types.Any()) {
                    //..log the activity type data being saved
                    var typeJson = JsonSerializer.Serialize(types, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Activity types data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"Activity types found", "DEBUG");
                }

                return types;
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
        
        public async Task<bool> InsertActivityTypeAsync(ActivityType activityType) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Save new activity type >>>>", "INFO");
    
            try {
                //..log the activity type data being saved
                var typeJson = JsonSerializer.Serialize(activityType, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"ActivityType data: {typeJson}", "DEBUG");
        
                await uow.ActivityTypeRepository.InsertAsync(activityType);

                //..check entity state
                var entityState = ((UnitOfWork)uow).Context.Entry(activityType).State;
                Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");
        
                var result = await uow.SaveChangesAsync();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
        
                return result > 0;
            } catch (Exception ex) {
                Logger.LogActivity($"CreateCompanyAsync failed: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
        
                //..re-throw to the controller handle
                throw; 
            }
        }

        public async Task<bool> UpdateActivityTypeAsync(ActivityType activityType) {
            using var uow = UowFactory.Create();

            try {

                var typeJson = JsonSerializer.Serialize(activityType, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Activity Type data: {typeJson}", "DEBUG");

                var type = await uow.ActivityTypeRepository.GetAsync(t => t.Id == activityType.Id);
                if(type != null){ 
                    //..update activity type
                    type.Name = activityType.Name;
                    type.SystemKeyword = activityType.SystemKeyword;
                    type.Description = activityType.Description;
                    type.Enabled = activityType.Enabled;
                    type.Category = activityType.Category;
                    type.IsSupportActivity = activityType.IsSupportActivity;
                    type.LastModifiedBy = $"{activityType.LastModifiedBy}";
                    type.LastModifiedOn = activityType.LastModifiedOn;
                    _= await uow.ActivityTypeRepository.UpdateAsync(type);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(type).State;
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

        public async Task<bool> DeleteActivityTypeAsync(ActivityType activityType, bool markAsDeleted = false) {
            using var uow = UowFactory.Create();

            try {

                var typeJson = JsonSerializer.Serialize(activityType, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Activity Type data: {typeJson}", "DEBUG");

                var type = await uow.ActivityTypeRepository.GetAsync(t => t.Id == activityType.Id);
                if(type != null){ 
                    //..mark as delete activity type
                    _= await uow.ActivityTypeRepository.DeleteAsync(type, markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(type).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");
                   
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
