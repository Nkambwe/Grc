using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Grc.Middleware.Api.Services {
    public class DepartmentUnitService : BaseService, IDepartmentUnitService {
        public DepartmentUnitService(IServiceLoggerFactory loggerFactory, 
                                    IUnitOfWorkFactory uowFactory, 
                                    IMapper mapper) 
                                    : base(loggerFactory, uowFactory, mapper) {
        }
        
        public async Task<DepartmentUnit> GetUnitByIdAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve department unit record with '{id}' ID", "INFO");
    
            try {;
                var department = await uow.DepartmentUnitRepository.GetAsync(t => t.Id == id, includeDeleted);
                if(department != null) {
                    //..log the Department unit data being retrieved
                    var typeJson = JsonSerializer.Serialize(department, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Department unit data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"Department unit with ID '{id}' not found", "DEBUG");
                }

                return department;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Department unit: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            };
        }
        
        public async Task<DepartmentUnit> GetUnitByCodeAsync(string code, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve department unit record with '{code}' code", "INFO");
    
            try {;
                var department = await uow.DepartmentUnitRepository.GetAsync(t => t.UnitCode == code, includeDeleted);
                if(department != null) {
                    //..log the Department unit data being retrieved
                    var typeJson = JsonSerializer.Serialize(department, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Department unit data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"Department unit with code '{code}' not found", "DEBUG");
                }

                return department;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Department unit : {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            };
        }

        public async Task<DepartmentUnit> GetUnitByNameAsync(string name, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve department unit record with '{name}' name", "INFO");
    
            try {;
                var department = await uow.DepartmentUnitRepository.GetAsync(t => t.UnitName == name, includeDeleted);
                if(department != null) {
                    //..log the Department unit data being retrieved
                    var typeJson = JsonSerializer.Serialize(department, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Department unit data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"Department unit with name '{name}' not found", "DEBUG");
                }

                return department;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Department unit: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            };
        }

        public async Task<PagedResult<DepartmentUnit>> GetPagedUnitsAsync(DateTime? createdFrom = null, DateTime? createdTo = null, long? userId = null, int pageIndex = 1, int pageSize = 20, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all department units", "INFO");
    
            try {
                //...build query first
                var query = uow.DepartmentUnitRepository.GetAll(includeDeleted, d => d.Department)
                    .AsQueryable(); 
        
                //..apply filters
                var filteredQuery = query.Where(a => 
                    (!createdFrom.HasValue || a.CreatedOn >= createdFrom.Value) &&
                    (!createdTo.HasValue || a.CreatedOn <= createdTo.Value) &&
                    (!userId.HasValue || a.CreatedBy == $"{userId.Value}")
                );
        
                var orderedQuery = filteredQuery.OrderByDescending(x => x.CreatedOn);
        
                //..execute query
                var totalRecords = await orderedQuery.CountAsync();
                var entities = await orderedQuery
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
        
                return new PagedResult<DepartmentUnit>  {
                    Entities = entities,
                    Count = totalRecords,
                    Page = pageIndex,
                    Size = pageSize
                };
            } catch (Exception ex)  {
                Logger.LogActivity($"Failed to retrieve department units: {ex.Message}", "ERROR");
                throw;
            }
        }

        public Task<bool> InsertUnitAsync(DepartmentUnit department) {
            throw new NotImplementedException();
        }
        
        public Task<bool> DeleteUnitAsync(DepartmentUnit department, bool includeDeleted) {
            throw new NotImplementedException();
        }

    }
}
