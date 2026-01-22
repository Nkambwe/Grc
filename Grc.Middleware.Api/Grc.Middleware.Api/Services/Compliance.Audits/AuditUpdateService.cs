using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Audits {

    public class AuditUpdateService : BaseService, IAuditUpdateService {

        public AuditUpdateService(IServiceLoggerFactory loggerFactory, IUnitOfWorkFactory uowFactory, IMapper mapper) 
            : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit notes", "INFO");

            try {
                return uow.AuditTypeRepository.Count();
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count audit notes in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public int Count(Expression<Func<AuditUpdate, bool>> predicate) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit notes in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.AuditUpdateRepository.Count(predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count audit notes in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<AuditUpdate, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit notes in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.AuditUpdateRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count audit notes in the database: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Exists(Expression<Func<AuditUpdate, bool>> predicate, bool excludeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Audit notes exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.AuditUpdateRepository.Exists(predicate, excludeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for Audit notes in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<AuditUpdate, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Audit notes exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.AuditUpdateRepository.ExistsAsync(predicate, excludeDeleted, token);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for Audit notes in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<AuditUpdate> GetAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Audit notes with ID '{id}'", "INFO");

            try {
                return await uow.AuditUpdateRepository.GetAsync(id, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Audit notes : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<AuditUpdate> GetAsync(Expression<Func<AuditUpdate, bool>> predicate, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Audit notes that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.AuditUpdateRepository.GetAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Audit notes : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<AuditUpdate> GetAsync(Expression<Func<AuditUpdate, bool>> predicate, bool includeDeleted = false, params Expression<Func<AuditUpdate, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Audit type that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.AuditUpdateRepository.GetAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Audit notes : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<AuditUpdate>> GetAllAsync(Expression<Func<AuditUpdate, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Audit notes", "INFO");

            try {
                return await uow.AuditUpdateRepository.GetAllAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Audit notes : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<AuditUpdate>> GetAllAsync(Expression<Func<AuditUpdate, bool>> predicate, bool includeDeleted = false, params Expression<Func<AuditUpdate, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Audit notes", "INFO");

            try {
                return await uow.AuditUpdateRepository.GetAllAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Audit notes : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<AuditUpdate>> GetAllAsync(bool includeDeleted = false, params Expression<Func<AuditUpdate, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Audit notes", "INFO");

            try {
                return await uow.AuditUpdateRepository.GetAllAsync(includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Audit notes : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<AuditUpdate>> GetAllAsync(Expression<Func<AuditUpdate, bool>> predicate, bool includeDeleted = false, Func<IQueryable<AuditUpdate>, IQueryable<AuditUpdate>> includes = null) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Audit notes", "INFO");

            try {
                return await uow.AuditUpdateRepository.GetAllAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Audit notes : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> InsertAsync(AuditUpdateRequest request, string username) {
            using var uow = UowFactory.Create();
            try {
                //..map record to entity
                var notes = new AuditUpdate() {
                    ReportId = request.ReportId,
                    Notes = (request.UpdateNotes ?? string.Empty).Trim(),
                    NoteDate = DateTime.Now,
                    SendReminder = request.SendReminders,
                    SendReminderOn = request.SendDate,
                    SendTo = request.SendToEmails,
                    ReminderMessage = request.ReminderMessage,
                    AddedBy = request.AddedBy ?? string.Empty,
                    IsDeleted = request.IsDeleted,
                    CreatedOn = DateTime.Now,
                    CreatedBy = $"{username}"
                };

                //..log the en tity data being saved
                var categoryJson = JsonSerializer.Serialize(notes, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit notes data: {categoryJson}", "DEBUG");

                var added = await uow.AuditUpdateRepository.InsertAsync(notes);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(notes).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save Audit notes : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> UpdateAsync(AuditUpdateRequest request, string username, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update audit notes request", "INFO");

            try {
                var notes = await uow.AuditUpdateRepository.GetAsync(a => a.Id == request.Id, includeDeleted);
                if (notes != null) {
                    notes.Notes = (request.UpdateNotes ?? string.Empty).Trim();
                    notes.IsDeleted = request.IsDeleted;
                    notes.SendReminder = request.SendReminders;
                    notes.SendReminderOn = request.SendDate;
                    notes.SendTo = request.SendToEmails;
                    notes.ReminderMessage = request.ReminderMessage;
                    notes.AddedBy = request.AddedBy ?? string.Empty;
                    notes.LastModifiedOn = DateTime.Now;
                    notes.LastModifiedBy = $"{username}";

                    //..check entity state
                    _ = await uow.AuditUpdateRepository.UpdateAsync(notes, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(notes).State;
                    Logger.LogActivity($"Audit notes state after Update: {entityState}", "DEBUG");

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
                Logger.LogActivity($"Audit notes data: {categoryJson}", "DEBUG");

                var circular = uow.AuditUpdateRepository.Get(t => t.Id == request.RecordId);
                if (circular != null) {
                    //..mark as delete this notes
                    _ = uow.AuditUpdateRepository.Delete(circular, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(circular).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete notes : {ex.Message}", "ERROR");
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
                Logger.LogActivity($"Audit notes data: {categoryJson}", "DEBUG");

                var task = await uow.AuditUpdateRepository.GetAsync(t => t.Id == request.RecordId);
                if (task != null) {
                    //..mark as delete this type
                    _ = await uow.AuditUpdateRepository.DeleteAsync(task, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(task).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete notes : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<AuditUpdate>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<AuditUpdate, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audit notes", "INFO");

            try {
                return await uow.AuditUpdateRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve audit notes : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<AuditUpdate>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditUpdate, bool>> predicate = null) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audit notes", "INFO");

            try {
                return await uow.AuditUpdateRepository.PageAllAsync(page, size, includeDeleted, predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve audit notes : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<AuditUpdateResponse>> PageLookupAsync<AuditUpdateResponse>(int page, int size, bool includeDeleted, Expression<Func<AuditUpdate, AuditUpdateResponse>> selector) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audit notes", "INFO");

            try {
                return await uow.AuditUpdateRepository.PageLookupAsync(page, size, includeDeleted, selector);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve audit notes: {ex.Message}", "ERROR");
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
