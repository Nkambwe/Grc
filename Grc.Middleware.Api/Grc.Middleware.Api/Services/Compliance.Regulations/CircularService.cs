using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {

    public class CircularService : BaseService, ICircularService {

        public CircularService(IServiceLoggerFactory 
            loggerFactory, IUnitOfWorkFactory uowFactory, 
            IMapper mapper) 
            : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of circulars", "INFO");

            try {
                return uow.CircularRepository.Count();
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count circulars in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public int Count(Expression<Func<Circular, bool>> predicate) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of circulars in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.CircularRepository.Count(predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count circulars in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<Circular, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of circulars in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.CircularRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count circulars in the database: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Exists(Expression<Func<Circular, bool>> predicate, bool excludeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Circular exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.CircularRepository.Exists(predicate, excludeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for Circular in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<Circular, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Circular exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return  await uow.CircularRepository.ExistsAsync(predicate, excludeDeleted, token);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for Circular in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<Circular>> GetAllAsync(Expression<Func<Circular, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Circulars", "INFO");

            try {
                return await uow.CircularRepository.GetAllAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Circulars : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<Circular>> GetAllAsync(Expression<Func<Circular, bool>> predicate, bool includeDeleted = false, params Expression<Func<Circular, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Circulars", "INFO");

            try {
                return await uow.CircularRepository.GetAllAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Circulars : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<Circular> GetAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Circular with ID '{id}'", "INFO");

            try {
                return await uow.CircularRepository.GetAsync(id, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Circular : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<Circular> GetAsync(Expression<Func<Circular, bool>> predicate, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Circular that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.CircularRepository.GetAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Circular : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<Circular> GetAsync(Expression<Func<Circular, bool>> predicate, bool includeDeleted = false, params Expression<Func<Circular, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Circular that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.CircularRepository.GetAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Circular : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Insert(CircularRequest request, string username) {
            using var uow = UowFactory.Create();
            try {
                //..map record to entity
                var category = new Circular() {
                    CircularTitle = (request.CircularTitle ?? string.Empty).Trim(),
                    Requirement = (request.Requirement ?? string.Empty).Trim(),
                    RecievedOn = request.RecievedOn,
                    DeadlineOn = request.DeadlineOn,
                    Status = request.Status,
                    SubmissionDate = request.SubmissionDate,
                    FilePath = request.FilePath,
                    SubmittedBy = request.SubmittedBy,
                    Reference = request.SubmissionReference,
                    IsBreached = request.IsBreached,
                    BreachRisk = request.BreachRisk,
                    BreachReason = request.BreachReason,
                    SendReminder = request.SendReminder,
                    Interval = request.Interval,
                    IntervalType = request.IntervalType,
                    Reminder = request.Reminder,
                    RequiredSubmissionDate = request.RequiredSubmissionDate,
                    RequiredSubmissionDay = request.RequiredSubmissionDay,
                    Comments = request.Comments,
                    FrequencyId = request.FrequencyId,
                    AuthorityId = request.AuthorityId,
                    DepartmentId = request.OwnerId,
                    IsDeleted = request.IsDeleted,
                    CreatedBy = $"{username}",
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = $"{username}",
                    LastModifiedOn = DateTime.Now
                };

                //..log the en tity data being saved
                var categoryJson = JsonSerializer.Serialize(category, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Circular data: {categoryJson}", "DEBUG");

                var added = uow.CircularRepository.Insert(category);
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

        public async Task<bool> InsertAsync(CircularRequest request, string username) {
            using var uow = UowFactory.Create();
            try {
                //..map record to entity
                var category = new Circular() {
                    CircularTitle = (request.CircularTitle ?? string.Empty).Trim(),
                    Requirement = (request.Requirement ?? string.Empty).Trim(),
                    RecievedOn = request.RecievedOn,
                    DeadlineOn = request.DeadlineOn,
                    Status = request.Status,
                    SubmissionDate = request.SubmissionDate,
                    FilePath = request.FilePath,
                    SubmittedBy = request.SubmittedBy,
                    IsBreached = request.IsBreached,
                    BreachRisk = request.BreachRisk,
                    BreachReason = request.BreachReason,
                    Reference = request.SubmissionReference,
                    SendReminder = request.SendReminder,
                    Interval = request.Interval,
                    IntervalType = request.IntervalType,
                    Reminder = request.Reminder,
                    RequiredSubmissionDate = request.RequiredSubmissionDate,
                    RequiredSubmissionDay = request.RequiredSubmissionDay,
                    FrequencyId = request.FrequencyId,
                    AuthorityId = request.AuthorityId,
                    DepartmentId = request.OwnerId,
                    IsDeleted = request.IsDeleted,
                    CreatedBy = $"{username}",
                    CreatedOn = DateTime.Now,
                    Comments = request.Comments,
                    LastModifiedBy = $"{username}",
                    LastModifiedOn = DateTime.Now
                };

                //..log the en tity data being saved
                var categoryJson = JsonSerializer.Serialize(category, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Circular data: {categoryJson}", "DEBUG");

                var added = await uow.CircularRepository.InsertAsync(category);
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

        public bool Update(CircularRequest request, string username, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Circular request", "INFO");

            try {
                var circular = uow.CircularRepository.Get(a => a.Id == request.Id, includeDeleted);

                if (circular != null) {
                    //..update record
                    circular.CircularTitle = (request.CircularTitle ?? string.Empty).Trim();
                    circular.Requirement = (request.Requirement ?? string.Empty).Trim();
                    circular.IsDeleted = request.IsDeleted;
                    circular.Status = request.Status;
                    circular.RecievedOn = request.RecievedOn;
                    circular.DeadlineOn = request.DeadlineOn;
                    circular.SubmissionDate = request.SubmissionDate;
                    circular.FilePath = request.FilePath;
                    circular.SubmittedBy = request.SubmittedBy;
                    circular.IsBreached = request.IsBreached;
                    circular.BreachRisk = request.BreachRisk;
                    circular.BreachReason = request.BreachReason;
                    circular.SendReminder = request.SendReminder;
                    circular.Interval = request.Interval;
                    circular.IntervalType = request.IntervalType;
                    circular.Reminder = request.Reminder;
                    circular.RequiredSubmissionDate = request.RequiredSubmissionDate;
                    circular.RequiredSubmissionDay = request.RequiredSubmissionDay;
                    circular.Comments = request.Comments;
                    circular.AuthorityId = request.AuthorityId;
                    circular.FrequencyId = request.FrequencyId;
                    circular.DepartmentId = request.OwnerId;
                    circular.Reference = request.SubmissionReference;
                    circular.LastModifiedOn = DateTime.Now;
                    circular.LastModifiedBy = $"{username}";

                    //..check entity state
                    _ = uow.CircularRepository.Update(circular, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(circular).State;
                    Logger.LogActivity($"Circular state after Update: {entityState}", "DEBUG");

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

        public async Task<bool> UpdateAsync(CircularRequest request, string username, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Circular request", "INFO");

            try {
                var circular = await uow.CircularRepository.GetAsync(a => a.Id == request.Id, includeDeleted);
                if (circular != null) {
                    //..update record
                    circular.CircularTitle = (request.CircularTitle ?? string.Empty).Trim();
                    circular.Requirement = (request.Requirement ?? string.Empty).Trim();
                    circular.IsDeleted = request.IsDeleted;
                    circular.Status = request.Status;
                    circular.RecievedOn = request.RecievedOn;
                    circular.DeadlineOn = request.DeadlineOn;
                    circular.SubmissionDate = request.SubmissionDate;
                    circular.FilePath = request.FilePath;
                    circular.BreachRisk = request.BreachRisk;
                    circular.IsBreached = request.IsBreached;
                    circular.BreachReason = request.BreachReason;
                    circular.SendReminder = request.SendReminder;
                    circular.Interval = request.Interval;
                    circular.IntervalType = request.IntervalType;
                    circular.Reminder = request.Reminder;
                    circular.RequiredSubmissionDate = request.RequiredSubmissionDate;
                    circular.RequiredSubmissionDay = request.RequiredSubmissionDay;
                    circular.Comments = request.Comments;
                    circular.SubmittedBy = request.SubmittedBy;
                    circular.AuthorityId = request.AuthorityId;
                    circular.FrequencyId = request.FrequencyId;
                    circular.DepartmentId = request.OwnerId;
                    circular.Reference = request.SubmissionReference;
                    circular.LastModifiedOn = DateTime.Now;
                    circular.LastModifiedBy = $"{username}";

                    //..check entity state
                    _ = await uow.CircularRepository.UpdateAsync(circular, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(circular).State;
                    Logger.LogActivity($"Circular state after Update: {entityState}", "DEBUG");

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

        public async Task<bool> UpdateSubmissionAsync(CircularSubmissionRequest request, string username) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update circular submission", "INFO");

            try {
                var record = await uow.CircularRepository.GetAsync(a => a.Id == request.Id);
                if (record != null) {
                    record.Reference = request.Reference;
                    record.Status = (request.Status ?? string.Empty).Trim();
                    record.FilePath = (request.FilePath ?? string.Empty).Trim();
                    record.SubmittedBy = (request.SubmittedBy ?? string.Empty).Trim();
                    record.Comments = request.Comments;
                    record.SubmissionDate = DateTime.Now;
                    record.IsBreached = !string.IsNullOrWhiteSpace(request.BreachReason);
                    record.BreachReason = request.BreachReason;
                    record.LastModifiedOn = DateTime.Now;
                    record.LastModifiedBy = $"{username}";

                    //..check entity state
                    _ = await uow.CircularRepository.UpdateAsync(record, true);
                    var entityState = ((UnitOfWork)uow).Context.Entry(record).State;
                    Logger.LogActivity($"Submission state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update submission record: {ex.Message}", "ERROR");
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
                Logger.LogActivity($"Circular circular data: {categoryJson}", "DEBUG");

                var circular = uow.CircularRepository.Get(t => t.Id == request.RecordId);
                if (circular != null) {
                    //..mark as delete this circular
                    _ = uow.CircularRepository.Delete(circular, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(circular).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete circular : {ex.Message}", "ERROR");
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
                Logger.LogActivity($"Circular circular data: {categoryJson}", "DEBUG");

                var circular = await uow.CircularRepository.GetAsync(t => t.Id == request.RecordId);
                if (circular != null) {
                    //..mark as delete this circular
                    _ = await uow.CircularRepository.DeleteAsync(circular, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(circular).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete circular : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<Circular>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<Circular, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Circulars", "INFO");

            try {
                return await uow.CircularRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve circulars : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<Circular>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<Circular, bool>> predicate = null) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged circulars", "INFO");

            try {
                return await uow.CircularRepository.PageAllAsync(page, size, includeDeleted, predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve circulars : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<CircularResponse>> PageLookupAsync<CircularResponse>(int page, int size, bool includeDeleted, Expression<Func<Circular, CircularResponse>> selector) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Circulars", "INFO");

            try {
                return await uow.CircularRepository.PageLookupAsync(page, size, includeDeleted, selector);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Circulars: {ex.Message}", "ERROR");
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
