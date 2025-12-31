using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {

    public class ComplianceIssueService : BaseService, IComplianceIssueService {

        public ComplianceIssueService(IServiceLoggerFactory loggerFactory, IUnitOfWorkFactory uowFactory, IMapper mapper) 
            : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Compliance issues in the database", "INFO");

            try {
                return uow.ComplianceIssueRepository.Count();
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to Compliance issues in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public int Count(Expression<Func<ComplianceIssue, bool>> predicate) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Compliance issues in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.ComplianceIssueRepository.Count(predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count Compliance issues in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<ComplianceIssue, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Compliance issues in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.ComplianceIssueRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count Compliance issues in the database: {ex.Message}", "ERROR");
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
                Logger.LogActivity($"Compliance issue data: {categoryJson}", "DEBUG");

                var categories = uow.ComplianceIssueRepository.Get(t => t.Id == request.RecordId);
                if (categories != null) {
                    //..mark as delete this Compliance issue
                    _ = uow.ComplianceIssueRepository.Delete(categories, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(categories).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Compliance issue : {ex.Message}", "ERROR");
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
                Logger.LogActivity($"Compliance issue data: {categoryJson}", "DEBUG");

                var categories = await uow.ComplianceIssueRepository.GetAsync(t => t.Id == request.RecordId);
                if (categories != null) {
                    //..mark as delete this Compliance issue
                    _ = await uow.ComplianceIssueRepository.DeleteAsync(categories, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(categories).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Compliance issue : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Exists(Expression<Func<ComplianceIssue, bool>> predicate, bool excludeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Compliance issue exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.ComplianceIssueRepository.Exists(predicate, excludeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for Compliance issue in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<ComplianceIssue, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Compliance issues exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.ComplianceIssueRepository.ExistsAsync(predicate, excludeDeleted, token);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for Compliance issues in the database: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ComplianceIssue>> GetAllAsync(Expression<Func<ComplianceIssue, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Compliance issues", "INFO");

            try {
                return await uow.ComplianceIssueRepository.GetAllAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Compliance issues : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ComplianceIssue>> GetAllAsync(Expression<Func<ComplianceIssue, bool>> predicate, bool includeDeleted = false, params Expression<Func<ComplianceIssue, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Compliance issues", "INFO");

            try {
                return await uow.ComplianceIssueRepository.GetAllAsync(includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Compliance issues : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<ComplianceIssue> GetAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Compliance issue with '{id}'", "INFO");

            try {
                return await uow.ComplianceIssueRepository.GetAsync(id, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Compliance issue : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<ComplianceIssue> GetAsync(Expression<Func<ComplianceIssue, bool>> predicate, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Compliance issue that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ComplianceIssueRepository.GetAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Compliance issue : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<ComplianceIssue> GetAsync(Expression<Func<ComplianceIssue, bool>> predicate, bool includeDeleted = false, params Expression<Func<ComplianceIssue, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Compliance issues that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ComplianceIssueRepository.GetAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Compliance issues : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Insert(ComplianceIssueRequest request) {
            using var uow = UowFactory.Create();
            try {
                //..map entity
                var issue = new ComplianceIssue() {
                    StatutoryArticleId = request.ArticleId,
                    Description = (request.Description ?? string.Empty).Trim(),
                    Notes = request.Comments ?? string.Empty,
                    IsClosed = request.IsClosed,
                    IsDeleted = false,
                    CreatedOn = DateTime.Now,
                    CreatedBy = $"{request.UserName}",
                    LastModifiedBy = $"{request.UserName}",
                    LastModifiedOn = DateTime.Now
                };


                //..log the Compliance issue data being saved
                var issueJson = JsonSerializer.Serialize(issue, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Compliance issue data: {issueJson}", "DEBUG");

                var added = uow.ComplianceIssueRepository.Insert(issue);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(issue).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save Compliance issue : {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> InsertAsync(ComplianceIssueRequest request) {
            using var uow = UowFactory.Create();
            try {
                //..map entity
                var issue = new ComplianceIssue() { 
                    StatutoryArticleId = request.ArticleId,
                    Description = (request.Description ?? string.Empty).Trim(), 
                    Notes = request.Comments ?? string.Empty,
                    IsClosed = request.IsClosed,
                    IsDeleted = false,
                    CreatedOn = DateTime.Now,
                    CreatedBy = $"{request.UserName}",
                    LastModifiedBy  = $"{request.UserName}",
                    LastModifiedOn = DateTime.Now
                };

                //..log the Compliance issue data being saved
                var issueJson = JsonSerializer.Serialize(issue, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Compliance issue data: {issueJson}", "DEBUG");

                var added = await uow.ComplianceIssueRepository.InsertAsync(issue);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(issue).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save Compliance issue : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ComplianceIssue>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ComplianceIssue, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Compliance issues", "INFO");

            try {
                return await uow.ComplianceIssueRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Compliance issues : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ComplianceIssue>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ComplianceIssue, bool>> predicate = null) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Compliance issues", "INFO");

            try {
                return await uow.ComplianceIssueRepository.PageAllAsync(page, size, includeDeleted, predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Compliance issues : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<TResponse>> PageLookupAsync<TResponse>(int page, int size, bool includeDeleted, Expression<Func<ComplianceIssue, TResponse>> selector) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Compliance issues", "INFO");

            try {
                return await uow.ComplianceIssueRepository.PageLookupAsync(page, size, includeDeleted, selector);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Compliance issues: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Update(ComplianceIssueRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Compliance issue request", "INFO");

            try {
                var issue = uow.ComplianceIssueRepository.Get(a => a.Id == request.Id);
                if (issue != null) {
                    //..update Compliance issue record
                    issue.Description = (request.Description ?? string.Empty).Trim();
                    issue.Notes = request.Comments ?? string.Empty;
                    issue.IsClosed = request.IsClosed;
                    issue.IsDeleted = request.IsDeleted;
                    issue.LastModifiedOn = DateTime.Now;
                    issue.LastModifiedBy = $"{request.UserName}";

                    //..check entity state
                    _ = uow.ComplianceIssueRepository.Update(issue, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(issue).State;
                    Logger.LogActivity($"Compliance issue state after Update: {entityState}", "DEBUG");

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

        public async Task<bool> UpdateAsync(ComplianceIssueRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Compliance issue request", "INFO");

            try {
                var issue = await uow.ComplianceIssueRepository.GetAsync(a => a.Id == request.Id);
                if (issue != null) {
                    //..update Compliance issue record
                    issue.Description = (request.Description ?? string.Empty).Trim();
                    issue.Notes = request.Comments ?? string.Empty;
                    issue.IsDeleted = request.IsDeleted;
                    issue.IsClosed = request.IsClosed;
                    issue.LastModifiedOn = DateTime.Now;
                    issue.LastModifiedBy = $"{request.UserName}";

                    //..check entity state
                    _ =await uow.ComplianceIssueRepository.UpdateAsync(issue, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(issue).State;
                    Logger.LogActivity($"Compliance issue state after Update: {entityState}", "DEBUG");

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
                ErrorSource = "STATUTORY-ARTICLES-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId
            };

        }

        #endregion

    }

}
