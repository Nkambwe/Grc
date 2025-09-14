using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Services {
    public class BranchService : BaseService , IBranchService, IBaseService {
        public BranchService(IServiceLoggerFactory loggerFactory, 
            IUnitOfWorkFactory uowFactory, 
            IMapper mapper) :
            base(loggerFactory, uowFactory, mapper) {
        }

        public async Task<Branch> GetByIdAsync(long id, bool includeDeleted=false){ 
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve branch record with '{id}' ID", "INFO");
    
            try {;
                var department = await uow.BranchRepository.GetAsync(t => t.Id == id, includeDeleted);
                if(department != null) {
                    //..log the branch data being retrieved
                    var typeJson = JsonSerializer.Serialize(department, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Branch data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"Branch with ID '{id}' not found", "DEBUG");
                }

                return department;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Branch: {ex.Message}", "ERROR");
        
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
                    ErrorSource = "BRANCH-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                
                throw; 
            };
        }

        public async Task<Branch> GetByNameAsync(string name, bool includeDeleted=false){ 
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve branch record with '{name}' as name", "INFO");
    
            try {;
                var department = await uow.BranchRepository.GetAsync(t => t.BranchName == name, includeDeleted);
                if(department != null) {
                    //..log the branch data being retrieved
                    var typeJson = JsonSerializer.Serialize(department, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Branch data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"Branch with name '{name}' not found", "DEBUG");
                }

                return department;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Branch: {ex.Message}", "ERROR");
        
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
                    ErrorSource = "BRANCH-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                throw; 
            };
        }

        public async Task<Branch> GetBySolIdAsync(string solId, bool includeDeleted=false){ 
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve branch record with '{solId}' as name", "INFO");
    
            try {;
                var department = await uow.BranchRepository.GetAsync(t => t.SolId == solId, includeDeleted);
                if(department != null) {
                    //..log the branch data being retrieved
                    var typeJson = JsonSerializer.Serialize(department, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Branch data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"Branch with SolId '{solId}' not found", "DEBUG");
                }

                return department;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Branch: {ex.Message}", "ERROR");
        
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
                    ErrorSource = "BRANCH-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw; 
            };
            
        }

        public async Task<IList<Branch>> GetAllAsync(bool includeDeleted=false){ 
             using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all branch records", "INFO");
    
            try {;
                var department = await uow.BranchRepository.GetAllAsync(includeDeleted);
                if(department != null) {
                    //..log the branch records being retrieved
                    var typeJson = JsonSerializer.Serialize(department, new JsonSerializerOptions { 
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles 
                    });
                    Logger.LogActivity($"Branches data: {typeJson}", "DEBUG");
                } else {
                    Logger.LogActivity($"No branch data found", "DEBUG");
                }

                return department;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve branches: {ex.Message}", "ERROR");
        
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
                    ErrorSource = "BRANCH-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw; 
            };
       }

        public async Task<bool> InsertBranchAsync(BranchRequest request) {
             using var uow = UowFactory.Create();
             try {

                //..create branch record
                var branch = new Branch(){ 
                    BranchName = request.BranchName,
                    SolId = request.SolId,
                    CompanyId = request.CompanyId,
                    CreatedBy = $"{request.UserId}",
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = $"{request.UserId}",
                    LastModifiedOn = DateTime.Now,
                };

                //..log the branch data being saved
                var branchJson = JsonSerializer.Serialize(branch, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Branch data: {branchJson}", "DEBUG");
        
                await uow.BranchRepository.InsertAsync(branch);

                //..check branch state
                var entityState = ((UnitOfWork)uow).Context.Entry(branch).State;
                Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");
        
                var result = await uow.SaveChangesAsync();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");

                return result > 0;
             } catch (Exception ex) {
                Logger.LogActivity($"Failed to save branch: {ex.Message}", "ERROR");
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
                    ErrorSource = "BRANCH-SERVICE-MIDDLEWARE",
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

        public async Task<bool> UpdateBranchAsync(BranchRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update branch with ID {request.UserId}", "INFO");
    
            try {

                var branch = await uow.BranchRepository.GetAsync(u => u.Id == request.Id);
                if(branch != null){ 
                    //..update branch record
                    branch.BranchName = request.BranchName;
                    branch.SolId = request.SolId;
                    //branch.CompanyId = request.CompanyId;
                    branch.IsDeleted = request.IsDeleted;
                    branch.LastModifiedOn = DateTime.Now;
                    branch.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _= await uow.BranchRepository.UpdateAsync(branch);
                    var entityState = ((UnitOfWork)uow).Context.Entry(branch).State;
                    Logger.LogActivity($"Entity state after Update: {entityState}", "DEBUG");
                   
                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update branch record: {ex.Message}", "ERROR");
        
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
                    ErrorSource = "BRANCH-SERVICE-MIDDLEWARE",
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

        public async Task<bool> DeleteBranchAsync(DeleteRequst request) {
            using var uow = UowFactory.Create();

            try {

                var branchJson = JsonSerializer.Serialize(request, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Branch data: {branchJson}", "DEBUG");

                var branch = await uow.BranchRepository.GetAsync(t => t.Id == request.RecordId);
                if(branch != null){ 
                    //..mark as delete this branch
                    _= await uow.BranchRepository.DeleteAsync(branch, request.IsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(branch).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");
                   
                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete branch: {ex.Message}", "ERROR");
        
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
                    ErrorSource = "BRANCH-SERVICE-MIDDLEWARE",
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
            Logger.LogActivity($"Check if branch with ID '{id}' exists", "INFO");
    
            try {
                return await uow.BranchRepository.ExistsAsync(b => b.Id == id); 
            } catch (Exception ex)  {
                Logger.LogActivity($"Failed to retrieve branch record: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "BRANCH-SERVICE-MIDDLEWARE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }
        }

        public async Task<bool> ExistsAsync(BranchRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if branch with name or solId exists", "INFO");
    
            try {
                return await uow.BranchRepository.ExistsAsync(b => b.BranchName == request.BranchName ||b.SolId == request.SolId); 
            } catch (Exception ex)  {
                Logger.LogActivity($"Failed to retrieve branch record: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                
                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new(){ 
                    ErrorMessage = ex.Message,
                    ErrorSource = "BRANCH-SERVICE-MIDDLEWARE",
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