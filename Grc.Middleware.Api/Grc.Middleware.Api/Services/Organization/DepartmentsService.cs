using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Services.Organization {

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
                
                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "DEPARTMENT-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                
                throw; 
            };
        }

        public async Task<Department> GetDepartmentByCodeAsync(string code, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve department record with '{code}' code", "INFO");
    
            try {;
                var department = await uow.DepartmentRepository.GetAsync(t => t.DepartmentCode == code, includeDeleted);
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
                
                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "DEPARTMENT-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                
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

                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "DEPARTMENT-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                
                throw; 
            };
        }
        
        public async Task<IList<Department>> GetAllAsync(bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all departments records", "INFO");
    
            try {;
                var department = await uow.DepartmentRepository.GetAllAsync(includeDeleted);
                if(department != null) {
                    //..log the department records being retrieved
                    var typeJson = JsonSerializer.Serialize(department, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Department data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"No Department data found", "DEBUG");
                }

                return department;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve departments: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "DEPARTMENT-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw; 
            };
        }

        public async Task<PagedResult<Department>> GetPagedDepartmentsAsync(DateTime? createdFrom = null, DateTime? createdTo = null, long? userId = null, int pageIndex = 1, int pageSize = 20, bool includeDeleted=false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all departments", "INFO");
    
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
                
                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "DEPARTMENT-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                
                throw;
            }
        }

        public async Task<bool> InsertDepartmentAsync(DepartmentRequest request) {
            using var uow = UowFactory.Create();
             try {

                //..create department record
                var department = new Department(){ 
                    DepartmentCode = request.DepartmentCode,
                    DepartmentName = request.DepartmentName,
                    Alias = request.Alias,
                    BranchId = request.BranchId,
                    CreatedBy = $"{request.UserId}",
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = $"{request.UserId}",
                    LastModifiedOn = DateTime.Now,
                };

                //..log the department data being saved
                var departmentJson = JsonSerializer.Serialize(department, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Department data: {departmentJson}", "DEBUG");
        
                await uow.DepartmentRepository.InsertAsync(department);

                //..check branch state
                var entityState = ((UnitOfWork)uow).Context.Entry(department).State;
                Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");
        
                var result = await uow.SaveChangesAsync();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");

                return result > 0;
             } catch (Exception ex) {
                Logger.LogActivity($"Failed to save department: {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "BRANCHSERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw; 
             }
        }
        
        public async Task<bool> UpdateDepartmentAsync(DepartmentRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update department with ID {request.UserId}", "INFO");
    
            try {

                var department = await uow.DepartmentRepository.GetAsync(u => u.Id == request.Id);
                if(department != null){ 
                    department.DepartmentCode = request.DepartmentCode;
                    department.DepartmentName = request.DepartmentName;
                    department.Alias = request.Alias;
                    department.IsDeleted = request.IsDeleted;
                    department.IsDeleted = request.IsDeleted;
                    department.BranchId = request.BranchId;
                    department.LastModifiedOn = DateTime.Now;
                    department.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _= await uow.DepartmentRepository.UpdateAsync(department);
                    var entityState = ((UnitOfWork)uow).Context.Entry(department).State;
                    Logger.LogActivity($"Entity state after Update: {entityState}", "DEBUG");
                   
                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update department record: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "DEPARTMENT-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw; 
            }
        }

        public async Task<bool> DeleteDepartmentAsync(IdRequest request) {
            using var uow = UowFactory.Create();

            try {

                var branchJson = JsonSerializer.Serialize(request, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Department data: {branchJson}", "DEBUG");

                var department = await uow.DepartmentRepository.GetAsync(t => t.Id == request.RecordId);
                if(department != null){ 
                    //..mark as delete this department
                    _= await uow.DepartmentRepository.DeleteAsync(department, request.IsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(department).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");
                   
                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete department: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "DEPARTMENT-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                
                throw; 
            }
        }
        
        public async Task<bool> ExistsByIdAsync(long id) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if department with ID '{id}' exists", "INFO");
    
            try {
                return await uow.DepartmentRepository.ExistsAsync(b => b.Id == id); 
            } catch (Exception ex)  {
                Logger.LogActivity($"Failed to retrieve department record: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "DEPARTMENT-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }
        }

        public async Task<bool> ExistsAsync(DepartmentRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if unit with name or code exists", "INFO");
    
            try {
                //...build query first
                return await uow.DepartmentRepository.ExistsAsync(d => d.DepartmentCode == request.DepartmentCode || d.DepartmentCode == request.DepartmentCode); 
            } catch (Exception ex)  {
                Logger.LogActivity($"Failed to retrieve department info: {ex.Message}", "ERROR");
                
                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "DEPARTMENT-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                
                throw;
            }
        }

    }
}
