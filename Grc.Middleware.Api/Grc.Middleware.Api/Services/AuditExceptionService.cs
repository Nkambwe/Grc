using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Utils;
using System;
using System.Linq.Expressions;
using System.Threading;

namespace Grc.Middleware.Api.Services {

    public class AuditExceptionService : BaseService, IAuditExceptionService {
        public AuditExceptionService(IServiceLoggerFactory loggerFactory, 
                                     IUnitOfWorkFactory uowFactory, 
                                     IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit exceptions in the database", "INFO");

            try {
                return uow.AuditExceptionRepository.Count(); 
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count audit exceptions in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public int Count(Expression<Func<AuditException, bool>> predicate) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit exceptions in the database", "INFO");

            try {
                return uow.AuditExceptionRepository.Count(predicate); 
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count audit exceptions in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit exceptions in the database", "INFO");

            try {
                return await uow.AuditExceptionRepository.CountAsync(cancellationToken); 
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count audit exceptions in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit exceptions in the database", "INFO");

            try {
                return await uow.AuditExceptionRepository.CountAsync(excludeDeleted, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count audit exceptions in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<AuditException, bool>> predicate, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit exceptions in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.CountAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count audit exceptions in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<AuditException, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit exceptions in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count audit exceptions in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public bool Exists(Expression<Func<AuditException, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an audit exceptions exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.AuditExceptionRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for audit exceptions in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<AuditException, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an audit exceptions exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for audit exceptions in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<AuditException, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch audit exceptions if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for audit exceptions in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public AuditException Get(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit exceptions with ID '{id}'", "INFO");

            try
            {
                return uow.AuditExceptionRepository.Get(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exception: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public AuditException Get(Expression<Func<AuditException, bool>> where, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit exceptions that fits predicate >> '{where}'", "INFO");

            try
            {
                return uow.AuditExceptionRepository.Get(where, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exception: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public AuditException Get(Expression<Func<AuditException, bool>> where, bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit exceptions that fits predicate >> '{where}'", "INFO");

            try
            {
                return uow.AuditExceptionRepository.Get(where, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exception: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public IQueryable<AuditException> GetAll(bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit exceptions", "INFO");

            try
            {
                return uow.AuditExceptionRepository.GetAll(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public IList<AuditException> GetAll(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit exceptions", "INFO");

            try
            {
                return uow.AuditExceptionRepository.GetAll(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public IList<AuditException> GetAll(Expression<Func<AuditException, bool>> where, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit exceptions that fit predicate '{where}'", "INFO");

            try
            {
                return uow.AuditExceptionRepository.GetAll(where, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<IList<AuditException>> GetAllAsync(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit exceptions", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetAllAsync(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<IList<AuditException>> GetAllAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit exceptions that fit predicate '{where}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetAllAsync(where, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<IList<AuditException>> GetAllAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit exceptions that fit predicate '{where}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetAllAsync(where, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<IList<AuditException>> GetAllAsync(bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all audit exceptions", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetAllAsync(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<AuditException> GetAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit exceptions with ID '{id}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetAsync(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<AuditException> GetAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit exceptions that fit predicate '{where}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetAsync(where, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<AuditException> GetAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit exceptions that fit predicate '{where}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetAsync(where, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<IList<AuditException>> GetTopAsync(Expression<Func<AuditException, bool>> where, int top, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} audit exceptions that fit predicate >> {where}", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetTopAsync(where, top, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public bool Insert(AuditException exception)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(AuditException exception)
        {
            throw new NotImplementedException();
        }

        public bool Update(AuditException exception, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(AuditException exception, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public bool Delete(AuditException exception, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllAsync(IList<AuditException> exceptions, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(AuditException exception, bool markAsDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyInsertAsync(AuditException[] exceptions)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(AuditException[] exceptions)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkyUpdateAsync(AuditException[] exceptions, params Expression<Func<AuditException, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditException>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<AuditException, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditException>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<AuditException, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditException>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditException, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<AuditException>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<AuditException, bool>> where = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }


    }
}
