using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Grc.Middleware.Api.Services {

    public class DepartmentsService : BaseService, IDepartmentsService {

        public DepartmentsService(IServiceLoggerFactory loggerFactory, 
            IUnitOfWorkFactory uowFactory, IMapper mapper) 
            : base(loggerFactory, uowFactory, mapper) {
        }
        
        public async Task<Department> GetDepartmentByIdAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve department record with '{id}' ID", "INFO");
    
            try {;
                var department = await uow.DepartmentRepository.GetAsync(t => t.Id == id, includeDeleted);
                if(department != null) {
                    //..log the Department data being retrieved
                    var typeJson = JsonSerializer.Serialize(department, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Department data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"Department with ID '{id}' not found", "DEBUG");
                }

                return department;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Department: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            };
        }

        public async Task<Department> GetDepartmentByCodeAsync(string code, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve department record with '{code}' code", "INFO");
    
            try {;
                var department = await uow.DepartmentRepository.GetAsync(t => t.DepartmenCode == code, includeDeleted);
                if(department != null) {
                    //..log the Department data being retrieved
                    var typeJson = JsonSerializer.Serialize(department, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Department data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"Department with code '{code}' not found", "DEBUG");
                }

                return department;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Department: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            };
        }

        public async Task<Department> GetDepartmentByNameAsync(string name, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve department record with '{name}' name", "INFO");
    
            try {;
                var department = await uow.DepartmentRepository.GetAsync(t => t.DepartmentName == name, includeDeleted);
                if(department != null) {
                    //..log the Department data being retrieved
                    var typeJson = JsonSerializer.Serialize(department, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Department data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"Department with name '{name}' not found", "DEBUG");
                }

                return department;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Department: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            };
        }

        public async Task<PagedResult<Department>> GetPagedDepartmentsAsync(DateTime? createdFrom = null, DateTime? createdTo = null, long? userId = null, int pageIndex = 1, int pageSize = 20, bool includeDeleted=false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all log settings", "INFO");
    
            try {
                //...build query first
                var query = uow.DepartmentRepository.GetAll(includeDeleted, d => d.Branch)
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
        
                return new PagedResult<Department>  {
                    Entities = entities,
                    Count = totalRecords,
                    Page = pageIndex,
                    Size = pageSize
                };
            } catch (Exception ex)  {
                Logger.LogActivity($"Failed to retrieve departments: {ex.Message}", "ERROR");
                throw;
            }
        }

        public Task<bool> InsertDepartmentAsync(Department department) {
            throw new NotImplementedException();
        }
        
        public Task<bool> DeleteDepartmentAsync(Department department, bool includeDeleted) {
            throw new NotImplementedException();
        }

    }

}
