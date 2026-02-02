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

    public class ControlItemService : BaseService, IControlItemService {

        public ControlItemService(IServiceLoggerFactory loggerFactory, IUnitOfWorkFactory uowFactory, IMapper mapper)
            : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Control Items in the database", "INFO");

            try {
                return uow.ControlItemRepository.Count();
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to Control Items in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public int Count(Expression<Func<ControlItem, bool>> predicate) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Control Items in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.ControlItemRepository.Count(predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count Control Items in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<ControlItem, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Control Items in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.ControlItemRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count Control Items in the database: {ex.Message}", "ERROR");
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
                Logger.LogActivity($"Control Item data: {categoryJson}", "DEBUG");

                var categories = uow.ControlItemRepository.Get(t => t.Id == request.RecordId);
                if (categories != null) {
                    //..mark as delete this Control Item
                    _ = uow.ControlItemRepository.Delete(categories, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(categories).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Control Item : {ex.Message}", "ERROR");
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
                Logger.LogActivity($"Control Item data: {categoryJson}", "DEBUG");

                var categories = await uow.ControlItemRepository.GetAsync(t => t.Id == request.RecordId);
                if (categories != null) {
                    //..mark as delete this Control Item
                    _ = await uow.ControlItemRepository.DeleteAsync(categories, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(categories).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Control Item : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Exists(Expression<Func<ControlItem, bool>> predicate, bool excludeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Control Items exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.ControlItemRepository.Exists(predicate, excludeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for Control Items in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<ControlItem, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Control Items exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.ControlItemRepository.ExistsAsync(predicate, excludeDeleted, token);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for Control Items in the database: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ControlItem>> GetAllAsync(Expression<Func<ControlItem, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Control Items", "INFO");

            try {
                return await uow.ControlItemRepository.GetAllAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Control Items : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ControlItem>> GetAllAsync(Expression<Func<ControlItem, bool>> predicate, bool includeDeleted = false, params Expression<Func<ControlItem, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Control Items", "INFO");

            try {
                return await uow.ControlItemRepository.GetAllAsync(includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Control Items : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ComplianceMapResponse>> GetComplianceControlsAsync<ComplianceMapResponse>(int page, int size, bool includeDeleted, Expression<Func<ControlCategory, ComplianceMapResponse>> selector) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Compliance maps", "INFO");

            try {
                return await uow.ControlCategoryRepository.PageLookupAsync(page, size, includeDeleted, selector);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Compliance maps : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<ControlItem> GetAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Control Item with '{id}'", "INFO");

            try {
                return await uow.ControlItemRepository.GetAsync(id, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Control Item : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<ControlItem> GetAsync(Expression<Func<ControlItem, bool>> predicate, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Control Item that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ControlItemRepository.GetAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Control Item : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<ControlItem> GetAsync(Expression<Func<ControlItem, bool>> predicate, bool includeDeleted = false, params Expression<Func<ControlItem, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Control Item that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ControlItemRepository.GetAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Control Item : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Insert(ControlItemRequest request) {
            using var uow = UowFactory.Create();
            try {
                //..map Control Item request to Control Item entity
                var category = Mapper.Map<ControlItemRequest, ControlItem>(request);

                //..log the Control Item data being saved
                var categoryJson = JsonSerializer.Serialize(category, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Control Item data: {categoryJson}", "DEBUG");

                var added = uow.ControlItemRepository.Insert(category);
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
                Logger.LogActivity($"Failed to save Control Category : {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> InsertAsync(ControlItemRequest request) {
            using var uow = UowFactory.Create();
            try {
                //..map Control Item request to Control Item entity
                var category = new ControlItem() { 
                    ControlCategoryId = request.CategoryId,
                    ItemName = (request.ItemName ?? string.Empty).Trim(),
                    Exclude = request.Exclude,
                    IsDeleted = request.IsDeleted,
                    CreatedBy = $"{request.UserName}",
                    CreatedOn = DateTime.Now,
                    Notes = request.Comments ?? string.Empty,
                    LastModifiedBy = $"{request.UserName}",
                    LastModifiedOn = DateTime.Now
                };

                //..log the Control Item data being saved
                var categoryJson = JsonSerializer.Serialize(category, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Control Item data: {categoryJson}", "DEBUG");

                var added = await uow.ControlItemRepository.InsertAsync(category);
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
                Logger.LogActivity($"Failed to save Control Item : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ControlItem>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ControlItem, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Control items", "INFO");

            try {
                return await uow.ControlItemRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve control items : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ControlItem>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ControlItem, bool>> predicate = null) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged control Items", "INFO");

            try {
                return await uow.ControlItemRepository.PageAllAsync(page, size, includeDeleted, predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve control Items : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ControlItemResponse>> PageLookupAsync<ControlItemResponse>(int page, int size, bool includeDeleted, Expression<Func<ControlItem, ControlItemResponse>> selector) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Control Items", "INFO");

            try {
                return await uow.ControlItemRepository.PageLookupAsync(page, size, includeDeleted, selector);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Control Items: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Update(ControlItemRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Control Item request", "INFO");

            try {
                var category = uow.ControlItemRepository.Get(a => a.Id == request.Id);
                if (category != null) {
                    //..update Control Item record
                    category.ItemName = (request.ItemName ?? string.Empty).Trim();
                    category.Notes = request.Comments ?? string.Empty;
                    category.IsDeleted = request.IsDeleted;
                    category.Exclude = request.Exclude;
                    category.LastModifiedOn = DateTime.Now;
                    category.LastModifiedBy = $"{request.UserName}";

                    //..check entity state
                    _ = uow.ControlItemRepository.Update(category, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(category).State;
                    Logger.LogActivity($"Control Item state after Update: {entityState}", "DEBUG");

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

        public async Task<bool> UpdateAsync(ControlItemRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Control Item request", "INFO");

            try {
                var items = await uow.ControlItemRepository.GetAsync(a => a.Id == request.Id, includeDeleted);
                if (items != null) {
                    //..update Control Item record
                    items.ItemName = (request.ItemName ?? string.Empty).Trim();
                    items.Notes = request.Comments ?? string.Empty;
                    items.IsDeleted = request.IsDeleted;
                    items.Exclude = request.Exclude;
                    items.LastModifiedOn = DateTime.Now;
                    items.LastModifiedBy = $"{request.UserName}";

                    //..check entity state
                    _ = await uow.ControlItemRepository.UpdateAsync(items, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(items).State;
                    Logger.LogActivity($"Control Item state after Update: {entityState}", "DEBUG");

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
