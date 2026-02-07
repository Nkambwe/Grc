using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services {
    public class BugService : BaseService, IBugService {
        public BugService(IServiceLoggerFactory loggerFactory, 
                            IUnitOfWorkFactory uowFactory, 
                            IMapper mapper) 
            : base(loggerFactory, uowFactory, mapper) {
        }

        public async Task<bool> ExistsAsync(Expression<Func<SystemError, bool>> predicate) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if system error with predicate: {predicate} exists" , "INFO");
            try {
                return await uow.SystemErrorRespository.ExistsAsync(predicate, true);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to determine status of system erors: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                await uow.SaveChangesAsync();
                throw;
            }
        }

        public async Task<SystemError> GetBugAsync(long recordId) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve system bug with id: {recordId}", "INFO");
            try {
                return await uow.SystemErrorRespository.GetAsync(recordId, true);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve system erors: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                await uow.SaveChangesAsync();
                throw;
            }
        }

        public async Task<PagedResult<SystemError>> GetBugsAsync(BugListRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve all system bugs", "INFO");
            try {
                return await uow.SystemErrorRespository.PageAllAsync(
                    request.PageIndex, 
                    request.PageSize,false);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve system erors: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                await uow.SaveChangesAsync();
                throw;
            }
        }

        public async Task<IList<SystemError>> GetAllAsync(System.Linq.Expressions.Expression<Func<SystemError, bool>> where, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve all system bugs", "INFO");
            try {
                return await uow.SystemErrorRespository.GetAllAsync(where, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve system erors: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                await uow.SaveChangesAsync();
                throw;
            }
        }

        public async Task<PagedResult<SystemError>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<SystemError, bool>> predicate) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve all system bugs", "INFO");
            try {
                return await uow.SystemErrorRespository.PageAllAsync(page, size, includeDeleted, predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve system erors: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                await uow.SaveChangesAsync();
                throw;
            }
        }
        
        public async Task<bool> InsertSystemErrorAsync(BugRequest request, string username) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Save system error record >>>>", "INFO");

            try {
                //..log the system error being saved
                var bugJson = JsonSerializer.Serialize(request, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Error data: {bugJson}", "DEBUG");

                //..get company record
                var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = company != null ? company.Id : 1;

                var bug = new SystemError() {
                    ErrorMessage = request.ErrorMessage,
                    ErrorSource = request.Source,
                    StackTrace = request.StatckTrace,
                    Severity = request.Severity,
                    IsUserReported = true,
                    Assigned = !string.IsNullOrWhiteSpace(request.AssignedTo),
                    AssignedTo = request.AssignedTo,
                    FixStatus = "OPEN",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId,
                    IsDeleted = false,
                    CreatedOn = DateTime.Now,
                    CreatedBy = username,
                    LastModifiedBy = username,
                    LastModifiedOn = DateTime.Now

                };
                await uow.SystemErrorRespository.InsertAsync(bug);

                //..check entity state
                var entityState = ((UnitOfWork)uow).Context.Entry(request).State;
                Logger.LogActivity($"System Error state after insert: {entityState}", "DEBUG");

                var result = await uow.SaveChangesAsync();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");

                return result > 0;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save system erors: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                await uow.SaveChangesAsync();
                throw;
            }
        }

        public async Task<bool> UpdateStatusAsync(long recordId, string status, string username) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Change system error status", "INFO");

            try {
                var bug = await uow.SystemErrorRespository.GetAsync(a => a.Id == recordId);
                if (bug != null) {
                    bug.FixStatus = status;
                    bug.LastModifiedOn = DateTime.Now;
                    bug.LastModifiedBy = $"{username}";
                    if (status.Equals("CLOSED")) {
                        bug.ClosedOn = DateTime.Now;
                    }
                    bug.LastModifiedOn = DateTime.Now;
                    bug.LastModifiedBy = $"{username}";

                    //..check entity state
                    _ = await uow.SystemErrorRespository.UpdateAsync(bug);
                    var entityState = ((UnitOfWork)uow).Context.Entry(bug).State;
                    Logger.LogActivity($"System Error state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update system erors: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                await uow.SaveChangesAsync();
                throw;
            }
        }

        public async Task<bool> UpdateErrorAsync(BugRequest request, string username) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Update system error", "INFO");

            try {
                var bug = await uow.SystemErrorRespository.GetAsync(a => a.Id == request.Id);
                if (bug != null) {
                    //..only update these if is user generated
                    if (bug.IsUserReported) {
                        bug.ErrorMessage = request.ErrorMessage;
                        bug.ErrorSource = request.Source;
                        bug.Severity = request.Severity;
                        bug.StackTrace = request.StatckTrace ?? string.Empty;
                    }
                    bug.FixStatus = request.Status;
                    bug.AssignedTo = request.AssignedTo;
                    if (!string.IsNullOrEmpty(request.AssignedTo)) {
                        bug.Assigned = true;
                    }
                    bug.LastModifiedOn = DateTime.Now;
                    bug.LastModifiedBy = $"{username}";
                    bug.LastModifiedOn = DateTime.Now;
                    bug.LastModifiedBy = $"{username}";

                    //..check entity state
                    _ = await uow.SystemErrorRespository.UpdateAsync(bug);
                    var entityState = ((UnitOfWork)uow).Context.Entry(bug).State;
                    Logger.LogActivity($"System Error state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update system erors: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                await uow.SaveChangesAsync();
                throw;
            }
        }

        #region Private Methods

        private SystemError HandleError(IUnitOfWork uow, Exception ex) {
            var innerEx = ex.InnerException;
            while (innerEx != null) {
                Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                innerEx = innerEx.InnerException;
            }
            Logger.LogActivity($"{ex.StackTrace}", "ERROR");

            var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
            long companyId = company != null ? company.Id : 1;
            return new() {
                ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                ErrorSource = "GUG-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId,
                CreatedBy = "SYSTEM",
                CreatedOn = DateTime.Now
            };

        }

        #endregion
    }
}
