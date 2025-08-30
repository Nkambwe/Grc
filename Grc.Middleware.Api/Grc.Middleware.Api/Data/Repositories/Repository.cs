using Grc.Middleware.Api.Data.Entities;
using Grc.Middleware.Api.Utils;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.Data.SqlClient;
using Grc.Middleware.Api.Helpers;

namespace Grc.Middleware.Api.Data.Repositories {
    public class Repository<T> : IRepository<T> where T : BaseEntity {
        protected readonly IServiceLogger Logger;
        protected readonly GrcContext context; 

        public Repository(IServiceLoggerFactory loggerFactory, GrcContext _context)
        {
            Logger = loggerFactory.CreateLogger();
            Logger.Channel = $"REPO-{DateTime.Now:yyyyMMddHHmmss}";
            context = _context;
        }

        public T Get(long id, bool includeDeleted = false) {
            T entity = null;
            var entities = context.Set<T>();

            try {

                var query = entities.AsQueryable();
                if (!includeDeleted) {
                    query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
                }

                entity = query.FirstOrDefault(e => e.Id == id);
            } catch (Exception ex) {
                Logger.LogActivity($"Get operation failed: {ex.Message}", "DBOPS");
                Logger.LogActivity("STACKTRACE ::", "DBOPS");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
            }

            return entity;
        }

        public async Task<T> GetAsync(long id, bool includeDeleted = false) {
            T entity = null;
            var entities = context.Set<T>();
            try {
                var query = entities.AsQueryable();
                if (!includeDeleted) {
                    query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
                }

                entity = await query.FirstOrDefaultAsync(e => e.Id == id);

            } catch (Exception ex) {
                Logger.LogActivity($"Get operation failed: {ex.Message}", "DBOPS");
                Logger.LogActivity("STACKTRACE ::", "DBOPS");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
            }

            return entity;
        }

        public T Get(Expression<Func<T, bool>> where, bool includeDeleted = false) {
            T entity = null;
            var entities = context.Set<T>();
            try {
                var query = entities.AsQueryable();

                // Apply soft-delete filter if includeDeleted is false
                if (!includeDeleted) {
                    query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
                }

                entity = query.FirstOrDefault(where);

            } catch (Exception ex) {
                Logger.LogActivity($"Get operation failed: {ex.Message}", "DBOPS");
                Logger.LogActivity("STACKTRACE ::", "DBOPS");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
            }

            return entity;
        }

