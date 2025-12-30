using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Utils;
using System;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {

    public class StatutoryArticleService : BaseService, IStatutoryArticleService {

        public StatutoryArticleService(IServiceLoggerFactory loggerFactory,
            IUnitOfWorkFactory uowFactory,
            IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of statutory articles in the database", "INFO");

            try {
                return uow.StatutoryArticleRepository.Count();
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to statutory articles in the database: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public int Count(Expression<Func<StatutoryArticle, bool>> predicate) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of statutory articles in the database", "INFO");

            try {
                return uow.StatutoryArticleRepository.Count(predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count statutory regulations in the database: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of statutory articles in the database", "INFO");

            try {
                return await uow.StatutoryArticleRepository.CountAsync(cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count statutory articles in the database: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of statutory articles in the database", "INFO");

            try {
                return await uow.StatutoryArticleRepository.CountAsync(excludeDeleted, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count statutory articles in the database: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<StatutoryArticle, bool>> predicate, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of statutory articles in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.StatutoryArticleRepository.CountAsync(predicate, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count statutory articles in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<StatutoryArticle, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of statutory articles in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.StatutoryArticleRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count statutory articles in the database: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Exists(Expression<Func<StatutoryArticle, bool>> predicate, bool excludeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an statutory article exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.StatutoryArticleRepository.Exists(predicate, excludeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for statutory article in the database: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<StatutoryArticle, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an statutory article exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.StatutoryArticleRepository.ExistsAsync(predicate, excludeDeleted, token);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for statutory article in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<StatutoryArticle, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch statutory articles if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try {
                return await uow.StatutoryArticleRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for statutory articles in the database: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public StatutoryArticle Get(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get statutory article with ID '{id}'", "INFO");

            try {
                return uow.StatutoryArticleRepository.Get(id, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve statutory article: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public StatutoryArticle Get(Expression<Func<StatutoryArticle, bool>> predicate, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get statutory article that fits predicate >> '{predicate}'", "INFO");

            try {
                return uow.StatutoryArticleRepository.Get(predicate, includeDeleted);
            }  catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve statutory article : {ex.Message}", "ERROR");;

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public StatutoryArticle Get(Expression<Func<StatutoryArticle, bool>> predicate, bool includeDeleted = false, params Expression<Func<StatutoryArticle, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get statutory article that fits predicate >> '{predicate}'", "INFO");

            try {
                return uow.StatutoryArticleRepository.Get(predicate, includeDeleted, includes);
            }catch (Exception ex){
                Logger.LogActivity($"Failed to retrieve statutory article : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IQueryable<StatutoryArticle> GetAll(bool includeDeleted = false, params Expression<Func<StatutoryArticle, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all statutory articles", "INFO");

            try {
                return uow.StatutoryArticleRepository.GetAll(includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve statutory articles : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IList<StatutoryArticle> GetAll(bool includeDeleted = false){
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all statutory articles", "INFO");

            try{
                return uow.StatutoryArticleRepository.GetAll(includeDeleted);
            }catch (Exception ex){
                Logger.LogActivity($"Failed to retrieve statutory articles : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IList<StatutoryArticle> GetAll(Expression<Func<StatutoryArticle, bool>> predicate, bool includeDeleted){
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all statutory article that fit predicate '{predicate}'", "INFO");

            try{
                return uow.StatutoryArticleRepository.GetAll(predicate, includeDeleted);
            }catch (Exception ex){
                Logger.LogActivity($"Failed to retrieve statutory article : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<StatutoryArticle>> GetAllAsync(bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all statutory articles", "INFO");

            try{
                return await uow.StatutoryArticleRepository.GetAllAsync(includeDeleted);
            }catch (Exception ex){
                Logger.LogActivity($"Failed to retrieve statutory articles : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<StatutoryArticle>> GetAllAsync(Expression<Func<StatutoryArticle, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all statutory articles that fit predicate '{predicate}'", "INFO");

            try{
                return await uow.StatutoryArticleRepository.GetAllAsync(predicate, includeDeleted);
            }catch (Exception ex){
                Logger.LogActivity($"Failed to retrieve statutory articles : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<StatutoryArticle>> GetAllAsync(Expression<Func<StatutoryArticle, bool>> predicate, bool includeDeleted = false, params Expression<Func<StatutoryArticle, object>>[] includes){
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all statutory articles that fit predicate '{predicate}'", "INFO");

            try{
                return await uow.StatutoryArticleRepository.GetAllAsync(predicate, includeDeleted, includes);
            }catch (Exception ex){
                Logger.LogActivity($"Failed to retrieve statutory articles : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<StatutoryArticle>> GetAllAsync(bool includeDeleted = false, params Expression<Func<StatutoryArticle, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all statutory articles", "INFO");

            try{
                return await uow.StatutoryArticleRepository.GetAllAsync(includeDeleted, includes);
            }catch (Exception ex){
                Logger.LogActivity($"Failed to retrieve statutory articles : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<StatutoryArticle> GetAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get statutory article with ID '{id}'", "INFO");

            try{
                return await uow.StatutoryArticleRepository.GetAsync(id, includeDeleted);
            }catch (Exception ex){
                Logger.LogActivity($"Failed to retrieve statutory article : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<StatutoryArticle> GetAsync(Expression<Func<StatutoryArticle, bool>> predicate, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get statutory article that fit predicate '{predicate}'", "INFO");

            try{
                return await uow.StatutoryArticleRepository.GetAsync(predicate, includeDeleted);
            }catch (Exception ex){
                Logger.LogActivity($"Failed to retrieve statutory article : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }
        
        public async Task<StatutoryArticle> GetAsync(Expression<Func<StatutoryArticle, bool>> predicate, bool includeDeleted = false, params Expression<Func<StatutoryArticle, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get statutory article that fit predicate '{predicate}'", "INFO");

            try{
                return await uow.StatutoryArticleRepository.GetAsync(predicate, includeDeleted, includes);
            }catch (Exception ex){
                Logger.LogActivity($"Failed to retrieve statutory article : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<StatutoryArticle>> GetTopAsync(Expression<Func<StatutoryArticle, bool>> predicate, int top, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} statutory articles that fit predicate >> {predicate}", "INFO");

            try {
                return await uow.StatutoryArticleRepository.GetTopAsync(predicate, top, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve statutory article : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Insert(StatutoryArticleRequest request) {
            using var uow = UowFactory.Create();
            try {
                //..map statutory article request to Statutory Article entity
                var article = Mapper.Map<StatutoryArticleRequest, StatutoryArticle>(request);

                //..log the statutory article data being saved
                var articleJson = JsonSerializer.Serialize(article, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statutory article data: {articleJson}", "DEBUG");

                var added = uow.StatutoryArticleRepository.Insert(article);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(article).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save statutory article : {ex.Message}", "ERROR");;

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> InsertAsync(StatutoryArticleRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map statutory article request to statutory article entity
                var article = Mapper.Map<StatutoryArticleRequest, StatutoryArticle>(request);

                //..log the statutory article data being saved
                var articleJson = JsonSerializer.Serialize(article, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statutory article data: {articleJson}", "DEBUG");

                var added = await uow.StatutoryArticleRepository.InsertAsync(article);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(article).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save statutory article : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Update(StatutoryArticleRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update article request", "INFO");

            try {
                var artcle = uow.StatutoryArticleRepository.Get(a => a.Id == request.Id);
                if (artcle != null) {
                    //..update statutory article record
                    artcle.StatuteId = request.StatutoryId;
                    artcle.Article = (request.Section ?? string.Empty).Trim();
                    artcle.Summery = (request.Summery ?? string.Empty).Trim();
                    artcle.ObligationOrRequirement = request.Obligation;
                    artcle.IsMandatory = request.IsMandatory;
                    artcle.ExcludeFromCompliance = request.ExcludeFromCompliance;
                    artcle.Coverage = request.Coverage;
                    artcle.IsCovered = request.IsCovered;
                    artcle.FrequencyId = request.FrequencyId;
                    artcle.OwnerId = request.OwnerId;
                    artcle.ComplianceAssurance = request.ComplianceAssurance;
                    artcle.Comments = (request.Comments ?? string.Empty).Trim();
                    artcle.IsDeleted = request.IsDeleted;
                    artcle.LastModifiedOn = DateTime.Now;
                    artcle.LastModifiedBy = $"{request.ModifiedBy}";

                    //..check entity state
                    _ = uow.StatutoryArticleRepository.Update(artcle, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(artcle).State;
                    Logger.LogActivity($"Statutory article state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update statutory article record: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> UpdateAsync(StatutoryArticleRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update statutory article", "INFO");

            try {
                var artcle = await uow.StatutoryArticleRepository.GetAsync(a => a.Id == request.Id);
                if (artcle != null) {
                    //..update statutory article record
                    artcle.StatuteId = request.StatutoryId;
                    artcle.Article = (request.Section ?? string.Empty).Trim();
                    artcle.Summery = (request.Summery ?? string.Empty).Trim();
                    artcle.ObligationOrRequirement = request.Obligation;
                    artcle.IsMandatory = request.IsMandatory;
                    artcle.ExcludeFromCompliance = request.ExcludeFromCompliance;
                    artcle.Coverage = request.Coverage;
                    artcle.IsCovered = request.IsCovered;
                    artcle.ComplianceAssurance = request.ComplianceAssurance;
                    artcle.Comments = (request.Comments ?? string.Empty).Trim();
                    artcle.FrequencyId = request.FrequencyId;
                    artcle.OwnerId = request.OwnerId;
                    artcle.IsDeleted = request.IsDeleted;
                    artcle.LastModifiedOn = DateTime.Now;
                    artcle.LastModifiedBy = $"{request.ModifiedBy}";

                    //..check entity state
                    _ = await uow.StatutoryArticleRepository.UpdateAsync(artcle, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(artcle).State;
                    Logger.LogActivity($"Statutory article state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update statutory article record: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Delete(IdRequest request) {
            using var uow = UowFactory.Create();
            try {
                var auditJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statutory article data: {auditJson}", "DEBUG");

                var statute = uow.StatutoryArticleRepository.Get(t => t.Id == request.RecordId);
                if (statute != null) {
                    //..mark as delete this Statutory article
                    _ = uow.StatutoryArticleRepository.Delete(statute, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(statute).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Statutory article : {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> DeleteAsync(IdRequest request) {
            using var uow = UowFactory.Create();
            try {
                var statuteJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statutory article data: {statuteJson}", "DEBUG");

                var tasktask = await uow.StatutoryArticleRepository.GetAsync(t => t.Id == request.RecordId);
                if (tasktask != null) {
                    //..mark as delete this Statutory Regulation
                    _ = await uow.StatutoryArticleRepository.DeleteAsync(tasktask, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(tasktask).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete statutory article : {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> DeleteAllAsync(IList<long> requestItems, bool markAsDeleted = false) {
            using var uow = UowFactory.Create();
            try {
                var statutes = await uow.StatutoryArticleRepository.GetAllAsync(e => requestItems.Contains(e.Id));
                if (statutes.Count == 0)
                {
                    Logger.LogActivity($"Records not found", "INFO");
                    return false;
                }
                return await uow.StatutoryArticleRepository.DeleteAllAsync(statutes, markAsDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Statutory article: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyInsertAsync(StatutoryArticleRequest[] requestItems){
            using var uow = UowFactory.Create();
            try{
                //..map statutory regulation to statutory regulation entity
                var statutes = requestItems.Select(Mapper.Map<StatutoryArticleRequest, StatutoryArticle>).ToArray();
                var statuteJson = JsonSerializer.Serialize(statutes, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statutory article data: {statuteJson}", "DEBUG");
                return await uow.StatutoryArticleRepository.BulkyInsertAsync(statutes);
            }catch (Exception ex){
                Logger.LogActivity($"Failed to save statutory article : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(StatutoryArticleRequest[] requestItems) {
            using var uow = UowFactory.Create();
            try {
                //..map statutory article request to statutory article entity
                var statutes = requestItems.Select(Mapper.Map<StatutoryArticleRequest, StatutoryArticle>).ToArray();
                var statuteJson = JsonSerializer.Serialize(statutes, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statutory article data: {statuteJson}", "DEBUG");
                return await uow.StatutoryArticleRepository.BulkyUpdateAsync(statutes);
            } catch (Exception ex){
                Logger.LogActivity($"Failed to save statutory article : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(StatutoryArticleRequest[] requestItems, params Expression<Func<StatutoryArticle, object>>[] propertySelectors) {
            using var uow = UowFactory.Create();
            try {
                //..map statutory articles request to statutory articles entity
                var statutes = requestItems.Select(Mapper.Map<StatutoryArticleRequest, StatutoryArticle>).ToArray();
                var statuteJson = JsonSerializer.Serialize(statutes, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statutory articles data: {statuteJson}", "DEBUG");
                return await uow.StatutoryArticleRepository.BulkyUpdateAsync(statutes, propertySelectors);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save statutory articles : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<StatutoryArticle>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<StatutoryArticle, bool>> predicate = null, params Expression<Func<StatutoryArticle, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged statutory articles", "INFO");

            try {
                return await uow.StatutoryArticleRepository.PageAllAsync(page, size, includeDeleted, predicate, includes);
            } catch (Exception ex){
                Logger.LogActivity($"Failed to retrieve statutory articles: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<StatutoryArticle>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<StatutoryArticle, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged statutory articles", "INFO");

            try {
                return await uow.StatutoryArticleRepository.PageAllAsync(token, page, size, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieves statutory articles : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<StatutoryArticle>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<StatutoryArticle, bool>> predicate = null) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged statutory articles", "INFO");

            try {
                return await uow.StatutoryArticleRepository.PageAllAsync(page, size, includeDeleted, predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve statutory articles: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<StatutoryArticle>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<StatutoryArticle, bool>> predicate = null, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged statutory articles", "INFO");

            try {
                return await uow.StatutoryArticleRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve statutory articles : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<ObligationResponse> GetObligationAsync(Expression<Func<StatutoryArticle, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get obligation for statutory article that fits this predicate '{predicate}'", "INFO");
            try {
                Expression<Func<StatutoryArticle, ObligationResponse>> selector =
                    article => new ObligationResponse {
                        Id = article.Id,

                        Category = article.Statute.Category.CategoryName,
                        Statute = article.Statute.RegulatoryName,

                        Section = article.Article,
                        Summery = article.Summery,
                        IsMandatory = article.IsMandatory,
                        Obligation = article.ObligationOrRequirement,
                        Exclude = article.ExcludeFromCompliance,
                        Coverage = article.Coverage,
                        IsCovered = article.IsCovered,
                        Assurance = article.ComplianceAssurance,

                        ComplianceIssues = article.ComplianceIssues
                            .Where(i => !i.IsDeleted)
                            .Select(i => new ComplianceIssueResponse {
                                Id = i.Id,
                                ArticleId = i.Id,
                                Description = i.Description,
                                Comments = i.Notes,
                                IsDeleted = i.IsDeleted
                            }).ToList(),

                        Revisions = article.ArticleRevisions
                            .Where(r => !r.IsDeleted)
                            .Select(r => new ArticleRevisionResponse {
                                Id = r.Id,
                                ArticleId = r.ArticleId,
                                Section = r.Section,
                                Revision = r.Revision,
                                Comments = r.Comments
                            }).ToList(),

                        ComplianceMaps = article.StatutoryArticleControls
                            .GroupBy(sac => sac.ControlItem.ControlCategory)
                            .Select(categoryGroup => new ObligationComplianceMapResponse {
                                Id = categoryGroup.Key.Id,
                                CategoryName = categoryGroup.Key.CategoryName,
                                Comments = categoryGroup.Key.Notes,
                                Exclude = !categoryGroup.Key.Exclude,

                                Items = categoryGroup
                                    .Select(sac => sac.ControlItem)
                                    .Distinct()
                                    .Select(ci => new ObligationComplianceItemResponse {
                                        Id = ci.Id,
                                        CategoryId = ci.ControlCategoryId,
                                        ItemName = ci.ItemName,
                                        Comments = ci.Notes,
                                        Exclude = !ci.Exclude
                                    }).ToList()
                            }).ToList()
                    };

                var record = await uow.StatutoryArticleRepository.GetLookupAsync(predicate, selector, includeDeleted);
                return record;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve statutory article obligation: {ex.Message}", "ERROR");
                //..save error object to the database
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
