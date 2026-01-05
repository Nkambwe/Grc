using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {

    public class CircularIssueService : BaseService, ICircularIssueService {

        public CircularIssueService(IServiceLoggerFactory loggerFactory, 
            IUnitOfWorkFactory uowFactory, 
            IMapper mapper) 
            : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of issues", "INFO");

            try {
                return uow.CircularIssueRepository.Count();
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count issues in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public int Count(Expression<Func<CircularIssue, bool>> predicate) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of issues in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.CircularIssueRepository.Count(predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count issues in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<CircularIssue, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of issues in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.CircularIssueRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count issues in the database: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Exists(Expression<Func<CircularIssue, bool>> predicate, bool excludeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Issue exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.CircularIssueRepository.Exists(predicate, excludeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for Issue in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<CircularIssue, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Issue exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.CircularIssueRepository.ExistsAsync(predicate, excludeDeleted, token);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for Issue in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<CircularIssue>> GetAllAsync(Expression<Func<CircularIssue, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Circular Issues", "INFO");

            try {
                return await uow.CircularIssueRepository.GetAllAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Circular Issues : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<CircularIssue>> GetAllAsync(Expression<Func<CircularIssue, bool>> predicate, bool includeDeleted = false, params Expression<Func<CircularIssue, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Circular Issues", "INFO");

            try {
                return await uow.CircularIssueRepository.GetAllAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Circular Issues : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<CircularIssue> GetAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Circular issue With ID '{id}'", "INFO");

            try {
                return await uow.CircularIssueRepository.GetAsync(id, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Circular issue: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<CircularIssue> GetAsync(Expression<Func<CircularIssue, bool>> predicate, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Circular issue that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.CircularIssueRepository.GetAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Circular issue: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<CircularIssue> GetAsync(Expression<Func<CircularIssue, bool>> predicate, bool includeDeleted = false, params Expression<Func<CircularIssue, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Circular issue that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.CircularIssueRepository.GetAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Circular issue: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Insert(CircularIssueRequest request) {
            using var uow = UowFactory.Create();
            try {
                //..map record to entity
                var category = new CircularIssue() {
                    IssueDescription = (request.IssueDescription ?? string.Empty).Trim(),
                    Resolution = request.Resolution ?? string.Empty,
                    Status = request.Status,
                    RecievedOn = request.RecievedOn,
                    ResolvedOn = request.ResolvedOn,
                    IsDeleted = request.IsDeleted,
                    CreatedBy = $"{request.UserName}",
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = $"{request.UserName}",
                    LastModifiedOn = DateTime.Now
                };

                //..log the en tity data being saved
                var categoryJson = JsonSerializer.Serialize(category, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Circular issue data: {categoryJson}", "DEBUG");

                var added = uow.CircularIssueRepository.Insert(category);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(category).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save Circular : {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> InsertAsync(CircularIssueRequest request) {
            using var uow = UowFactory.Create();
            try {
                //..map record to entity
                var category = new CircularIssue() {
                    IssueDescription = (request.IssueDescription ?? string.Empty).Trim(),
                    Resolution = request.Resolution ?? string.Empty,
                    Status = request.Status,
                    RecievedOn = request.RecievedOn,
                    ResolvedOn = request.ResolvedOn,
                    IsDeleted = request.IsDeleted,
                    CreatedBy = $"{request.UserName}",
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = $"{request.UserName}",
                    LastModifiedOn = DateTime.Now
                };

                //..log the en tity data being saved
                var categoryJson = JsonSerializer.Serialize(category, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Circular issue data: {categoryJson}", "DEBUG");

                var added = await uow.CircularIssueRepository.InsertAsync(category);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(category).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save Circular : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Update(CircularIssueRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Circular issue request", "INFO");

            try {
                var issue = uow.CircularIssueRepository.Get(a => a.Id == request.Id, includeDeleted);
                if (issue != null) {
                    //..update Control Item record
                    issue.IssueDescription = (request.IssueDescription ?? string.Empty).Trim();
                    issue.Resolution = request.Resolution ?? string.Empty;
                    issue.IsDeleted = request.IsDeleted;
                    issue.Status = request.Status;
                    issue.RecievedOn = request.RecievedOn;
                    issue.ResolvedOn = request.ResolvedOn;
                    issue.LastModifiedOn = DateTime.Now;
                    issue.LastModifiedBy = $"{request.UserName}";

                    //..check entity state
                    _ = uow.CircularIssueRepository.Update(issue, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(issue).State;
                    Logger.LogActivity($"Circular issue state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> UpdateAsync(CircularIssueRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Circular issue request", "INFO");

            try {
                var issue = await uow.CircularIssueRepository.GetAsync(a => a.Id == request.Id, includeDeleted);
                if (issue != null) {
                    //..update record
                    issue.IssueDescription = (request.IssueDescription ?? string.Empty).Trim();
                    issue.Resolution = request.Resolution ?? string.Empty;
                    issue.IsDeleted = request.IsDeleted;
                    issue.Status = request.Status;
                    issue.RecievedOn = request.RecievedOn;
                    issue.ResolvedOn = request.ResolvedOn;
                    issue.LastModifiedOn = DateTime.Now;
                    issue.LastModifiedBy = $"{request.UserName}";

                    //..check entity state
                    _ = await uow.CircularIssueRepository.UpdateAsync(issue, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(issue).State;
                    Logger.LogActivity($"Circular issue state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Delete(IdRequest request) {
            using var uow = UowFactory.Create();
            try {
                var categoryJson = JsonSerializer.Serialize(request, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Circular Issue data: {categoryJson}", "DEBUG");

                var issue = uow.CircularIssueRepository.Get(t => t.Id == request.RecordId);
                if (issue != null) {
                    //..mark as delete this issue
                    _ = uow.CircularIssueRepository.Delete(issue, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(issue).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete issue : {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> DeleteAsync(IdRequest request) {
            using var uow = UowFactory.Create();
            try {
                var categoryJson = JsonSerializer.Serialize(request, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Circular Issue data: {categoryJson}", "DEBUG");

                var issue = await uow.CircularIssueRepository.GetAsync(t => t.Id == request.RecordId);
                if (issue != null) {
                    //..mark as delete this issue
                    _ = await uow.CircularIssueRepository.DeleteAsync(issue, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(issue).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete issue : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<CircularIssue>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<CircularIssue, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Circular issues", "INFO");

            try {
                return await uow.CircularIssueRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve circular issues : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<CircularIssue>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<CircularIssue, bool>> predicate = null) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged circular issues", "INFO");

            try {
                return await uow.CircularIssueRepository.PageAllAsync(page, size, includeDeleted, predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve circular issues : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<CircularIssueResponse>> PageLookupAsync<CircularIssueResponse>(int page, int size, bool includeDeleted, Expression<Func<CircularIssue, CircularIssueResponse>> selector) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Circular issues", "INFO");

            try {
                return await uow.CircularIssueRepository.PageLookupAsync(page, size, includeDeleted, selector);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Circular issues: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        #region private methods

        private SystemError HandleError(IUnitOfWork uow, Exception ex) {

            //..log inner exceptions here too
            var innerEx = ex.InnerException;
            while (innerEx != null) {
                Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                innerEx = innerEx.InnerException;
            }

            var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
            long companyId = company != null ? company.Id : 1;
            return new() {
                ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                ErrorSource = "CIRCULAR-ARTICLES-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId
            };

        }

        #endregion

    }
}
