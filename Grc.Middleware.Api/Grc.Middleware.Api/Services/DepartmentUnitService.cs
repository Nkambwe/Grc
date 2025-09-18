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

                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "DEPARTMENT-UNIT-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
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

                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "DEPARTMENT-UNIT-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
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
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "DEPARTMENT-UNIT-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw; 
            };
        }
        
        public async Task<IList<DepartmentUnit>> GetAllAsync(bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all units records", "INFO");
    
            try {;
                var department = await uow.DepartmentUnitRepository.GetAllAsync(includeDeleted);
                if(department != null) {
                    //..log the units records being retrieved
                    var typeJson = JsonSerializer.Serialize(department, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Units data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"No Units data found", "DEBUG");
                }

                return department;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve units: {ex.Message}", "ERROR");
        
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
                    ErrorSource = "DEPARTMENT-UNITS-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
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
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "DEPARTMENT-UNIT-SERVICE-MIDDLEWARE",
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
        
        public async Task<bool> ExistsByIdAsync(long id) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if unit with ID '{id}' exists", "INFO");
    
            try {
                return await uow.DepartmentUnitRepository.ExistsAsync(b => b.Id == id); 
            } catch (Exception ex)  {
                Logger.LogActivity($"Failed to retrieve unit record: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "DEPARTMENT-UNIT-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }
        }

        public async Task<bool> ExistsAsync(DepartmentUnitRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if unit with name or code exists", "INFO");
    
            try {
                //...build query first
                return await uow.DepartmentUnitRepository.ExistsAsync(
                    u => u.UnitName == request.UnitName ||
                    u.UnitCode == request.UnitCode); 
            } catch (Exception ex)  {
                Logger.LogActivity($"Failed to retrieve department units: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "DEPARTMENT-UNIT-SERVICE-MIDDLEWARE",
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

        public async Task<bool> InsertUnitAsync(DepartmentUnitRequest request) {
            using var uow = UowFactory.Create();

             try {

                //..create unit record
                var unit = new DepartmentUnit(){ 
                    UnitCode = request.UnitCode,
                    UnitName = request.UnitName,
                    DepartmentId = request.DepartmentId,
                    CreatedBy = $"{request.UserId}",
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = $"{request.UserId}",
                    LastModifiedOn = DateTime.Now,
                };

                //..log the unit data being saved
                var unitJson = JsonSerializer.Serialize(unit, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Unit data: {unitJson}", "DEBUG");
        
                await uow.DepartmentUnitRepository.InsertAsync(unit);

                //..check unit state
                var entityState = ((UnitOfWork)uow).Context.Entry(unit).State;
                Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");
        
                var result = await uow.SaveChangesAsync();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");

                return result > 0;
             } catch (Exception ex) {
                Logger.LogActivity($"Failed to save unit: {ex.Message}", "ERROR");
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
                    ErrorSource = "DEPARTMENT-UNIT-SERVICE-MIDDLEWARE",
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
        
        public async Task<bool> UpdateUnitAsync(DepartmentUnitRequest request) {
           using var uow = UowFactory.Create();
            Logger.LogActivity($"Update unit with ID {request.UserId}", "INFO");
    
            try {

                var unit = await uow.DepartmentUnitRepository.GetAsync(u => u.Id == request.Id);
                if(unit != null){ 
                    //..update unit record
                    unit.UnitCode = request.UnitCode;
                    unit.UnitName = request.UnitName;
                    unit.DepartmentId = request.DepartmentId;
                    unit.LastModifiedOn = DateTime.Now;
                    unit.IsDeleted = request.IsDeleted;
                    unit.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _= await uow.DepartmentUnitRepository.UpdateAsync(unit);
                    var entityState = ((UnitOfWork)uow).Context.Entry(unit).State;
                    Logger.LogActivity($"Entity state after Update: {entityState}", "DEBUG");
                   
                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update unit record: {ex.Message}", "ERROR");
        
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
                    ErrorSource = "DEPARTMENT-UNIT-SERVICE-MIDDLEWARE",
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

        public async Task<bool> DeleteUnitAsync(IdRequst request) {
            using var uow = UowFactory.Create();

            try {

                var unitJson = JsonSerializer.Serialize(request, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Unit data: {unitJson}", "DEBUG");

                var unit = await uow.DepartmentUnitRepository.GetAsync(t => t.Id == request.RecordId);
                if(unit != null){ 
                    //..mark as deleted this department unit
                    _= await uow.DepartmentUnitRepository.DeleteAsync(unit, request.IsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(unit).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");
                   
                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete unit: {ex.Message}", "ERROR");
        
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
                    ErrorSource = "DEPARTMENT-UNIT-SERVICE-MIDDLEWARE",
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
