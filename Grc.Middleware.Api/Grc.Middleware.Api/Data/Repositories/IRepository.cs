using Grc.Middleware.Api.Data.Entities;
using Grc.Middleware.Api.Helpers;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Data.Repositories {

    public interface IRepository<T> where T : BaseEntity {
        /// <summary>
        /// Count number of entities in the database
        /// </summary>
        /// <returns>Number of entities found in the database</returns>
        int Count();

        /// <summary>
        /// Count number of entities in the database
        /// </summary>
        /// <param name="predicate">Count filter</param>
        /// <returns>Number of entities found in the database</returns>
        int Count(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Count number of entities in the database
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Task containg number of entities found in the database</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Asynchronous count number of entities in the database
        /// </summary>
        /// <param name="excludeDeleted">Exclude deleted entities</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Task containg number of entities found in the database</returns>
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronous count number of entities in the database
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Task containg number of entities found in the database</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        /// <summary>
        /// Asynchronous count number of entities in the database
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="excludeDeleted">Exclude deleted entities</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Task containg number of entities found in the database</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if an entity exists if it fits predicate
        /// </summary>
        /// <param name="where">Search predicate</param>
        /// <param name="excludeDeleted">Flag to exclude deleted entities in the search</param>
        /// <returns>Search result for entity</returns>
        bool Exists(Expression<Func<T, bool>> where, bool excludeDeleted = false);

        /// <summary>
        /// Asynchronous check if an entity exists if it fits predicate
        /// </summary>
        /// <param name="where">Search predicate</param>
        /// <param name="excludeDeleted">Flag to exclude deleted entities in the search</param>
        /// <returns>Task containg search result for entity</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> where, bool excludeDeleted = false, CancellationToken token = default);
        /// <summary>
        /// Check for batch existence for multiple conditions
        /// </summary>
        /// <param name="predicates"></param>
        /// <param name="excludeDeleted"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<T, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get entity by Id. Check whether to returned deleted entities <see cref="ISoftDelete"/>
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <param name="includeDeleted">Flag to check whether to returned deleted entities</param>
        /// <returns>Entity with defined Id</returns>
        T Get(long id, bool includeDeleted = false);

        /// <summary>
        /// Get entity by Id. Check whether to returned deleted entities <see cref="ISoftDelete"/>
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <param name="includeDeleted">Flag to check whether to returned deleted entities</param>
        /// <returns>Task containing entity with defined Id</returns>
        Task<T> GetAsync(long id, bool includeDeleted = false);

        /// <summary>
        /// Get entity the fits predicate. Check whether to returned deleted entities <see cref="ISoftDelete"/>
        /// </summary>
        /// <param name="where">Search Predicate</param>
        /// <param name="includeDeleted">Flag to check whether to returned deleted entities</param>
        /// <returns>Entity that fits predicate</returns>
        T Get(Expression<Func<T, bool>> where, bool includeDeleted = false);

        /// <summary>
        /// Get entity the fits predicate. Check whether to returned deleted entities <see cref="ISoftDelete"/>
        /// </summary>
        /// <param name="where">Search Predicate</param>
        /// <param name="includeDeleted">Flag to check whether to returned deleted entities</param>
         /// <returns>Entity that fits predicate</returns>
        T Get(Expression<Func<T, bool>> where, bool includeDeleted = false, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Get entity the fits predicate. Check whether to returned deleted entities <see cref="ISoftDelete"/>
        /// </summary>
        /// <param name="where">Search Predicate</param>
        /// <param name="includeDeleted">Flag to check whether to returned deleted entities</param>
        /// <returns>Entity that fits predicate</returns>
        Task<T> GetAsync(Expression<Func<T, bool>> where, bool includeDeleted = false);

        Task<TResult> GetLookupAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, bool includeDeleted = false);

        /// <summary>
        /// Asynchronous search for an entity that fits predicate with related entities. Option to check if it can be marked as deleted
        /// </summary>
        /// <param name="where">Search Predicate</param>
        /// <param name="includeDeleted">Flag to check whether to returned deleted entities</param>
        /// <param name="includes">Search includes</param>
        /// <remarks>Usage var entity = await GetAsync(e => e.Id == 1, includeDeleted: false, x => x.RelatedEntity, x => x.AnotherEntity);</remarks>
        /// <returns>Task with entity that fits predicate</returns>
        Task<T> GetAsync(Expression<Func<T, bool>> where, bool includeDeleted = false, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Get queriable data set 
        /// </summary>
        /// <param name="includeDeleted"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        IQueryable<T> GetAll(bool includeDeleted = false, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Get a list of all entities. Option to check if entities can be marked as deleted
        /// </summary>
        /// <param name="includeDeleted">Flag whether to include deleted records in the search</param>
        /// <remarks>Usage var entities = GetAll(includeDeleted: false);</remarks>
        /// <returns>Collection of all entities</returns>
        IList<T> GetAll(bool includeDeleted = false);

        /// <summary>
        /// Asynchronous search for a list of all entities. Option to check if entities can be marked as deleted 
        /// </summary>
        /// <param name="includeDeleted">Flag whether to include deleted records in the search</param>
        /// <remarks>Usage var entities = await GetAllAsync(includeDeleted: false);</remarks>
        /// <returns>Task containg a collection of all entities</returns>
        Task<IList<T>> GetAllAsync(bool includeDeleted = false);

        /// <summary>
        /// Get a list of all entities that fit search predicate. Option to check if entities can be marked as deleted
        /// </summary>
        /// <param name="where">Search predicate</param>
        /// <param name="includeDeleted">Flag whether to include deleted records in the search</param>
        /// <returns>Collection of all entities that fit predicate</returns>
        IList<T> GetAll(Expression<Func<T, bool>> where, bool includeDeleted);

        /// <summary>
        /// Asynchronous search for a list of all entities that fit search predicate. Option to check if entities can be marked as deleted
        /// </summary>
        /// <param name="where">Search predicate</param>
        /// <param name="includeDeleted">Flag whether to include deleted records in the search</param>
        /// <returns>Task containg a collection of all entities that fit predicate</returns>
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> where, bool includeDeleted);
        
        /// <summary>
        /// Asynchronous search for an entity that fits predicate with related entities. Option to check if it can be marked as deleted
        /// </summary>
        /// <param name="where">Search Predicate</param>
        /// <param name="includeDeleted">Flag to check whether to returned deleted entities</param>
        /// <param name="includes">Search includes</param>
        /// <remarks>Usage var entity = await GetAsync(e => e.Id == 1, includeDeleted: false, x => x.RelatedEntity, x => x.AnotherEntity);</remarks>
        /// <returns>Task with a list of entitities that fits predicate</returns>
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> where, bool includeDeleted = false, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Asynchronous search for an entity that fits predicate with related entities. Option to check if it can be marked as deleted
        /// </summary>
        /// <param name="includes">Search includes</param>
        /// <remarks>Usage var entity = await GetAsync(e => e.Id == 1, includeDeleted: false, x => x.RelatedEntity, x => x.AnotherEntity);</remarks>
        /// <returns>Task with a list of entitities that fits predicate</returns>
        Task<IList<T>> GetAllAsync(bool includeDeleted = false, params Expression<Func<T, object>>[] includes);

        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> where, bool includeDeleted = false, Func<IQueryable<T>, IQueryable<T>> includeFunc = null);

        /// <summary>
        /// Get selected top count of a list of objects
        /// </summary>
        /// <param name="where">Search predicate</param>
        /// <param name="top">Top count to select</param>
        /// <param name="includeDeleted">Flag whether to include deleted records in the search</param>
        /// <returns>Task containg a collection of all entities that fit predicate</returns>
        Task<IList<T>> GetTopAsync(Expression<Func<T, bool>> where, int top, bool includeDeleted = false);

        /// <summary>
        /// Insert new entity to the database
        /// </summary>
        /// <param name="entity">Entity to insert to the database</param>
        /// <returns>Result for insert result</returns>
        bool Insert(T entity);

        /// <summary>
        /// Asynchronouse insert new entity to the database
        /// </summary>
        /// <param name="entity">Entity to insert to the database</param>
        /// <returns>Task containing insert result</returns>
        Task<bool> InsertAsync(T entity);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <param name="includeDeleted">Flag whether to include deleted records in the update</param>
        /// <returns>Result from the update</returns>
        bool Update(T entity, bool includeDeleted = false);

        /// <summary>
        /// Asynchronous entity update
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <param name="includeDeleted">Flag whether to include deleted records in the update</param>
        /// <returns>Task containing result from the update</returns>
        Task<bool> UpdateAsync(T entity, bool includeDeleted = false);

        /// <summary>
        /// Asynchronous deletion of an entity
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        /// <param name="markAsDeleted">Flag to soft delete an entity</param>
        /// <returns>Task containg deletion status</returns>
        bool Delete(T entity, bool markAsDeleted = false);

        /// <summary>
        /// Asynchronous deletion of an entity
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        /// <param name="markAsDeleted">Flag to soft delete an entity</param>
        /// <returns>Task containg deletion status</returns>
        Task<bool> DeleteAsync(T entity, bool markAsDeleted = false);

        /// <summary>
        /// Asynchronous deletion of a list of entities
        /// </summary>
        /// <param name="entities">List of entities to delete</param>
        /// <returns>Task containg deletion status</returns>
        Task<bool> DeleteAllAsync(IList<T> entities, bool markAsDeleted = false);

        /// <summary>
        /// Bulk inserts to the database
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<bool> BulkyInsertAsync(T[] entities);

        /// <summary>
        /// Bulk updates to the database
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<bool> BulkyUpdateAsync(T[] entities);

        /// <summary>
        /// Update bulk entities for specific properties selected
        /// </summary>
        /// <param name="entities">Collection of entities to update</param>
        /// <param name="propertySelectors">List of properyies to update</param>
        /// <returns></returns>
        Task<bool> BulkyUpdateAsync(T[] entities, params Expression<Func<T, object>>[] propertySelectors);

        /// <summary>
        /// Page all records seleceted
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="size">Number of entities to take</param>
        /// <param name="includeDeleted">Flag to include deleted entities in the search</param>
        /// <param name="includes">Search includes</param>
        /// <returns></returns>
        Task<PagedResult<T>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Page all entities selected in a query
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="page">Page number</param>
        /// <param name="size">Number of entities to take</param>
        /// <param name="includeDeleted">Flag to include deleted entities in the search</param>
        /// <param name="includes">Search includes</param>
        /// <returns></returns>
        Task<PagedResult<T>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Pagenate records that fit predicate
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="size">Page size</param>
        /// <param name="includeDeleted">Flag to include deleted entities in the search</param>
        /// <param name="where">Filter predicate</param>
        /// <returns></returns>
        Task<PagedResult<T>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<T, bool>> where = null);

        /// <summary>
        ///  Pagenate records that fit predicate
        /// </summary>
        /// <param name="token"></param>
        /// <param name="page">Page number</param>
        /// <param name="size">Page size</param>
        /// <param name="includeDeleted">Flag to include deleted entities in the search</param>
        /// <param name="where">Filter predicate</param>
        /// <returns></returns>
        Task<PagedResult<T>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<T, bool>> where = null, bool includeDeleted = false);
        /// <summary>
        /// Project navigation at repository for deep graph objects
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="includeDeleted"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<PagedResult<TResult>> PageLookupAsync<TResult>(int page, int size, bool includeDeleted, Expression<Func<T, TResult>> selector);
        /// <summary>
        /// DBContext HashCode
        /// </summary>
        /// <returns></returns>
        int GetContextHashCode();

    }

}
