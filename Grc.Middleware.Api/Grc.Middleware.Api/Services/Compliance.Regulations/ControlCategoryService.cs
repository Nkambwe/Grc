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

    public class ControlCategoryService : BaseService, IControlCategoryService {

        public ControlCategoryService(IServiceLoggerFactory loggerFactory, IUnitOfWorkFactory uowFactory, IMapper mapper)
            : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Control Categories in the database", "INFO");

            try {
                return uow.ControlCategoryRepository.Count();
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to Control Categories in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public int Count(Expression<Func<ControlCategory, bool>> predicate) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Control Categories in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.ControlCategoryRepository.Count(predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count Control Categories in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<ControlCategory, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Control Categories in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.ControlCategoryRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count Control Categories in the database: {ex.Message}", "ERROR");
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
                Logger.LogActivity($"Control Category data: {categoryJson}", "DEBUG");

                var categories = uow.ControlCategoryRepository.Get(t => t.Id == request.RecordId);
                if (categories != null) {
                    //..mark as delete this Control Category
                    _ = uow.ControlCategoryRepository.Delete(categories, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(categories).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Control Category : {ex.Message}", "ERROR");
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
                Logger.LogActivity($"Control Category data: {categoryJson}", "DEBUG");

                var categories = await uow.ControlCategoryRepository.GetAsync(t => t.Id == request.RecordId, true, t=> t.ControlItems);
                if (categories != null) {
                    if(categories.ControlItems != null && categories.ControlItems.Count > 0) {
                        Logger.LogActivity($"Control Category has {categories.ControlItems.Count} related Control Items. Deleting them first.", "INFO");
                        foreach(var item in categories.ControlItems) {
                            _ = await uow.ControlItemRepository.DeleteAsync(item, request.markAsDeleted);
                            Logger.LogActivity($"Deleted Control Item with ID: {item.Id}", "INFO");
                        }
                    }

                    //..mark as delete this Control Category
                    _ = await uow.ControlCategoryRepository.DeleteAsync(categories, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(categories).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Control Category : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Exists(Expression<Func<ControlCategory, bool>> predicate, bool excludeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Control Categories exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return uow.ControlCategoryRepository.Exists(predicate, excludeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for Control Categories in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<ControlCategory, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Control Categories exists in the database that fit predicate >> '{predicate}'", "INFO");

            try {
                return await uow.ControlCategoryRepository.ExistsAsync(predicate, excludeDeleted, token);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to check for Control Categories in the database: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ControlCategory>> GetAllAsync(Expression<Func<ControlCategory, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Control Categories", "INFO");
            try {
                return await uow.ControlCategoryRepository.GetAllAsync(predicate, includeDeleted); 
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Control Categories : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ControlCategory>> GetAllAsync(Expression<Func<ControlCategory, bool>> predicate, bool includeDeleted = false, params Expression<Func<ControlCategory, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Control Categories", "INFO");
            try {
                return await uow.ControlCategoryRepository.GetAllAsync(includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Control Categories : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<ControlCategoryResponse> GetAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Control Category with '{id}'", "INFO");

            ControlCategoryResponse categoryResponse = null;
            try {
                var category = await uow.ControlCategoryRepository.GetAsync(id, includeDeleted);
                if (category != null) {
                    categoryResponse = new ControlCategoryResponse {
                        Id = category.Id,
                        CategoryName = category.CategoryName ?? string.Empty,
                        Comments = category.Notes ?? string.Empty,
                        Exclude = category.Exclude,
                        IsDeleted = category.IsDeleted
                    };

                    if (category.ControlItems.Count > 0) {
                        var itemIds = category.ControlItems.Select(ci => ci.Id).ToList();
                        Logger.LogActivity($"Item IDs {itemIds.Count} Found", "INFO");

                        var items = await uow.ControlItemRepository.GetAllAsync(ci => itemIds.Contains(ci.Id), includeDeleted);
                        if (items != null && items.Count > 0) {
                            categoryResponse.ControlItems = new List<ControlItemResponse>();
                            foreach (var item in items) {
                                categoryResponse.ControlItems.Add(new ControlItemResponse {
                                    CategoryId = item.ControlCategoryId,
                                    ItemName = item.ItemName ?? string.Empty,
                                    Comments = item.Notes ?? string.Empty,
                                    IsDeleted = item.IsDeleted,
                                    Exclude = item.Exclude
                                });
                            }
                            Logger.LogActivity($"Mapped {categoryResponse.ControlItems.Count} Control Items", "INFO");
                        }
                    }

                }
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Control Category : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }

            return categoryResponse;
        }

        public async Task<ControlCategoryResponse> GetAsync(Expression<Func<ControlCategory, bool>> predicate, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Control Category that fit predicate '{predicate}'", "INFO");

            ControlCategoryResponse categoryResponse = null;
            try {
                var category = await uow.ControlCategoryRepository.GetAsync(predicate, includeDeleted);
                if (category != null) {
                    categoryResponse = new ControlCategoryResponse {
                        Id = category.Id,
                        CategoryName = category.CategoryName ?? string.Empty,
                        Comments = category.Notes ?? string.Empty,
                        Exclude = category.Exclude,
                        IsDeleted = category.IsDeleted
                    };

                    if (category.ControlItems.Count > 0) {
                        var itemIds = category.ControlItems.Select(ci => ci.Id).ToList();
                        Logger.LogActivity($"Item IDs {itemIds.Count} Found", "INFO");

                        var items = await uow.ControlItemRepository.GetAllAsync(ci => itemIds.Contains(ci.Id), includeDeleted);
                        if (items != null && items.Count > 0) {
                            categoryResponse.ControlItems = new List<ControlItemResponse>();
                            foreach (var item in items) {
                                categoryResponse.ControlItems.Add(new ControlItemResponse {
                                    CategoryId = item.ControlCategoryId,
                                    ItemName = item.ItemName ?? string.Empty,
                                    Comments = item.Notes ?? string.Empty,
                                    IsDeleted = item.IsDeleted,
                                    Exclude = item.Exclude
                                });
                            }
                            Logger.LogActivity($"Mapped {categoryResponse.ControlItems.Count} Control Items", "INFO");
                        }
                    }

                }
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Control Category : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }

            return categoryResponse;
        }

        public async Task<ControlCategoryResponse> GetAsync(Expression<Func<ControlCategory, bool>> predicate, bool includeDeleted = false, params Expression<Func<ControlCategory, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Control Category that fit predicate '{predicate}'", "INFO");

            ControlCategoryResponse categoryResponse = null;
            try {
                var category = await uow.ControlCategoryRepository.GetAsync(predicate, includeDeleted, includes);
                if (category != null) {
                    categoryResponse = new ControlCategoryResponse {
                        Id = category.Id,
                        CategoryName = category.CategoryName ?? string.Empty,
                        Comments = category.Notes ?? string.Empty,
                        Exclude = category.Exclude,
                        IsDeleted = category.IsDeleted
                    };

                    if (category.ControlItems.Count > 0) {
                        var itemIds = category.ControlItems.Select(ci => ci.Id).ToList();
                        Logger.LogActivity($"Item IDs {itemIds.Count} Found", "INFO");

                        var items = await uow.ControlItemRepository.GetAllAsync(ci => itemIds.Contains(ci.Id), includeDeleted);
                        if (items != null && items.Count > 0) {
                            categoryResponse.ControlItems = new List<ControlItemResponse>();
                            foreach (var item in items) {
                                categoryResponse.ControlItems.Add(new ControlItemResponse {
                                    CategoryId = item.ControlCategoryId,
                                    ItemName = item.ItemName ?? string.Empty,
                                    Comments = item.Notes ?? string.Empty,
                                    IsDeleted = item.IsDeleted,
                                    Exclude = item.Exclude
                                });
                            }
                            Logger.LogActivity($"Mapped {categoryResponse.ControlItems.Count} Control Items", "INFO");
                        }
                    }
                    
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Control Category : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }

            return categoryResponse;
        }

        public bool Insert(ControlCategoryRequest request) {
            using var uow = UowFactory.Create();

            try {
                //..create category entity
                var category = new ControlCategory {
                    CategoryName = request.CategoryName?.Trim() ?? string.Empty,
                    Notes = request.Comments ?? string.Empty,
                    IsDeleted = request.IsDeleted,
                    Exclude = request.Exclude,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = request.UserName,
                    LastModifiedOn = DateTime.UtcNow,
                    LastModifiedBy = request.UserName,
                    ControlItems = new List<ControlItem>()
                };

                //..add child items
                if (request.ControlItems != null && request.ControlItems.Any()) {
                    foreach (var item in request.ControlItems) {
                        category.ControlItems.Add(new ControlItem {
                            ItemName = item.ItemName ?? string.Empty,
                            Exclude = item.Exclude,
                            IsDeleted = item.IsDeleted,
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = request.UserName,
                            LastModifiedOn = DateTime.UtcNow,
                            LastModifiedBy = request.UserName,
                            Notes = item.Comments ?? string.Empty
                        });
                    }
                }

                //..log payload
                var categoryJson = JsonSerializer.Serialize(category, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });

                Logger.LogActivity($"Control Category data: {categoryJson}", "DEBUG");

                //..insert root entity only
                var added = uow.ControlCategoryRepository.Insert(category);
                if (!added)
                    return false;

                //..debug state
                var entityState = ((UnitOfWork)uow).Context.Entry(category).State;
                Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                //..save once category with it's items
                var result = uow.SaveChanges();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");

                return result > 0;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save Control Category: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> InsertAsync(ControlCategoryRequest request) {
            using var uow = UowFactory.Create();

            try {
                //..create category entity
                var category = new ControlCategory {
                    CategoryName = request.CategoryName?.Trim() ?? string.Empty,
                    Notes = request.Comments ?? string.Empty,
                    IsDeleted = request.IsDeleted,
                    Exclude = request.Exclude,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = request.UserName,
                    LastModifiedOn = DateTime.UtcNow,
                    LastModifiedBy = request.UserName,
                    ControlItems = new List<ControlItem>()
                };

                //..add child items
                if (request.ControlItems != null && request.ControlItems.Any()) {
                    foreach (var item in request.ControlItems) {
                        category.ControlItems.Add(new ControlItem {
                            ItemName = item.ItemName ?? string.Empty,
                            Exclude = item.Exclude,
                            IsDeleted = item.IsDeleted,
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = request.UserName,
                            LastModifiedOn = DateTime.UtcNow,
                            LastModifiedBy = request.UserName,
                            Notes = item.Comments ?? string.Empty
                        });
                    }
                }

                //..log payload
                var categoryJson = JsonSerializer.Serialize(category, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });

                Logger.LogActivity($"Control Category data: {categoryJson}", "DEBUG");

                //..insert root entity only
                var added = await uow.ControlCategoryRepository.InsertAsync(category);
                if (!added)
                    return false;

                //..debug state
                var entityState = ((UnitOfWork)uow).Context.Entry(category).State;
                Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                //..save once category with it's items
                var result = await uow.SaveChangesAsync();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");

                return result > 0;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save Control Category: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ControlCategory>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ControlCategory, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Control Categories", "INFO");

            try {
                return await uow.ControlCategoryRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve control Categories : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ControlCategory>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ControlCategory, bool>> predicate = null) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged control Categories", "INFO");

            try {
                return await uow.ControlCategoryRepository.PageAllAsync(page, size, includeDeleted, predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve control Categories : {ex.Message}", "ERROR");

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }

        }

        public async Task<PagedResult<ControlCategoryResponse>> PageLookupAsync<ControlCategoryResponse>(int page, int size, bool includeDeleted, Expression<Func<ControlCategory, ControlCategoryResponse>> selector) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Control Categories", "INFO");

            try {
                return await uow.ControlCategoryRepository.PageLookupAsync(page, size, includeDeleted, selector);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Control Categories: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Update(ControlCategoryRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Update Control Category request", "INFO");

            try {
                var category = uow.ControlCategoryRepository.Get(c => c.Id == request.Id, includeDeleted, c => c.ControlItems);
                if (category == null)
                    return false;

                //..remove deleted items
                var requestItemIds = request.ControlItems.Where(x => !(x.Id == 0 && !x.IsDeleted)).Select(x => x.Id).ToHashSet();
                var itemsToRemove = category.ControlItems.Where(ci => !requestItemIds.Contains(ci.Id)).ToList();

                foreach (var item in itemsToRemove) {
                    category.ControlItems.Remove(item);
                }

                //..handle add/update of control items
                foreach (var reqItem in request.ControlItems) {
                    if (reqItem.Id == 0) {
                        //..add new control items
                        category.ControlItems.Add(new ControlItem {
                            ItemName = reqItem.ItemName ?? string.Empty,
                            Exclude = reqItem.Exclude,
                            IsDeleted = reqItem.IsDeleted,
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = request.UserName,
                            LastModifiedOn = DateTime.UtcNow,
                            LastModifiedBy = request.UserName,
                            Notes = reqItem.Comments ?? string.Empty
                        });
                    } else {
                        //..update existing control items
                        var existingItem = category.ControlItems
                            .FirstOrDefault(ci => ci.Id == reqItem.Id);

                        if (existingItem != null) {
                            existingItem.ItemName = reqItem.ItemName ?? string.Empty;
                            existingItem.Exclude = reqItem.Exclude;
                            existingItem.IsDeleted = reqItem.IsDeleted;
                            existingItem.LastModifiedOn = DateTime.UtcNow;
                            existingItem.LastModifiedBy = request.UserName;
                            existingItem.Notes = reqItem.Comments ?? string.Empty;
                        }
                    }
                }

                //..update parent
                category.CategoryName = request.CategoryName?.Trim() ?? string.Empty;
                category.Notes = request.Comments ?? string.Empty;
                category.IsDeleted = request.IsDeleted;
                category.Exclude = request.Exclude;
                category.LastModifiedOn = DateTime.UtcNow;
                category.LastModifiedBy = request.UserName;

                //..save changes
                var result = uow.SaveChanges();
                return result > 0;
            } catch (Exception ex) {
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ControlCategoryRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Update Control Category request", "INFO");

            try {
                var category = await uow.ControlCategoryRepository.GetAsync(c => c.Id == request.Id, includeDeleted, c => c.ControlItems);
                if (category == null)
                    return false;

                //..remove deleted items
                var requestItemIds = request.ControlItems.Where(x => !(x.Id == 0 && !x.IsDeleted)).Select(x => x.Id).ToHashSet();
                var itemsToRemove = category.ControlItems.Where(ci => !requestItemIds.Contains(ci.Id)).ToList();

                foreach (var item in itemsToRemove) {
                    category.ControlItems.Remove(item);
                }

                //..handle add/update of control items
                foreach (var reqItem in request.ControlItems) {
                    if (reqItem.Id == 0) {
                        //..add new control items
                        category.ControlItems.Add(new ControlItem {
                            ItemName = reqItem.ItemName ?? string.Empty,
                            Exclude = reqItem.Exclude,
                            IsDeleted = reqItem.IsDeleted,
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = request.UserName,
                            LastModifiedOn = DateTime.UtcNow,
                            LastModifiedBy = request.UserName,
                            Notes = reqItem.Comments ?? string.Empty
                        });
                    } else {
                        //..update existing control items
                        var existingItem = category.ControlItems
                            .FirstOrDefault(ci => ci.Id == reqItem.Id);

                        if (existingItem != null) {
                            existingItem.ItemName = reqItem.ItemName ?? string.Empty;
                            existingItem.Exclude = reqItem.Exclude;
                            existingItem.IsDeleted = reqItem.IsDeleted;
                            existingItem.LastModifiedOn = DateTime.UtcNow;
                            existingItem.LastModifiedBy = request.UserName;
                            existingItem.Notes = reqItem.Comments ?? string.Empty;
                        }
                    }
                }

                //..update parent
                category.CategoryName = request.CategoryName?.Trim() ?? string.Empty;
                category.Notes = request.Comments ?? string.Empty;
                category.IsDeleted = request.IsDeleted;
                category.Exclude = request.Exclude;
                category.LastModifiedOn = DateTime.UtcNow;
                category.LastModifiedBy = request.UserName;

                //..save changes
                var result = await uow.SaveChangesAsync();
                return result > 0;
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
