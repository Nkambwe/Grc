using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {

    public class ArticleRevisionService : BaseService, IArticleRevisionService {

        public ArticleRevisionService(IServiceLoggerFactory loggerFactory,
            IUnitOfWorkFactory uowFactory,
            IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of article revisions in the database", "INFO");

            try {
                return uow.ArticleRevisionRepository.Count();
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to article revisions in the database: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public int Count(Expression<Func<ArticleRevision, bool>> predicate) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of article revisions in the database", "INFO");

            try {
                return uow.ArticleRevisionRepository.Count(predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count article revisions in the database: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of article revisions in the database", "INFO");

            try {
                return await uow.ArticleRevisionRepository.CountAsync(cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count article revisions in the database: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of article revisons in the database", "INFO");

            try {
                return await uow.ArticleRevisionRepository.CountAsync(excludeDeleted, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count article revisons in the database: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<ArticleRevision, bool>> predicate, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of article revisions in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.ArticleRevisionRepository.CountAsync(predicate, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count article revisions in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<ArticleRevision, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of article revisions in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.ArticleRevisionRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count article revisions  in the database: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Exists(Expression<Func<ArticleRevision, bool>> predicate, bool excludeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an article revision exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.ArticleRevisionRepository.Exists(predicate, excludeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for article revison in the database: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<ArticleRevision, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an article revision exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.ArticleRevisionRepository.ExistsAsync(predicate, excludeDeleted, token);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for article revision in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ArticleRevision, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch article revision if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try {
                return await uow.ArticleRevisionRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for article revision in the database: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public ArticleRevision Get(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get article revision with ID '{id}'", "INFO");

            try {
                return uow.ArticleRevisionRepository.Get(id, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revision : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public ArticleRevision Get(Expression<Func<ArticleRevision, bool>> predicate, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get article revision that fits predicate >> '{predicate}'", "INFO");

            try {
                return uow.ArticleRevisionRepository.Get(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revision : {ex.Message}", "ERROR"); ;

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public ArticleRevision Get(Expression<Func<ArticleRevision, bool>> predicate, bool includeDeleted = false, params Expression<Func<ArticleRevision, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get article revision that fits predicate >> '{predicate}'", "INFO");

            try {
                return uow.ArticleRevisionRepository.Get(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revision : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IQueryable<ArticleRevision> GetAll(bool includeDeleted = false, params Expression<Func<ArticleRevision, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all article revision", "INFO");

            try {
                return uow.ArticleRevisionRepository.GetAll(includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revision : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IList<ArticleRevision> GetAll(bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all article revisions", "INFO");

            try {
                return uow.ArticleRevisionRepository.GetAll(includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revisions : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IList<ArticleRevision> GetAll(Expression<Func<ArticleRevision, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all article revision that fit predicate '{predicate}'", "INFO");

            try {
                return uow.ArticleRevisionRepository.GetAll(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revision : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ArticleRevision>> GetAllAsync(bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all article revisions", "INFO");

            try {
                return await uow.ArticleRevisionRepository.GetAllAsync(includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revisions : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ArticleRevision>> GetAllAsync(Expression<Func<ArticleRevision, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all article revisions that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ArticleRevisionRepository.GetAllAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revisions : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ArticleRevision>> GetAllAsync(Expression<Func<ArticleRevision, bool>> predicate, bool includeDeleted = false, params Expression<Func<ArticleRevision, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all article revisions that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ArticleRevisionRepository.GetAllAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revisions : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ArticleRevision>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ArticleRevision, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all article revisions", "INFO");

            try {
                return await uow.ArticleRevisionRepository.GetAllAsync(includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revisions : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<ArticleRevision> GetAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get article revision with ID '{id}'", "INFO");

            try {
                return await uow.ArticleRevisionRepository.GetAsync(id, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revision: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<ArticleRevision> GetAsync(Expression<Func<ArticleRevision, bool>> predicate, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get article revision that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ArticleRevisionRepository.GetAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revision : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<ArticleRevision> GetAsync(Expression<Func<ArticleRevision, bool>> predicate, bool includeDeleted = false, params Expression<Func<ArticleRevision, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get article revision that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ArticleRevisionRepository.GetAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revision : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ArticleRevision>> GetTopAsync(Expression<Func<ArticleRevision, bool>> predicate, int top, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} article revision that fit predicate >> {predicate}", "INFO");

            try {
                return await uow.ArticleRevisionRepository.GetTopAsync(predicate, top, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to article revision article : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Insert(ArticleRevisionRequest request) {
            using var uow = UowFactory.Create();
            try {
                //..map request
                var revision = Mapper.Map<ArticleRevisionRequest, ArticleRevision>(request);

                //..log the article revision data being saved
                var articleJson = JsonSerializer.Serialize(revision, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Article revision data: {articleJson}", "DEBUG");

                var added = uow.ArticleRevisionRepository.Insert(revision);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(revision).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save article revision : {ex.Message}", "ERROR"); ;

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> InsertAsync(ArticleRevisionRequest request) {
            using var uow = UowFactory.Create();
            try {
                //..map request
                var revision = Mapper.Map<ArticleRevisionRequest, ArticleRevision>(request);

                //..log the article revision data being saved
                var articleJson = JsonSerializer.Serialize(revision, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Article revision data: {articleJson}", "DEBUG");

                var added = await uow.ArticleRevisionRepository.InsertAsync(revision);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(revision).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save article revision : {ex.Message}", "ERROR"); ;

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Update(ArticleRevisionRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update article revision request", "INFO");

            try {
                var artcle = uow.ArticleRevisionRepository.Get(a => a.Id == request.Id);
                if (artcle != null) {
                    //..update article revision record
                    artcle.ArticleId = request.ArticleId;
                    artcle.Section = (request.Section ?? string.Empty).Trim();
                    artcle.Summery = (request.Summery ?? string.Empty).Trim();
                    artcle.Revision = (request.Revision ?? string.Empty).Trim();
                    artcle.Comments = (request.Comments ?? string.Empty).Trim();
                    artcle.ReviewedOn = request.ReviewedOn;
                    artcle.IsDeleted = request.IsDeleted;
                    artcle.LastModifiedOn = DateTime.Now;
                    artcle.LastModifiedBy = $"{request.ModifiedBy}";

                    //..check entity state
                    _ = uow.ArticleRevisionRepository.Update(artcle, includeDeleted
                        );
                    var entityState = ((UnitOfWork)uow).Context.Entry(artcle).State;
                    Logger.LogActivity($"Article revision state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update article revision record: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ArticleRevisionRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update article revision request", "INFO");

            try {
                var artcle = uow.ArticleRevisionRepository.Get(a => a.Id == request.Id);
                if (artcle != null) {
                    //..update article revision record
                    artcle.ArticleId = request.ArticleId;
                    artcle.Section = (request.Section ?? string.Empty).Trim();
                    artcle.Summery = (request.Summery ?? string.Empty).Trim();
                    artcle.Revision = (request.Revision ?? string.Empty).Trim();
                    artcle.Comments = (request.Comments ?? string.Empty).Trim();
                    artcle.ReviewedOn = request.ReviewedOn;
                    artcle.IsDeleted = request.IsDeleted;
                    artcle.LastModifiedOn = DateTime.Now;
                    artcle.LastModifiedBy = $"{request.ModifiedBy}";

                    //..check entity state
                    _ = await uow.ArticleRevisionRepository.UpdateAsync(artcle, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(artcle).State;
                    Logger.LogActivity($"Article revision state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update article revision record: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Delete(IdRequest idRequest) {
            using var uow = UowFactory.Create();
            try {
                var auditJson = JsonSerializer.Serialize(idRequest, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statutory article data: {auditJson}", "DEBUG");

                var statute = uow.ArticleRevisionRepository.Get(t => t.Id == idRequest.RecordId);
                if (statute != null) {
                    //..mark as delete this article revision
                    _ = uow.ArticleRevisionRepository.Delete(statute, idRequest.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(statute).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete article revision : {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> DeleteAsync(IdRequest idRequest) {
            using var uow = UowFactory.Create();
            try {
                var auditJson = JsonSerializer.Serialize(idRequest, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Statutory article data: {auditJson}", "DEBUG");

                var statute = await uow.ArticleRevisionRepository.GetAsync(t => t.Id == idRequest.RecordId);
                if (statute != null) {
                    //..mark as delete this article revision
                    _ = await uow.ArticleRevisionRepository.DeleteAsync(statute, idRequest.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(statute).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete article revision : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> DeleteAllAsync(IList<long> requestItems, bool markAsDeleted = false) {
            using var uow = UowFactory.Create();
            try {
                var statutes = await uow.ArticleRevisionRepository.GetAllAsync(e => requestItems.Contains(e.Id));
                if (statutes.Count == 0) {
                    Logger.LogActivity($"Records not found", "INFO");
                    return false;
                }
                return await uow.ArticleRevisionRepository.DeleteAllAsync(statutes, markAsDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete article revision: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyInsertAsync(ArticleRevisionRequest[] requestItems) {
            using var uow = UowFactory.Create();
            try {
                //..map request items
                var statutes = requestItems.Select(Mapper.Map<ArticleRevisionRequest, ArticleRevision>).ToArray();
                var statuteJson = JsonSerializer.Serialize(statutes, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Article revision data: {statuteJson}", "DEBUG");
                return await uow.ArticleRevisionRepository.BulkyInsertAsync(statutes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save article revision: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(ArticleRevisionRequest[] requestItems) {
            using var uow = UowFactory.Create();
            try {
                //..map request
                var statutes = requestItems.Select(Mapper.Map<ArticleRevisionRequest, ArticleRevision>).ToArray();
                var statuteJson = JsonSerializer.Serialize(statutes, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Article revision data: {statuteJson}", "DEBUG");
                return await uow.ArticleRevisionRepository.BulkyUpdateAsync(statutes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save statutory article : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(ArticleRevisionRequest[] requestItems, params Expression<Func<ArticleRevision, object>>[] propertySelectors) {
            using var uow = UowFactory.Create();
            try {
                //..map request
                var statutes = requestItems.Select(Mapper.Map<ArticleRevisionRequest, ArticleRevision>).ToArray();
                var statuteJson = JsonSerializer.Serialize(statutes, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Article revisions data: {statuteJson}", "DEBUG");
                return await uow.ArticleRevisionRepository.BulkyUpdateAsync(statutes, propertySelectors);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save article revisions  : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ArticleRevision>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ArticleRevision, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged article revisions", "INFO");

            try {
                return await uow.ArticleRevisionRepository.PageAllAsync(page, size, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revisions: {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ArticleRevision>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ArticleRevision, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged article revisions", "INFO");

            try {
                return await uow.ArticleRevisionRepository.PageAllAsync(token, page, size, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieves article revisions : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ArticleRevision>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ArticleRevision, bool>> predicate = null) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged article revisions", "INFO");

            try {
                return await uow.ArticleRevisionRepository.PageAllAsync(page, size, includeDeleted, predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revisions: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ArticleRevision>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ArticleRevision, bool>> predicate = null, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged article revisions", "INFO");

            try {
                return await uow.ArticleRevisionRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve article revisions : {ex.Message}", "ERROR");
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
