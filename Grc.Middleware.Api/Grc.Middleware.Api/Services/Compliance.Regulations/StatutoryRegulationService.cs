using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {

    public class StatutoryRegulationService : BaseService, IStatutoryRegulationService {
        
        public StatutoryRegulationService(IServiceLoggerFactory loggerFactory, IUnitOfWorkFactory uowFactory,
            IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count()
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of regulations in the database", "INFO");

            try
            {
                return uow.StatutoryRegulationRepository.Count();
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to regulations in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public int Count(Expression<Func<StatutoryRegulation, bool>> predicate)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of statutory regulations in the database", "INFO");

            try
            {
                return uow.StatutoryRegulationRepository.Count(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count statutory regulationsin the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of statutory regulations in the database", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count statutory regulations in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of statutory regulations in the database", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.CountAsync(excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count statutory regulations in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<StatutoryRegulation, bool>> predicate, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of statutory regulations in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.CountAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count statutory regulations in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<StatutoryRegulation, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of statutory regulations in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to countstatutory regulations in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Exists(Expression<Func<StatutoryRegulation, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an statutory regulation exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.StatutoryRegulationRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for statutory regulation in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<StatutoryRegulation, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an statutory regulation exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for statutory regulation in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<StatutoryRegulation, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch regulations if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for statutory regulations in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public StatutoryRegulation Get(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get statutory regulation with ID '{id}'", "INFO");

            try
            {
                return uow.StatutoryRegulationRepository.Get(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulation: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public StatutoryRegulation Get(Expression<Func<StatutoryRegulation, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get statutory regulation that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.StatutoryRegulationRepository.Get(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulation : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public StatutoryRegulation Get(Expression<Func<StatutoryRegulation, bool>> predicate, bool includeDeleted = false, params Expression<Func<StatutoryRegulation, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get statutory regulation that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.StatutoryRegulationRepository.Get(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulation : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IQueryable<StatutoryRegulation> GetAll(bool includeDeleted = false, params Expression<Func<StatutoryRegulation, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all statutory regulations", "INFO");

            try
            {
                return uow.StatutoryRegulationRepository.GetAll(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulations : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IList<StatutoryRegulation> GetAll(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all statutory regulations", "INFO");

            try
            {
                return uow.StatutoryRegulationRepository.GetAll(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulations : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IList<StatutoryRegulation> GetAll(Expression<Func<StatutoryRegulation, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all statutory regulation that fit predicate '{predicate}'", "INFO");

            try
            {
                return uow.StatutoryRegulationRepository.GetAll(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulation : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<StatutoryRegulation>> GetAllAsync(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all statutory regulations", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.GetAllAsync(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulations : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<StatutoryRegulation>> GetAllAsync(Expression<Func<StatutoryRegulation, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all statutory regulations that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.GetAllAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulations : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<StatutoryRegulation>> GetAllAsync(Expression<Func<StatutoryRegulation, bool>> predicate, bool includeDeleted = false, params Expression<Func<StatutoryRegulation, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all statutory regulations that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.GetAllAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulations : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<StatutoryRegulation>> GetAllAsync(bool includeDeleted = false, params Expression<Func<StatutoryRegulation, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all statutory regulations", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.GetAllAsync(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulations : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<StatutoryRegulation> GetAsync(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get statutory regulation with ID '{id}'", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.GetAsync(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulation : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<StatutoryRegulation> GetAsync(Expression<Func<StatutoryRegulation, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get statutory regulation that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.GetAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulation : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<StatutoryRegulation> GetAsync(Expression<Func<StatutoryRegulation, bool>> predicate, bool includeDeleted = false, params Expression<Func<StatutoryRegulation, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get statutory regulation that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.GetAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulation : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<StatutoryRegulation>> GetTopAsync(Expression<Func<StatutoryRegulation, bool>> predicate, int top, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} statutory regulations that fit predicate >> {predicate}", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.GetTopAsync(predicate, top, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulation : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Insert(StatutoryRegulationRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map statutory regulation request tostatutory regulation entity
                var statute = Mapper.Map<StatutoryRegulationRequest, StatutoryRegulation>(request);

                //..log the statutory regulation data being saved
                var auditTaskJson = JsonSerializer.Serialize(statute, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statutory regulation data: {auditTaskJson}", "DEBUG");

                var added = uow.StatutoryRegulationRepository.Insert(statute);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(statute).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save statutory regulation : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> InsertAsync(StatutoryRegulationRequest request) {
            using var uow = UowFactory.Create();
            try {
                //..map statutory regulation request to statutory regulation entity
                var statute = Mapper.Map<StatutoryRegulationRequest, StatutoryRegulation>(request);

                //..log the statutory regulation data being saved
                var auditTaaskJson = JsonSerializer.Serialize(statute, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statutory regulation data: {auditTaaskJson}", "DEBUG");

                var added = await uow.StatutoryRegulationRepository.InsertAsync(statute);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(statute).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save statutory regulation : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Update(StatutoryRegulationRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update request", "INFO");

            try
            {
                var statute = uow.StatutoryRegulationRepository.Get(a => a.Id == request.Id);
                if (statute != null)
                {
                    //..update statutory Regulation record
                    statute.Code = (request.Code ?? string.Empty).Trim();
                    statute.RegulatoryName = (request.RegulatoryName ?? string.Empty).Trim();
                    statute.TypeId = request.TypeId;
                    statute.AuthorityId = request.AuthorityId;
                    statute.CategoryId = request.CategoryId;
                    statute.IsDeleted = request.IsDeleted;
                    statute.LastModifiedOn = DateTime.Now;
                    statute.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = uow.StatutoryRegulationRepository.Update(statute, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(statute).State;
                    Logger.LogActivity($"Statutory Regulation state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update statutory Regulation record: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> UpdateAsync(StatutoryRegulationRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update statutory Regulation", "INFO");

            try
            {
                var statute = await uow.StatutoryRegulationRepository.GetAsync(a => a.Id == request.Id);
                if (statute != null)
                {
                    //..update statutory Regulation record
                    statute.Code = (request.Code ?? string.Empty).Trim();
                    statute.RegulatoryName = (request.RegulatoryName ?? string.Empty).Trim();
                    statute.TypeId = request.TypeId;
                    statute.AuthorityId = request.AuthorityId;
                    statute.CategoryId = request.CategoryId;
                    statute.IsDeleted = request.IsDeleted;
                    statute.LastModifiedOn = DateTime.Now;
                    statute.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = await uow.StatutoryRegulationRepository.UpdateAsync(statute, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(statute).State;
                    Logger.LogActivity($"Statutory Regulation state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update statutory Regulation record: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Delete(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var auditJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statutory Regulation data: {auditJson}", "DEBUG");

                var statute = uow.StatutoryRegulationRepository.Get(t => t.Id == request.RecordId);
                if (statute != null)
                {
                    //..mark as delete this Statutory Regulation
                    _ = uow.StatutoryRegulationRepository.Delete(statute, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(statute).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Statutory Regulation : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> DeleteAsync(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var statuteJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statutory Regulation data: {statuteJson}", "DEBUG");

                var tasktask = await uow.StatutoryRegulationRepository.GetAsync(t => t.Id == request.RecordId);
                if (tasktask != null)
                {
                    //..mark as delete this Statutory Regulation
                    _ = await uow.StatutoryRegulationRepository.DeleteAsync(tasktask, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(tasktask).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete statutory Regulation : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> DeleteAllAsync(IList<long> requesItems, bool markAsDeleted = false)
        {
            using var uow = UowFactory.Create();
            try
            {
                var statutes = await uow.StatutoryRegulationRepository.GetAllAsync(e => requesItems.Contains(e.Id));
                if (statutes.Count == 0)
                {
                    Logger.LogActivity($"Records not found", "INFO");
                    return false;
                }
                return await uow.StatutoryRegulationRepository.DeleteAllAsync(statutes, markAsDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete sStatutory Regulation: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyInsertAsync(StatutoryRegulationRequest[] requesItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map statuitory regulation to statuitory regulation entity
                var statutes = requesItems.Select(Mapper.Map<StatutoryRegulationRequest, StatutoryRegulation>).ToArray();
                var statuteJson = JsonSerializer.Serialize(statutes, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statuitory regulation data: {statuteJson}", "DEBUG");
                return await uow.StatutoryRegulationRepository.BulkyInsertAsync(statutes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save statuitory regulation : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(StatutoryRegulationRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map statuitory regulation request to statuitory regulation entity
                var statutes = requestItems.Select(Mapper.Map<StatutoryRegulationRequest, StatutoryRegulation>).ToArray();
                var statuteJson = JsonSerializer.Serialize(statutes, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statuitory regulation data: {statuteJson}", "DEBUG");
                return await uow.StatutoryRegulationRepository.BulkyUpdateAsync(statutes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save statuitory regulation : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(StatutoryRegulationRequest[] requestItems, params Expression<Func<StatutoryRegulation, object>>[] propertySelectors)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map statutory regulations request to statutory regulations entity
                var statutes = requestItems.Select(Mapper.Map<StatutoryRegulationRequest, StatutoryRegulation>).ToArray();
                var statuteJson = JsonSerializer.Serialize(statutes, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statutory regulations data: {statuteJson}", "DEBUG");
                return await uow.StatutoryRegulationRepository.BulkyUpdateAsync(statutes, propertySelectors);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save statutory regulations : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<StatutoryRegulation>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<StatutoryRegulation, bool>> predicate = null, params Expression<Func<StatutoryRegulation, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged statutory regulations", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.PageAllAsync(page, size, includeDeleted, predicate, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulations: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<StatutoryRegulation>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<StatutoryRegulation, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged statutory regulations", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.PageAllAsync(token, page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieves statutory regulations : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<StatutoryRegulation>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<StatutoryRegulation, bool>> predicate = null)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged statutory regulations", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.PageAllAsync(page, size, includeDeleted, predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulations: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<StatutoryRegulation>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<StatutoryRegulation, bool>> predicate = null, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged statutory regulations", "INFO");

            try
            {
                return await uow.StatutoryRegulationRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve statutory regulations : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<StatuteSupportResponse> GetStatuteSupportItemsAsync(bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve support items for compliance laws and regulations", "INFO");

            try {

                StatuteSupportResponse response = new() {
                    Frequencies = new(),
                    Authorities = new(),
                    Responsibilities = new(),
                    StatuteTypes = new(),
                    Departments = new()
                };


                // get policy types
                var types = await uow.RegulatoryTypeRepository.GetAllAsync(false);

                //..get frequencies
                var frequencies = await uow.FrequencyRepository.GetAllAsync(false);

                // get authorities
                var authorities = await uow.AuthoritiesRepository.GetAllAsync(false);

                //..get department details
                var departments = await uow.DepartmentRepository.GetAllAsync(false, d => d.Responsibilities);

                //..populate frequencies    
                if (frequencies != null && frequencies.Count > 0) {
                    response.Frequencies.AddRange(
                        from frequency in frequencies
                        select new FrequencyResponse {
                            Id = frequency.Id,
                            FrequencyName = frequency.FrequencyName
                        });
                    Logger.LogActivity($"Return Frequency found: {frequencies.Count}", "DEBUG");
                }

                //..populate departments
                if (departments != null && departments.Count > 0) {

                    foreach (var dept in departments) {
                        //..add department
                        response.Departments.Add(new PolicyDepartmentResponse {
                            Id = dept.Id,
                            DepartmentName = dept.DepartmentName
                        });

                        //..add responsibilities
                        var owners = dept.Responsibilities;
                        if (owners != null && owners.Count > 0) {
                            response.Responsibilities.AddRange(
                                from owner in owners
                                select new ResponsibilityItemResponse {
                                    Id = owner.Id,
                                    DepartmentName = dept.DepartmentName,
                                    ResponsibilityRole = owner.ContactPosition
                                });
                            Logger.LogActivity($"Department Responsibilities found: {owners.Count}", "DEBUG");
                        }
                    }

                }

                //..authorities
                if (authorities != null && authorities.Count > 0) {
                    response.Authorities.AddRange(
                        from authority in authorities
                        select new RegulatoryAuthorityResponse {
                            Id = authority.Id,
                            AuthorityAlias = authority.AuthorityAlias,
                            AuthorityName = authority.AuthorityName,
                            IsDeleted = authority.IsDeleted,
                            CreatedOn = authority.CreatedOn,
                            UpdatedOn = authority.LastModifiedOn ?? DateTime.Now
                        });
                    Logger.LogActivity($"Regulatory Authorities found: {authorities.Count}", "DEBUG");
                }

                //..regulatory types
                if (types != null && types.Count > 0) {
                    response.StatuteTypes.AddRange(
                        from type in types
                        select new StatuteTypeResponse {
                            Id = type.Id,
                            TypeName = type.TypeName
                        });
                    Logger.LogActivity($"Regulatory types found: {types.Count}", "DEBUG");
                }

                return response;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve policy support items: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PolicySupportResponse> GetSupportItemsAsync(bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve support items for operations policies", "INFO");

            try {

                PolicySupportResponse response = new() {
                    Frequencies = new(),
                    Authorities = new(),
                    Responsibilities = new(),
                    RegulatoryTypes = new(),
                    Departments = new(),
                    ReturnTypes = new(),
                    EnforcementLaws = new()
                };

                
                // get policy types
                var types = await uow.RegulatoryDocumentTypeRepository.GetAllAsync(false);

                //..get frequencies
                var frequencies = await uow.FrequencyRepository.GetAllAsync(false);

                // get authorities
                var authorities = await uow.AuthoritiesRepository.GetAllAsync(false);

                // get acts and laws
                var acts = await uow.StatutoryArticleRepository.GetAllAsync(false);

                // get return types
                var returnTypes = await uow.ReturnTypeRepository.GetAllAsync(false);

                //..get department details
                var departments = await uow.DepartmentRepository.GetAllAsync(false, d => d.Responsibilities);

                //..populate frequencies    
                if (frequencies != null && frequencies.Count > 0) {
                    response.Frequencies.AddRange(
                        from frequency in frequencies
                        select new FrequencyResponse {
                            Id = frequency.Id,
                            FrequencyName = frequency.FrequencyName
                        });
                    Logger.LogActivity($"Return Frequency found: {frequencies.Count}", "DEBUG");
                }

                //..populate departments
                if (departments != null && departments.Count > 0) {

                    foreach(var dept in departments) {
                        //..add department
                        response.Departments.Add(new PolicyDepartmentResponse {
                            Id = dept.Id,
                            DepartmentName = dept.DepartmentName
                        });

                        //..add responsibilities
                        var owners = dept.Responsibilities;
                        if (owners != null && owners.Count > 0) {
                            response.Responsibilities.AddRange(
                                from owner in owners
                                select new ResponsibilityItemResponse {
                                    Id = owner.Id,
                                    DepartmentName = dept.DepartmentName,
                                    ResponsibilityRole = owner.ContactPosition
                                });
                            Logger.LogActivity($"Department Responsibilities found: {owners.Count}", "DEBUG");
                        }
                    }
                    
                }

                //..authorities
                if (authorities != null && authorities.Count > 0) {
                    response.Authorities.AddRange(
                        from authority in authorities
                        select new RegulatoryAuthorityResponse {
                            Id = authority.Id,
                            AuthorityAlias = authority.AuthorityAlias,
                            AuthorityName = authority.AuthorityName,
                            IsDeleted = authority.IsDeleted,
                            CreatedOn = authority.CreatedOn,
                            UpdatedOn = authority.LastModifiedOn ?? DateTime.Now
                        });
                    Logger.LogActivity($"Regulatory Authorities found: {authorities.Count}", "DEBUG");
                }

                //..regulatory types
                if (types != null && types.Count > 0) {
                    response.RegulatoryTypes.AddRange(
                        from type in types
                        select new RegulatoryTypeResponse {
                            Id = type.Id,
                            TypeName = type.DocumentType
                        });
                    Logger.LogActivity($"Regulatory types found: {types.Count}", "DEBUG");
                }

                //..return types
                if (returnTypes != null && returnTypes.Count > 0) {
                    response.ReturnTypes.AddRange(
                        from type in returnTypes
                        select new ReturnTypeResponse {
                            Id = type.Id,
                            TypeName = type.TypeName
                        });
                    Logger.LogActivity($"Return types found: {returnTypes.Count}", "DEBUG");
                }

                //..enabling laws
                if (acts != null && acts.Count > 0) {
                    response.EnforcementLaws.AddRange(
                        from act in acts
                        select new MiniObligationActResponse {
                            Id = act.Id,
                            Section = act.Article,
                            Requirement = act.Summery
                        });
                    Logger.LogActivity($"Statutory Laws found: {acts.Count}", "DEBUG");
                }

                return response;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve policy support items: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
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
                ErrorSource = "STATUTORY-REGISTER-SERVICE",
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
