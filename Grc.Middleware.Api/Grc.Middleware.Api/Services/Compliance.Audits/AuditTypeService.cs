using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Audits {

    public class AuditTypeService : BaseService, IAuditTypeService {

        public AuditTypeService(IServiceLoggerFactory loggerFactory, IUnitOfWorkFactory uowFactory, IMapper mapper) 
            : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit type", "INFO");

            try {
                return uow.AuditTypeRepository.Count();
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count audit type in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public int Count(Expression<Func<AuditType, bool>> predicate) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit type in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.AuditTypeRepository.Count(predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count audit type in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<AuditType, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit type in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.AuditTypeRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count audit type in the database: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Exists(Expression<Func<AuditType, bool>> predicate, bool excludeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Audit type exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.AuditTypeRepository.Exists(predicate, excludeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for Audit type in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<AuditType, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Audit type exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.AuditTypeRepository.ExistsAsync(predicate, excludeDeleted, token);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for Audit type in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<AuditType> GetAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Audit type with ID '{id}'", "INFO");

            try {
                return await uow.AuditTypeRepository.GetAsync(id, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Audit type : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<AuditType> GetAsync(Expression<Func<AuditType, bool>> predicate, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Audit type that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.AuditTypeRepository.GetAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Audit type : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<AuditType> GetAsync(Expression<Func<AuditType, bool>> predicate, bool includeDeleted = false, params Expression<Func<AuditType, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Audit type that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.AuditTypeRepository.GetAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Audit type : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<AuditType>> GetAllAsync(Expression<Func<AuditType, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Audit type", "INFO");

            try {
                return await uow.AuditTypeRepository.GetAllAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Audit type : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<AuditType>> GetAllAsync(Expression<Func<AuditType, bool>> predicate, bool includeDeleted = false, params Expression<Func<AuditType, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Audit type", "INFO");

            try {
                return await uow.AuditTypeRepository.GetAllAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Audit type : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<AuditType>> GetAllAsync(bool includeDeleted = false, params Expression<Func<AuditType, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Audit type", "INFO");

            try {
                return await uow.AuditTypeRepository.GetAllAsync(includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Audit type : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<AuditType>> GetAllAsync(Expression<Func<AuditType, bool>> predicate, bool includeDeleted = false, Func<IQueryable<AuditType>, IQueryable<AuditType>> includes = null) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Audit type", "INFO");

            try {
                return await uow.AuditTypeRepository.GetAllAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Audit type : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> InsertAsync(AuditTypeRequest request, string username) {
            using var uow = UowFactory.Create();
            try {
                //..map record to entity
                var type = new AuditType() {
                    TypeCode = (request.TypeCode ?? string.Empty).Trim(),
                    TypeName = (request.TypeName ?? string.Empty).Trim(),
                    Description = request.Description,
                    IsDeleted = request.IsDeleted,
                    CreatedBy = $"{username}",
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = $"{username}",
                    LastModifiedOn = DateTime.Now
                };

                //..log the en tity data being saved
                var categoryJson = JsonSerializer.Serialize(type, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit type data: {categoryJson}", "DEBUG");

                var added = await uow.AuditTypeRepository.InsertAsync(type);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(type).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save Audit type : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> UpdateAsync(AuditTypeRequest request, string username, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update audit type request", "INFO");

            try {
                var type = await uow.AuditTypeRepository.GetAsync(a => a.Id == request.Id, includeDeleted);
                if (type != null) {
                    type.TypeCode = (request.TypeCode ?? string.Empty).Trim();
                    type.TypeName = (request.TypeName ?? string.Empty).Trim();
                    type.IsDeleted = request.IsDeleted;
                    type.Description = request.Description;
                    type.LastModifiedOn = DateTime.Now;
                    type.LastModifiedBy = $"{username}";

                    //..check entity state
                    _ = await uow.AuditTypeRepository.UpdateAsync(type, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(type).State;
                    Logger.LogActivity($"Audit type state after Update: {entityState}", "DEBUG");

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
                Logger.LogActivity($"Audit type data: {categoryJson}", "DEBUG");

                var circular = uow.AuditTypeRepository.Get(t => t.Id == request.RecordId);
                if (circular != null) {
                    //..mark as delete this type
                    _ = uow.AuditTypeRepository.Delete(circular, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(circular).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete type : {ex.Message}", "ERROR");
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
                Logger.LogActivity($"Audit type data: {categoryJson}", "DEBUG");

                var task = await uow.AuditTypeRepository.GetAsync(t => t.Id == request.RecordId);
                if (task != null) {
                    //..mark as delete this type
                    _ = await uow.AuditTypeRepository.DeleteAsync(task, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(task).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete type : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<AuditType>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<AuditType, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audit type", "INFO");

            try {
                return await uow.AuditTypeRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve audit type : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<AuditType>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditType, bool>> predicate = null) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audit type", "INFO");

            try {
                return await uow.AuditTypeRepository.PageAllAsync(page, size, includeDeleted, predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve audit type : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<AuditTypeResponse>> PageLookupAsync<AuditTypeResponse>(int page, int size, bool includeDeleted, Expression<Func<AuditType, AuditTypeResponse>> selector) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audit type", "INFO");

            try {
                return await uow.AuditTypeRepository.PageLookupAsync(page, size, includeDeleted, selector);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve audit type: {ex.Message}", "ERROR");
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
