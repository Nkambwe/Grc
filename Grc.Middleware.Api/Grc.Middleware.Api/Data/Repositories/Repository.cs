using EFCore.BulkExtensions;
using Grc.Middleware.Api.Data.Entities;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;

namespace Grc.Middleware.Api.Data.Repositories {
    public class Repository<T> : IRepository<T> where T : BaseEntity {
        protected readonly IServiceLogger Logger;
        protected readonly GrcContext context;

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="loggerFactory">Logger Factory</param>
        /// <param name="_context">DB Context objcet</param>
        public Repository(IServiceLoggerFactory loggerFactory, GrcContext _context)
        {
            Logger = loggerFactory.CreateLogger();
            Logger.Channel = $"REPO-{DateTime.Now:yyyyMMddHHmmss}";
            context = _context;
        }

        public int Count() {
            try {
                return context.Set<T>().Count();
            }  catch (Exception ex) {
                Logger.LogActivity($"Count operation failed: {ex.Message}", "DbAction");
                Logger.LogActivity($"STACKTRACE :: {ex.StackTrace}", "DbStacktrace");
                return 0;
            }
        }

        public int Count(Expression<Func<T, bool>> predicate) {
            ArgumentNullException.ThrowIfNull(predicate);

            try {
                return context.Set<T>().Count(predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Count with predicate operation failed: {ex.Message}", "DbAction");
                Logger.LogActivity($"STACKTRACE :: {ex.StackTrace}", "DbStacktrace");
                return 0;
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default) {
            try {
                return await context.Set<T>().CountAsync(cancellationToken: cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"CountAsync operation failed: {ex.Message}", "DbAction");
                Logger.LogActivity($"STACKTRACE :: {ex.StackTrace}", "DbStacktrace");
                return 0;
            }
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) {
            ArgumentNullException.ThrowIfNull(predicate);

            try {
                return await context.Set<T>().CountAsync(predicate, cancellationToken: cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"CountAsync with predicate operation failed: {ex.Message}", "DbAction");
                Logger.LogActivity($"STACKTRACE :: {ex.StackTrace}", "DbStacktrace");
                return 0;
            }
        }

        public async Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            try {

                //..exclude deleted records
                if (excludeDeleted) {
                    return await context.Set<T>().CountAsync(e => !EF.Property<bool>(e, "IsDeleted"), cancellationToken);
                }

                return await context.Set<T>().CountAsync(cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"CountAsync with excludeDeleted operation failed: {ex.Message}", "DbAction");
                Logger.LogActivity($"STACKTRACE :: {ex.StackTrace}", "DbStacktrace");
                return 0;
            }
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            ArgumentNullException.ThrowIfNull(predicate);

            try {
                var query = context.Set<T>().AsQueryable();
                if (excludeDeleted) {
                    query = query.Where(e => !e.IsDeleted);
                }

                return await query.CountAsync(predicate, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"CountAsync with predicate and excludeDeleted operation failed: {ex.Message}", "DbAction");
                Logger.LogActivity($"STACKTRACE :: {ex.StackTrace}", "DbStacktrace");
                return 0;
            }
        }

        public bool Exists(Expression<Func<T, bool>> predicate, bool excludeDeleted = true) {     
            try {
                ArgumentNullException.ThrowIfNull(predicate);
                var dbSet = context.Set<T>();
                var record = dbSet.FirstOrDefault(predicate);

                if (excludeDeleted) {
                    return record != null && !record.IsDeleted;
                }

                return record == null;
            } catch (Exception ex) {
                Logger.LogActivity($"Exists operation failed: {ex.Message}", "DbAction");
                Logger.LogActivity($"STACKTRACE :: {ex.StackTrace}", "DbStacktrace");
                return false;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default) {

            try {
                ArgumentNullException.ThrowIfNull(predicate);
                var dbSet = context.Set<T>();
                var record = await dbSet.FirstOrDefaultAsync(predicate, cancellationToken: cancellationToken);

                if (excludeDeleted) {
                    return record != null && !record.IsDeleted;
                }

                return record != null;
            } catch (Exception ex)  {
                Logger.LogActivity($"ExistsAsync operation failed: {ex.Message}", "DbAction");
                Logger.LogActivity($"STACKTRACE :: {ex.StackTrace}", "DbStacktrace");
                return false;
            }
               
        }

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<T, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            if (predicates == null || predicates.Count == 0)
                return new Dictionary<string, bool>();

            try
            {
                var results = new Dictionary<string, bool>();
                var dbSet = context.Set<T>();
                var query = dbSet.AsQueryable();

                //..execute all queries in parallel for better performance
                var tasks = predicates.Select(async kvp => {
                    var predicate = kvp.Value;
                    var key = kvp.Key;

                    if (excludeDeleted) {
                        query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
                    }

                    var result = await query.AnyAsync(predicate, cancellationToken);
                    return new KeyValuePair<string, bool>(key, result);
                });

                var batchResults = await Task.WhenAll(tasks);
                foreach (var result in batchResults)
                {
                    results[result.Key] = result.Value;
                }

                return results;
            } catch (Exception ex) {
                Logger.LogActivity($"ExistsBatch operation failed: {ex.Message}", "DbAction");
                Logger.LogActivity($"STACKTRACE :: {ex.StackTrace}", "DbStacktrace");
                return new Dictionary<string, bool>();
            }
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

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> where, bool includeDeleted = false, params string[] includes) {
            IQueryable<T> query = context.Set<T>();

            if (!includeDeleted) {
                query = query.Where(e => !EF.Property<bool>(e, "IsDeleted"));
            }

            foreach (var include in includes) {
                query = query.Include(include);
            }

            return await query.Where(where).ToListAsync();
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

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> where, bool includeDeleted = false, Func<IQueryable<T>, IQueryable<T>> includeFunc = null) {
            IQueryable<T> query = context.Set<T>();

            if (!includeDeleted) {
                query = query.Where(e => !EF.Property<bool>(e, "IsDeleted"));
            }

            if (includeFunc != null) {
                query = includeFunc(query);
            }

            return await query.Where(where).ToListAsync();
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