        public T Get(Expression<Func<T, bool>> where, bool includeDeleted = false, params Expression<Func<T, object>>[] filters) {
            T entity = null;
            var entities = context.Set<T>();
            try {
                var query = entities.AsQueryable();

                //include related entities
                if (filters != null) {
                    query = filters.Aggregate(query,
                            (current, next) => current.Include(next));
                }

                // Apply soft-delete filter if includeDeleted is false
                if (!includeDeleted) {
                    query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
                }

                entity = query.FirstOrDefault(where);

            } catch (Exception ex) {
                Logger.LogActivity($"Get operation failed: {ex.Message}", "DBOPS");
                Logger.LogActivity("STACKTRACE ::", "DBOPS");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
            }

            return entity;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> where, bool includeDeleted = false) {

            var entities = context.Set<T>();
            try {
                var query = entities.AsQueryable();
                if (!includeDeleted) {
                    query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
                }

                return await query.FirstOrDefaultAsync(where);
            } catch (Exception ex) {
                Logger.LogActivity($"Get operation failed: {ex.Message}", "DBOPS");
                Logger.LogActivity("STACKTRACE ::", "DBOPS");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                return null;
            }
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false, params Expression<Func<T, object>>[] filters) {

            IQueryable<T> query = context.Set<T>();

            //..apply the deleted filter if needed
            if (!includeDeleted) {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }

            //..apply predicate
            query = query.Where(predicate);
            foreach (var filter in filters) {
                //..we need to use Include() which creates LEFT JOIN in EF Core
                query = query.Include(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public IQueryable<T> GetAll(bool includeDeleted = false, params Expression<Func<T, object>>[] filters) {
            IQueryable<T> query = context.Set<T>();

            if (filters != null && filters.Any()) {
                foreach (var include in filters) {
                    query = query.Include(include);
                }
            }

            if (!includeDeleted) {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }

            return query;
        }

        public IList<T> GetAll(bool includeDeleted = false) {
            IQueryable<T> query = context.Set<T>();

            if (!includeDeleted) {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }

            return query.ToList();
        }

        public async Task<IList<T>> GetAllAsync(bool includeDeleted = false) {
            IQueryable<T> query = context.Set<T>();
            return await query.Where(e => includeDeleted || EF.Property<bool>(e, "IsDeleted") == false).ToListAsync();
        }

        public IList<T> GetAll(Expression<Func<T, bool>> where, bool includeDeleted = false) {
            IQueryable<T> query = context.Set<T>();

            if (!includeDeleted) {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }

            return query.Where(where).ToList();
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> where, bool includeDeleted = false) {
            IQueryable<T> query = context.Set<T>();
            return await query.Where(e => includeDeleted || EF.Property<bool>(e, "IsDeleted") == false)
                .Where(where).ToListAsync();
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> where, bool includeDeleted = false, params Expression<Func<T, object>>[] filters) {
            IQueryable<T> query = context.Set<T>();

            //..includes if provided
            if (filters != null && filters.Any()) {
                foreach (var include in filters) {
                    query = query.Include(include);
                }
            }

            //..apply soft delete filter and where clause
            query = query.Where(e => includeDeleted || EF.Property<bool>(e, "IsDeleted") == false).Where(where);
            return await query.ToListAsync();
        }

        public async Task<IList<T>> GetAllAsync(bool includeDeleted = false, params Expression<Func<T, object>>[] filters) {
            IQueryable<T> query = context.Set<T>();

            if (filters != null && filters.Any()) {
                foreach (var include in filters) {
                    query = query.Include(include);
                }
            }
            
            if (!includeDeleted) {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }

            return await query.ToListAsync();
        }

        public async Task<IList<T>> GetTopAsync(Expression<Func<T, bool>> where, int top = 10, bool includeDeleted = false) {
            IQueryable<T> query = context.Set<T>();

            if (!includeDeleted) {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }

            return await query.Where(where).Take(top).ToListAsync();
        }

        public bool Insert(T entity) {
            ArgumentNullException.ThrowIfNull(entity);
            var dbSet = context.Set<T>();

            try {
                dbSet.Add(entity);
                return true;
            } catch (Exception ex) {
                Logger.LogActivity($"Insert operation failed: {ex.Message}", "DBOPS");
                Logger.LogActivity("STACKTRACE ::", "DBOPS");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                return false;
            }
        }

        public async Task<bool> InsertAsync(T entity) {
            ArgumentNullException.ThrowIfNull(entity);
            var dbSet = context.Set<T>();
            try {
                await dbSet.AddAsync(entity);
                return true;
            } catch (Exception ex) {
                Logger.LogActivity($"Insert operation failed: {ex.Message}", "DBOPS");
                Logger.LogActivity("STACKTRACE ::", "DBOPS");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                return false;
            }
        }

        public bool Update(T entity, bool includeDeleted = false) {
            ArgumentNullException.ThrowIfNull(entity);
            try {
                if (!includeDeleted) {
                    var entry = context.Entry(entity);
                    var isDeleted = (bool)entry.Property("IsDeleted").CurrentValue;

                    if (isDeleted) {
                        Logger.LogActivity("Update operation skipped: Entity is marked as deleted.", "DBOPS");
                        return true;
                    }
                }

                context.Update(entity);
                return true;
            } catch (Exception ex) {
                Logger.LogActivity($"Update operation failed: {ex.Message}", "DBOPS");
                Logger.LogActivity("STACKTRACE ::", "DBOPS");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(T entity, bool includeDeleted = false) {
            ArgumentNullException.ThrowIfNull(entity);
            try {
                if (!includeDeleted) {
                    var entry = context.Entry(entity);
                    var isDeleted = (bool)entry.Property("IsDeleted").CurrentValue;

                    if (isDeleted) {
                        Logger.LogActivity("Update operation skipped: Entity is marked as deleted.", "DBOPS");
                        return true;
                    }
                }

                context.Update(entity);
                return await Task.FromResult(true);
            } catch (Exception ex) {
                Logger.LogActivity($"Update operation failed: {ex.Message}", "DBOPS");
                Logger.LogActivity("STACKTRACE ::", "DBOPS");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                return false;
            }
        }
        
        public async Task<int> CountAsync() 
            => await context.Set<T>().CountAsync();

        public async Task<int> CountAsync(Expression<Func<T, bool>> where)
            => await context.Set<T>().CountAsync(where);

        public bool Delete(T entity, bool markAsDeleted = false) {
            ArgumentNullException.ThrowIfNull(entity);
            var dbSet = context.Set<T>();
            try {
                if (markAsDeleted) {
                    var entry = context.Entry(entity);
                    entry.Property("IsDeleted").CurrentValue = true;
                    entry.State = EntityState.Modified;
                } else {
                    dbSet.Remove(entity);
                }

                return true;
            } catch (Exception ex) {
                Logger.LogActivity($"Delete operation failed: {ex.Message}", "DBOPS");
                Logger.LogActivity("STACKTRACE ::", "DBOPS");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(T entity, bool markAsDeleted = false) {
            ArgumentNullException.ThrowIfNull(entity);
            var dbSet = context.Set<T>();
            try {
                if (markAsDeleted) {
                    var entry = context.Entry(entity);
                    entry.Property("IsDeleted").CurrentValue = true;
                    entry.State = EntityState.Modified;
                } else {
                    dbSet.Remove(entity);
                }

                return await Task.FromResult(true);
            } catch (Exception ex) {
                Logger.LogActivity($"Delete operation failed: {ex.Message}", "DBOPS");
                Logger.LogActivity("STACKTRACE ::", "DBOPS");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                return false;
            }
        }

        public async Task<bool> DeleteAllAsync(IList<T> entities, bool markAsDeleted = false) {
            ArgumentNullException.ThrowIfNull(entities);
            if (entities.Count == 0) return true;

            var dbSet = context.Set<T>();
            try {
                if (markAsDeleted) {
                    foreach (var entity in entities) {
                        var entry = context.Entry(entity);
                        entry.Property("IsDeleted").CurrentValue = true;
                        entry.State = EntityState.Modified;
                    }
                } else {
                    dbSet.RemoveRange(entities);
                }

                return await Task.FromResult(true);
            } catch (Exception ex) {
                Logger.LogActivity($"DeleteAll operation failed: {ex.Message}", "DBOPS");
                Logger.LogActivity("STACKTRACE ::", "DBOPS");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                return false;
            }
        }

        public bool Exists(Expression<Func<T, bool>> where, bool excludeDeleted = false) {
            var dbSet = context.Set<T>();
            var record = dbSet.FirstOrDefault(where);

            if (excludeDeleted) {
                return record != null && !record.IsDeleted;
            }

            return record == null;
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> where, bool excludeDeleted = false) {
            var dbSet = context.Set<T>();
            var record = await dbSet.FirstOrDefaultAsync(where);

            if (excludeDeleted) {
                return record != null && !record.IsDeleted;
            }

            return record != null;
        }

        public async Task<bool> BulkyInsertAsync(T[] entities) {
            if (entities is null || entities.Length == 0) {
                Logger.LogActivity("Bulk insert operation canceled: Empty list.", "DBOPS");
                return false;
            }

            var validEntities = entities.Where(e => e is not null).ToList();
            if (validEntities.Count == 0) {
                Logger.LogActivity("Bulk insert operation canceled: Only null values found.", "DBOPS");
                return false;
            }

            Logger.LogActivity($"Bulk insert operation started: {validEntities.Count} entities.", "DBOPS");

            var bulkConfig = new BulkConfig {
                SetOutputIdentity = false,
                PreserveInsertOrder = false,
                SqlBulkCopyOptions = SqlBulkCopyOptions.KeepIdentity
            };

            try {
                await context.BulkInsertAsync(validEntities, bulkConfig);
                Logger.LogActivity("Bulk insert operation completed successfully.", "DBOPS");
                return true;
            } catch (Exception ex) {
                Logger.LogActivity($"Bulk insert operation failed: {ex.Message}", "DBOPS");
                Logger.LogActivity("STACKTRACE ::", "DBOPS");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                return false;
            }
        }

        public async Task<bool> BulkyUpdateAsync(T[] entities) {
            if (entities is null || entities.Length == 0) {
                Logger.LogActivity("Bulk update operation canceled: Empty list.", "DBOPS");
                return false;
            }

            var validEntities = entities.Where(e => e is not null).ToList();
            if (validEntities.Count == 0) {
                Logger.LogActivity("Bulk update operation canceled: Only null values found.", "DBOPS");
                return false;
            }

            var bulkConfig = new BulkConfig {
                SetOutputIdentity = true,
                PreserveInsertOrder = true,
                UpdateByProperties = null
            };

            try {
                await context.BulkUpdateAsync(validEntities, bulkConfig);
                return true;
            } catch (Exception ex) {
                Logger.LogActivity($"Bulk update operation. Error! {ex.Message}", "DBOPS");
                Logger.LogActivity("STACKTRACE ::", "DBOPS");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                return false;
            }
        }

        public async Task<bool> BulkyUpdateAsync(T[] entities, params Expression<Func<T, object>>[] propertySelectors) {
            if (entities is null || entities.Length == 0) {
                Logger.LogActivity("Bulk update operation canceled: Empty list.", "DBOPS");
                return false;
            }

            var validEntities = entities.Where(e => e is not null).ToList();
            if (validEntities.Count == 0) {
                Logger.LogActivity("Bulk update operation canceled: Only null values found.", "DBOPS");
                return false;
            }

            //..convert expressions to property names
            var propertyNames = propertySelectors
                .Select(GetPropertyName)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();

            var bulkConfig = new BulkConfig {
                SetOutputIdentity = true,
                PreserveInsertOrder = true,
                UpdateByProperties = propertyNames
            };

            try {
                await context.BulkUpdateAsync(validEntities, bulkConfig);
                return true;
            } catch (Exception ex) {
                Logger.LogActivity($"Bulk update operation. Error! {ex.Message}", "DBOPS");
                Logger.LogActivity("STACKTRACE ::", "DBOPS");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
                return false;
            }
        }
        
        public virtual async Task<PagedResult<T>> PageAllAsync(int page, int size, bool includeDeleted = false, params Expression<Func<T, object>>[] includes) {
            page = Math.Max(1, page);
            size = Math.Max(1, size);

            IQueryable<T> query = context.Set<T>();

            //..apply includes
            foreach (var include in includes)
                query = query.Include(include);

            if (!includeDeleted)
                query = query.Where(m => !m.IsDeleted);

            var totalRecords = await query.CountAsync();
            var entities = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return new PagedResult<T> {
                Entities = entities,
                Count = totalRecords,
                Page = page,
                Size = size
            };
        }

        public virtual async Task<PagedResult<T>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<T, object>>[] includes) {
            //make sure page size is never negative
            page = Math.Max(1, page);   
            size = Math.Max(1, size);  
             IQueryable<T> query = context.Set<T>();

            //..apply includes
            foreach (var include in includes)
                query = query.Include(include);

            if (!includeDeleted)
                query = query.Where(m => !m.IsDeleted);

            var totalRecords = await query.CountAsync(token);
            var entities = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(token);

            return new PagedResult<T> {
                Entities = entities,
                Count = totalRecords,
                Page = page,
                Size = size
            };

        }

        public virtual async Task<PagedResult<T>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<T, bool>> where = null) {
            //..make sure page size is never negative
            page = Math.Max(1, page);   
            size = Math.Max(1, size);  
            var dbSet = context.Set<T>();
            var query = where != null ? dbSet.Where(where) : dbSet;
    
            //..handle soft deleted entities
            if (!includeDeleted && typeof(T).GetProperty("IsDeleted") != null) {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, "IsDeleted");
                var comparison = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);

                query = query.Where(lambda);
            }
    
            var totalCount = await query.CountAsync();
            var entities = await query.Skip((page - 1) * size).Take(size).ToListAsync();
   
            return new PagedResult<T> {
                Entities = entities,
                Count = totalCount,
                Page = page,
                Size = size
            };
        }

        public virtual async Task<PagedResult<T>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<T, bool>> where = null, bool includeDeleted = false) {
            //..make sure page size is never negative
            page = Math.Max(1, page);   
            size = Math.Max(1, size);  
            var dbSet = context.Set<T>();
    
            var query = where != null ? dbSet.Where(where) : dbSet;
    
            //..handle soft deleted entities
            if (!includeDeleted && typeof(T).GetProperty("IsDeleted") != null) {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, "IsDeleted");
                var comparison = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
        
                query = query.Where(lambda);
            }
    
            var totalCount = await query.CountAsync(token);
            var entities = await query.Skip((page - 1) * size).Take(size).ToListAsync(token);
    
            return new PagedResult<T> {
                Entities = entities,
                Count = totalCount,
                Page = page,
                Size = size
            };
        }

        public int GetContextHashCode()
            => context.GetType().GetHashCode();

        #region Helper Methods

        /// <summary>
        /// Extract class property name form class property
        /// </summary>
        /// <param name="where">Filter Predicate</param>
        /// <returns>Property name</returns>
        private static string GetPropertyName(Expression<Func<T, object>> where) {
            if (where.Body is MemberExpression exMember) {
                return exMember.Member.Name;
            } else if (where.Body is UnaryExpression unary) {
                //..for handling nullable properties
                if (unary.Operand is MemberExpression operand) {
                    return operand.Member.Name;
                }
            }

            return null;
        }

        #endregion

    }
}
